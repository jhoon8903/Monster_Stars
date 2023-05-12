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
                GameObject spritePrefab = (x + currentRowType) % 2 == 0 ? grid1Sprite : grid2Sprite;
                GameObject cell = Instantiate(spritePrefab, new Vector3(x, 0, 0), Quaternion.identity, transform);
            }

            currentRowType = currentRowType == 1 ? 2 : 1;

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
}
