using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private CharacterManager characterManager;
    [SerializeField]
    private GridManager gridManager;

    public void SpawnCharacters()
    {
        Debug.Log("Spawn!");
        for (int y = 0; y < gridManager._gridHeight; y++)
        {
            for (int x = 0; x < gridManager._gridWidth; x++)
            {
                float spawnChance = Random.Range(0f, 1f);
                if (spawnChance <= 1.0f && gridManager.IsCellEmpty(x, y))
                {
                    SpawnCharacterAtPosition(x, y);
                }
            }
        }
    }

    private void SpawnCharacterAtPosition(int x, int y)
    {
        Debug.Log("SpawnCharacterAtPosition");
        GameObject characterPrefab = characterManager.GetRandomCharacterPrefab();
        if (characterPrefab != null)
        {
            GameObject characterInstance = Instantiate(characterPrefab);
            characterInstance.transform.SetParent(gridManager.GetCell(x, y));
            characterInstance.transform.localPosition = Vector3.zero;
        }
    }
}
