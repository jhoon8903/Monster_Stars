using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private CharacterManager characterManager;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private CharacterPool characterPool;

    private int[] objectCounts = new int[5];

    public void SpawnCharacters()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        for (int x = 0; x < gridManager._gridWidth; x++)
        {
            for (int y = 0; y < gridManager._gridHeight; y++)
            {
                availablePositions.Add(new Vector2Int(x, y));
            }
        }

        int[] targetCounts = { 7, 7, 7, 7, 8 };

        int[] currentCounts = new int[5];

        while (availablePositions.Count > 0 && currentCounts.Sum() < 36)
        {
            int randomCharacterIndex = Random.Range(0, characterManager.characterGroup.Count);
            if (currentCounts[randomCharacterIndex] < targetCounts[randomCharacterIndex])
            {
                int randomPositionIndex = Random.Range(0, availablePositions.Count);
                Vector2Int randomPosition = availablePositions[randomPositionIndex];
                availablePositions.RemoveAt(randomPositionIndex);

                SpawnCharacterAtPosition(randomCharacterIndex, randomPosition.x, randomPosition.y);
                currentCounts[randomCharacterIndex]++;
            }
        }
    }

    public void SpawnCharacterAtPosition(int characterIndex, int x, int y)
    {
        Vector3 spawnPosition = new Vector3(x, y, 0);
        if (!IsCharacterAtPosition(spawnPosition))
        {
            GameObject characterPrefab = characterManager.characterGroup[characterIndex].gameObject;
            GameObject pooledCharacter = characterPool.GetPooledCharacter();
            if (pooledCharacter != null)
            {
                pooledCharacter.GetComponent<CharacterBase>().Setup(characterPrefab.GetComponent<CharacterBase>());
                pooledCharacter.transform.position = spawnPosition;
                pooledCharacter.SetActive(true);
            }
        }
    }


    public List<GameObject> GetPooledCharacters()
    {
        return characterPool.GetPooledCharacters();
    }


    public bool IsCharacterAtPosition(Vector3 position)
    {
        foreach (GameObject character in characterPool.GetPooledCharacters())
        {
            if (character.activeInHierarchy && character.transform.position == position)
            {
                return true;
            }
        }
        return false;
    }


    public void DeactivateCharactersOutsideGrid(int gridWidth, int gridHeight)
    {
        foreach (GameObject character in characterPool.GetPooledCharacters())
        {
            if (character.activeInHierarchy)
            {
                Vector3 position = character.transform.position;
                if (position.x < 0 || position.x >= gridWidth || position.y < 0 || position.y >= gridHeight)
                {
                    character.SetActive(false);
                }
            }
        }
    }


}