using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int _gridHeight = 6;
    public int _gridWidth = 6;
    public GameObject grid1Sprite;
    public GameObject grid2Sprite;
    public CharacterPool characterPool;
    private int _currentRowType = 1;
    private int _activeGridCount = 0;
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
                    //int randomCharacterIndex = Random.Range(0, characterManager.characterGroup.Count);
                    spawnManager.SpawnCharacterAtPosition(x, y);
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

            // Move the entire grid down
            foreach (Transform child in transform)
            {
                Vector3 newPosition = child.position;
                newPosition.y += 1;
                child.position = newPosition;
            }

            // Create new row and spawn characters
            for (int x = 0; x < _gridWidth; x++)
            {
                GameObject spritePrefab = (x + _currentRowType) % 2 == 0 ? grid1Sprite : grid2Sprite;
                GameObject cell = Instantiate(spritePrefab, new Vector3(x, 0, 0), Quaternion.identity, transform);
            }

            _currentRowType = _currentRowType == 1 ? 2 : 1;

            List<GameObject> inactiveCharacters = characterPool.GetPooledCharacters().Where(character => !character.activeInHierarchy).ToList();
            for (int x = 0; x < _gridWidth; x++)
            {
                if (!spawnManager.IsCharacterAtPosition(new Vector3(x, 0, 0)))
                {
                    int randomCharacterIndex = Random.Range(0, inactiveCharacters.Count);
                    GameObject character = inactiveCharacters[randomCharacterIndex];
                    character.transform.position = new Vector3(x, 0, 0);
                    character.SetActive(true);
                    inactiveCharacters.RemoveAt(randomCharacterIndex);
                }
            }
        }
    }
    public int GetActiveGridCount()
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    public int GetTotalGridPositions()
    {
        return _gridWidth * _gridHeight;
    }

    public GameObject GetEmptyGrid()
    {
        Debug.Log("GetEmptyGrid");
        List<GameObject> emptyGrids = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
            {
                emptyGrids.Add(child.gameObject);
            }
        }

        if (emptyGrids.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyGrids.Count);
            GameObject selectedGrid = emptyGrids[randomIndex];

            // Move all game objects above this grid up by 1 in y direction
            foreach (GameObject character in spawnManager.GetPooledCharacters())
            {
                if (character.activeInHierarchy && character.transform.position.y > selectedGrid.transform.position.y)
                {
                    Vector3 newPosition = character.transform.position;
                    newPosition.y += 1;
                    character.transform.position = newPosition;
                }
            }

            return selectedGrid;
        }

        return null;
    }

    public void IncrementActiveGridCount()
    {
        _activeGridCount++;
    }

}
