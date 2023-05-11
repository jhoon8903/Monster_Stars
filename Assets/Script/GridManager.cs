using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridHeight = 6;
    public GameObject row1Prefab;
    public GameObject row2Prefab;
    public RespawnManager respawnManager;

    private int currentRow = 1;

    public IEnumerator GenerateInitialGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            GameObject newRow = GenerateRow(new Vector3(0, 1 - y, 0));
            currentRow = (currentRow == 1) ? 2 : 1;
        }

        yield return new WaitForSeconds(1.0f);
        foreach (Transform rowTransform in transform)
        {
            respawnManager.SpawnCharactersInRow(rowTransform);
        }
    }

    public void AddRow() // row생성 호출 시 추가 row 생성
    {
        gridHeight++;
        GameObject newRow = GenerateRow(new Vector3(0, -gridHeight + 1, 0));
        respawnManager.SpawnCharactersInRow(newRow.transform);
        currentRow = (currentRow == 1) ? 2 : 1;
    }


    private GameObject GenerateRow(Vector3 position)
    {
        GameObject rowPrefab = (currentRow == 1) ? row1Prefab : row2Prefab;
        return Instantiate(rowPrefab, position, Quaternion.identity, transform);
    }


    public bool IsPositionInsideGrid(Vector2 position)
    {
        return position.x >=  -3  && position.y < gridHeight;
    }

    public GameObject GetCharacterAtPosition(Vector2 position)
    {
        foreach (Transform characterTransform in respawnManager.characterPool.transform)
        {
            if (Mathf.Approximately(characterTransform.position.x, position.x) && Mathf.Approximately(characterTransform.position.y, position.y))
            {
                return characterTransform.gameObject;
            }
        }
        return null;
    }


    public void RemoveCharacterFromGrid(Vector2 position)
    {
        Transform gridTransform = GetGridTransformAtPosition(position);
        if (gridTransform != null && gridTransform.childCount > 0)
        {
            GameObject character = gridTransform.GetChild(0).gameObject;
            character.transform.SetParent(null);
            character.SetActive(false);
        }
    }

    public void AddCharacterToGrid(Vector2 position, GameObject character)
    {
        Transform gridTransform = GetGridTransformAtPosition(position);
        if (gridTransform != null)
        {
            character.transform.SetParent(gridTransform);
            character.transform.position = new Vector3(gridTransform.position.x, gridTransform.position.y, -0.5f);
            character.SetActive(true);
        }
    }

    private Transform GetGridTransformAtPosition(Vector2 position)
    {
        foreach (Transform rowTransform in transform)
        {
            foreach (Transform gridTransform in rowTransform)
            {
                if (Mathf.Approximately(gridTransform.position.x, position.x) && Mathf.Approximately(gridTransform.position.y, position.y))
                {
                    return gridTransform;
                }
            }
        }
        return null;
    }

    public void RemoveCharacterAtPosition(Vector2 position)
    {
        GameObject character = GetCharacterAtPosition(position);
        if (character != null)
        {
            character.transform.SetParent(null);
            character.SetActive(false);
        }
    }

    public List<GameObject> FindMatchedCharacters(Vector2 position, int minMatchCount)
    {
        List<GameObject> matchedCharacters = new List<GameObject>();

        GameObject character = GetCharacterAtPosition(position);
        if (character != null)
        {
            string characterPrefabName = character.transform.parent.name.Replace("(Clone)", "").Trim();

            // Check horizontal matches
            List<GameObject> horizontalMatches = new List<GameObject>();
            horizontalMatches.Add(character);
            horizontalMatches.AddRange(FindMatchesInDirection(position, characterPrefabName, Vector2.right));
            horizontalMatches.AddRange(FindMatchesInDirection(position, characterPrefabName, Vector2.left));

            if (horizontalMatches.Count >= minMatchCount)
            {
                matchedCharacters.AddRange(horizontalMatches);
            }

            // Check vertical matches
            List<GameObject> verticalMatches = new List<GameObject>();
            verticalMatches.Add(character);
            verticalMatches.AddRange(FindMatchesInDirection(position, characterPrefabName, Vector2.up));
            verticalMatches.AddRange(FindMatchesInDirection(position, characterPrefabName, Vector2.down));

            if (verticalMatches.Count >= minMatchCount)
            {
                matchedCharacters.AddRange(verticalMatches);
            }
        }

        return matchedCharacters;
    }

    private List<GameObject> FindMatchesInDirection(Vector2 startPosition, string characterPrefabName, Vector2 direction)
    {
        List<GameObject> matches = new List<GameObject>();

        Vector2 currentPosition = startPosition;
        while (true)
        {
            currentPosition += direction;
            GameObject otherCharacter = GetCharacterAtPosition(currentPosition);

            if (otherCharacter != null)
            {
                string otherCharacterPrefabName = otherCharacter.transform.parent.name.Replace("(Clone)", "").Trim();
                if (otherCharacterPrefabName == characterPrefabName)
                {
                    matches.Add(otherCharacter);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return matches;
    }

}
