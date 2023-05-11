using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int _gridHeight = 6;
    public int _gridWidth = 6;
    public GameObject grid1Sprite;
    public GameObject grid2Sprite;
    [SerializeField] private int _maxRows = 9;
    private int currentRowType = 1;

    private void Start()
    {
        GenerateInitialGrid();
    }

    public void GenerateInitialGrid()
    {
        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                GameObject spritePrefab = (x + y) % 2 == 0 ? grid1Sprite : grid2Sprite;
                Instantiate(spritePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
            }
        }
    }

    public void AddRow()
    {
        if (_gridHeight < _maxRows)
        {
            _gridHeight++;
            for (int x = 0; x < _gridWidth; x++)
            {
                GameObject spritePrefab = ( x + currentRowType ) % 2 == 0 ? grid1Sprite : grid2Sprite;
                Instantiate(spritePrefab, new Vector3(x, -1, 0), Quaternion.identity, transform);
            }

            foreach (Transform child in transform)
            {
                Vector3 newPosition = child.position;
                newPosition.y += 1;
                child.position = newPosition;
            }
            currentRowType = currentRowType == 1 ? 2 : 1;
        }
    }

    public Transform GetCell(int x, int y)
    {
        if (x >= 0 && x < _gridWidth && y >= 0 && y < _gridHeight)
        {
            int index = (_gridHeight - y - 1) * _gridWidth + x;
            if (index < transform.childCount)
            {
                return transform.GetChild(index);
            }
        }
        return null;
    }

    public bool IsCellEmpty(int x, int y)
    {
        Transform cell = GetCell(x, y);
        Debug.Log(cell);
        return cell != null && cell.childCount == 0;
    }
}
