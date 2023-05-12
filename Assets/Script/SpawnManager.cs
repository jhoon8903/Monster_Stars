using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
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
        int charactersToSpawn = totalGridPositions;

        for (int i = 0; i < charactersToSpawn; i++)
        {
            if (availablePositions.Count == 0)
            {
                Debug.LogWarning("Not enough available positions on the grid.");
                return;
            }

            int randomPositionIndex = Random.Range(0, availablePositions.Count);
            Vector2Int randomPosition = availablePositions[randomPositionIndex];
            availablePositions.RemoveAt(randomPositionIndex);

            SpawnCharacterAtPosition(randomPosition.x, randomPosition.y);
        }
    }

    public void SpawnCharacterAtPosition(int x, int y)
    {
        Vector3 spawnPosition = new Vector3(x, y, -0.5f);
        if (!IsCharacterAtPosition(spawnPosition))
        {
            GameObject pooledCharacter = characterPool.GetPooledCharacter();

            if (pooledCharacter != null)
            {
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
