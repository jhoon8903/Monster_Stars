using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.PuzzleManagerGroup
{
    public sealed class GridManager : MonoBehaviour
    {
        public int gridHeight;
        public int gridWidth = 6;
        public GameObject grid1Sprite; 
        public GameObject grid2Sprite; 
        private const int CurrentRowType = 1; 
        private const int MaxRows = 8;  
        public Vector3Int bossSpawnArea;
        private GameObject[,] _gridCells;
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
            var newGridCells = new GameObject[gridWidth, gridHeight+1];
            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    newGridCells[x, y + 1] = _gridCells[x, y];
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
            }
            _gridCells = newGridCells;
            PlayerPrefs.SetInt("GridHeight", gridHeight);
        }
        public void ApplyBossSpawnColor(Vector3Int bossArea)
        {
            bossSpawnArea = bossArea;
            var orangeColor = new Color32(255,147, 0, 255); 
            var brownColor = new Color32(217, 191, 156, 255); 

            for (var x = bossSpawnArea.x - 1; x <= bossSpawnArea.x + 1; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) continue;
                    var cellRenderer = _gridCellRenderers[x, y];
                    cellRenderer.DOColor((x + y) % 2 == 0 ? orangeColor : brownColor, 1.0f);
                }
            }
        }

        public IEnumerator ResetBossSpawnColor()
        {
            var backgroundColor = new Color32(22, 101, 123, 255);
            Camera.main.DOColor(backgroundColor, 2.0f);
            var color = new Color(1, 1, 1, 1);

            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    var cellRenderer = _gridCellRenderers[x, y];
                    cellRenderer.DOColor(color, 2.0f);
                }
            }
            yield return null;
        }
    }
}
