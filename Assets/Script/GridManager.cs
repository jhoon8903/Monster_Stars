using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int _gridHeight = 6;
    public int _gridWidth = 6;
    public GameObject grid1Sprite;
    public GameObject grid2Sprite;
    private int currentRowType = 1;
    [SerializeField] private int _maxRows = 9;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private CharacterManager characterManager;

    private void Start()
    {
        GenerateInitialGrid();
        spawnManager.SpawnCharacters();
    }

    public void GenerateInitialGrid()
    {
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                GameObject spritePrefab = (x + y) % 2 == 0 ? grid1Sprite : grid2Sprite;
                GameObject cell = Instantiate(spritePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                if (!spawnManager.IsCharacterAtPosition(new Vector3(x, y, 0)))
                {
                    int randomCharacterIndex = Random.Range(0, characterManager.characterGroup.Count);
                    spawnManager.SpawnCharacterAtPosition(randomCharacterIndex, x, y);
                }
            }
        }
    }


    public void AddRow()
    {
        if (_gridHeight < _maxRows)
        {
            _gridHeight++;

            // Move existing characters up
            foreach (GameObject character in spawnManager.GetPooledCharacters())
            {
                if (character.activeInHierarchy)
                {
                    Vector3 newPosition = character.transform.position;
                    newPosition.y += 1;
                    character.transform.position = newPosition;
                }
            }

            // Create new row and spawn characters
            for (int x = 0; x < _gridWidth; x++)
            {
                GameObject spritePrefab = (x + currentRowType) % 2 == 0 ? grid1Sprite : grid2Sprite;
                Instantiate(spritePrefab, new Vector3(x, -1, 0), Quaternion.identity, transform);
                if (!spawnManager.IsCharacterAtPosition(new Vector3(x, -1, 0)))
                {
                    int randomCharacterIndex = Random.Range(0, characterManager.characterGroup.Count);
                    spawnManager.SpawnCharacterAtPosition(randomCharacterIndex, x, -1);
                }
            }

            // Move the entire grid up
            foreach (Transform child in transform)
            {
                Vector3 newPosition = child.position;
                newPosition.y += 1;
                child.position = newPosition;
            }
            currentRowType = currentRowType == 1 ? 2 : 1;
            spawnManager.DeactivateCharactersOutsideGrid(_gridWidth, _gridHeight);
        }
    }
}
