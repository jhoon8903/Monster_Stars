using System.Collections;
using UnityEngine;

namespace Script.PuzzleManagerGroup
{
    public sealed class GridManager : MonoBehaviour
    {
        public int gridHeight;
        public int gridWidth = 6;
        public GameObject grid1Sprite; 
        public GameObject grid2Sprite;
        public GameObject bossGrid1Sprite;
        public GameObject bossGrid2Sprite;
        private const int CurrentRowType = 1; 
        private const int MaxRows = 8;  
        public Vector3Int bossSpawnArea;
        private GameObject[,] _gridCells;
        private GameObject[,] _bossGridCells;
        private SpriteRenderer[,] _gridCellRenderers;

        public void GenerateInitialGrid(int height)
        {
            gridHeight = height;
            _gridCells = new GameObject[gridWidth, gridHeight];
            _gridCellRenderers = new SpriteRenderer[gridWidth, gridHeight];
            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    var spritePrefab = (x + y) % 2 == 0 ? grid1Sprite : grid2Sprite;
                    var newCell = Instantiate(spritePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                    _gridCells[x, y] = newCell;
                    _gridCellRenderers[x, y] = newCell.GetComponent<SpriteRenderer>();
                }
            }
            PlayerPrefs.SetInt("GridHeight", gridHeight);
        }

        public void AddRow()
        {
            if (gridHeight >= MaxRows) return;

            var newGridHeight = gridHeight + 1;
            PlayerPrefs.SetInt("GridHeight", newGridHeight);
            var newGridCells = new GameObject[gridWidth, newGridHeight];
            var newBossGridCells = new GameObject[gridWidth, newGridHeight]; // Initialize newBossGridCells array here
            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    newGridCells[x, y + 1] = _gridCells[x, y];
                    if (_bossGridCells != null) // If the boss grid exists
                    {
                        newBossGridCells[x, y + 1] = _bossGridCells[x, y]; // Copy existing boss grid cells
                    }
                }
            }
            gridHeight++;
            foreach (Transform child in transform)
            {
                var newPosition = child.position;
                newPosition.y += 1;
                child.position = newPosition;
            }
            for (var x = 0; x < gridWidth; x++)
            {
                var spritePrefab = (x + CurrentRowType) % 2 == 0 ? grid1Sprite : grid2Sprite;
                var newCell = Instantiate(spritePrefab, new Vector3(x, 0, 0), Quaternion.identity, transform);
                newGridCells[x, 0] = newCell;
                if (_bossGridCells != null) // If the boss grid exists
                {
                    newBossGridCells[x, 0] = null; // Add new row to boss grid
                }
            }
            _gridCells = newGridCells;
            _bossGridCells = newBossGridCells; // Assign the new array to _bossGridCells
            gridHeight = newGridHeight;
            Debug.Log(gridHeight);
        }

        public void ApplyBossSpawnColor(Vector3Int bossArea)
        {
            bossSpawnArea = bossArea;
            _bossGridCells = new GameObject[gridWidth, gridHeight];

            for (var x = bossSpawnArea.x - 1; x <= bossSpawnArea.x + 1; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) continue;

                    var spritePrefab = (x + y) % 2 == 0 ? bossGrid1Sprite : bossGrid2Sprite;
                    var newCell = Instantiate(spritePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                    _bossGridCells[x, y] = newCell;
                    _gridCells[x, y].SetActive(false);
                }
            }
        }

        public IEnumerator ResetBossSpawnColor()
        {
            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < _bossGridCells.GetLength(1); y++) // Use actual array height here
                {
                    if (_bossGridCells[x, y] == null) continue;
                    Destroy(_bossGridCells[x, y]);
                    _gridCells[x, y].SetActive(true);
                }
            }
            yield return null;
        }
    }
}
