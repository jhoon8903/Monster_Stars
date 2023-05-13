using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private CharacterPool characterPool;

    public List<GameObject> GetPooledCharacters()
    {
        return characterPool.GetPooledCharacters();
    }

    public void SpawnCharacters()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        for (int x = 0; x < gridManager._gridWidth; x++)
        {
            for (int y = 0; y < gridManager._gridWidth; y++)
            {
                availablePositions.Add(new Vector2Int(x, y));
            }
        }

        int totalGridPositions = gridManager._gridWidth * gridManager._gridHeight;
        int charactersToSpawn = totalGridPositions;

        for (int i = 0; i < charactersToSpawn; i++)
        {
            int randomPositionIndex = Random.Range(0, availablePositions.Count);
            Vector2Int randomPosition = availablePositions[randomPositionIndex];
            availablePositions.RemoveAt(randomPositionIndex);

            SpawnCharacterAtPosition(randomPosition.x, randomPosition.y);
        }
    }

    public void SpawnCharacterAtPosition(int x, int y)
    {
        Vector2 spawnPosition = new Vector2(x, y);

        if (!IsCharacterAtPosition(spawnPosition))
        {
            GameObject pooledCharacter = characterPool.GetPooledCharacter();

            if (pooledCharacter != null)
            {
                pooledCharacter.transform.position = spawnPosition;
                pooledCharacter.SetActive(true);
                gridManager.IncrementActiveGridCount();
            }
        }
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

    public GameObject GetCharacterAtPosition(Vector3 position)
    {
        foreach (GameObject character in characterPool.GetPooledCharacters())
        {
            if (character.activeInHierarchy && character.transform.position == position)
            {
                return character;
            }
        }
        return null;
    }

    public void MoveCharactersEmptyGrid(Vector2 emptyGridPosition)
    {
        foreach (GameObject character in characterPool.GetPooledCharacters())
        {
            if (character.transform.position.x == emptyGridPosition.x && character.transform.position.y < emptyGridPosition.y)
            {
                Vector2 newPosition = character.transform.position;
                newPosition.y += 1;
                character.transform.position = newPosition;
            }
        }
        RespawnCharacter();
    }

    public void RespawnCharacter()
    {
        List<GameObject> inactiveCharacters = characterPool.GetPooledCharacters().Where(character => !character.activeInHierarchy).ToList();
        List<int> freeXPositions = gridManager.GetFreeXPositions();
        for (int x = 0; x < freeXPositions.Count; x++)
        {
            int randomCharacterIndex = Random.Range(0, inactiveCharacters.Count);
            GameObject character = inactiveCharacters[randomCharacterIndex];
            character.transform.position = new Vector3(freeXPositions[x], 0, 0);
            character.SetActive(true);
            inactiveCharacters.RemoveAt(randomCharacterIndex);
        }

    }
}