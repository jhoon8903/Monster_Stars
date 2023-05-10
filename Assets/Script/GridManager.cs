using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridHeight = 6;
    public GameObject row1Prefab;
    public GameObject row2Prefab;
    public CharacterManager characterManager;
    public CharacterPool characterPool;

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
            SpawnCharactersInRow(rowTransform);
        }
    }

    public void AddRow() // row생성 호출 시 추가 row 생성
    {
        gridHeight++;
        GameObject newRow = GenerateRow(new Vector3(0, -gridHeight + 1, 0));
        SpawnCharactersInRow(newRow.transform);
        currentRow = (currentRow == 1) ? 2 : 1;
    }


    private GameObject GenerateRow(Vector3 position)
    {
        GameObject rowPrefab = (currentRow == 1) ? row1Prefab : row2Prefab;
        return Instantiate(rowPrefab, position, Quaternion.identity, transform);
    }


    private void SpawnCharactersInRow(Transform rowTransform)
    {
        for (int i = 0; i < rowTransform.childCount; i++)
        {
            Transform gridTransform = rowTransform.GetChild(i);
            GameObject characterPrefab = characterManager.GetRandomCharacterPrefab();
            Vector3 newPosition = new Vector3(gridTransform.position.x, gridTransform.position.y, -0.5f);
            GameObject characterInstance = characterPool.GetPooledObject(characterPrefab);
            characterInstance.transform.position = newPosition;
            characterInstance.transform.SetParent(gridTransform, true);
            characterInstance.SetActive(true);

            // Disable all child objects except for the one named "level0"
            for (int j = 0; j < characterInstance.transform.childCount; j++)
            {
                Transform child = characterInstance.transform.GetChild(j);
                if (child.name != "level0")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }



}
