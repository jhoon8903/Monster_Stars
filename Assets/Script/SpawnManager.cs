using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private CharacterManager characterManager;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private CharacterPool characterPool;

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

        int totalGridPositions = gridManager._gridWidth * gridManager._gridHeight;
        int charactersPerGroup = totalGridPositions / characterManager.characterGroup.Count;
        int extraCharacters = totalGridPositions % characterManager.characterGroup.Count;

        for (int i = 0; i < characterManager.characterGroup.Count; i++)
        {
            int charactersToSpawn = charactersPerGroup + (i < extraCharacters ? 1 : 0);
            for (int j = 0; j < charactersToSpawn; j++)
            {
                if (availablePositions.Count == 0)
                {
                    Debug.LogWarning("Not enough available positions on the grid.");
                    return;
                }

                int randomPositionIndex = Random.Range(0, availablePositions.Count);
                Vector2Int randomPosition = availablePositions[randomPositionIndex];
                availablePositions.RemoveAt(randomPositionIndex);
                int characterPrefabIndex = characterManager.characterGroup[i].prefabIndex;


                SpawnCharacterAtPosition(characterPrefabIndex, randomPosition.x, randomPosition.y);
            }
        }
    }


    public void SpawnCharacterAtPosition(int characterPrefabIndex, int x, int y)
    {
        Vector3 spawnPosition = new Vector3(x, y, 0);
        if (!IsCharacterAtPosition(spawnPosition))
        {
            GameObject characterPrefab = characterManager.characterGroup[characterPrefabIndex].gameObject;
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

}