using DG.Tweening;
using UnityEngine;

namespace Script.PuzzleManagerGroup
{
    public sealed class GridManager : MonoBehaviour
    {
        public int gridHeight = 6; 
        public int gridWidth = 6; 
        public GameObject grid1Sprite; 
        public GameObject grid2Sprite; 
        private const int CurrentRowType = 1; 
        private const int MaxRows = 8;  
        public Vector3Int bossSpawnArea;
        private GameObject[,] _gridCells;

        public void GenerateInitialGrid()
        {
            _gridCells = new GameObject[gridWidth, gridHeight];
            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    var spritePrefab = (x + y) % 2 == 0 ? grid1Sprite : grid2Sprite;
                    var newCell = Instantiate(spritePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                    _gridCells[x, y] = newCell;
                }
            }
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
                     var cell = _gridCells[x, y];
                     cell.GetComponent<SpriteRenderer>().DOColor((x + y) % 2 == 0 ? orangeColor : brownColor, 1.0f);
                 }
             }
         }

         public void ResetBossSpawnColor()
         {
             var backgroundColor = new Color32(22, 101, 123, 255);
             Camera.main.DOColor(backgroundColor, 2.0f);
             var color1 = new Color32(206, 82, 206, 255);
             var color2 = new Color32(250, 157, 65, 255);

             for (var y = 0; y < gridHeight; y++)
             {
                 for (var x = 0; x < gridWidth; x++)
                 {
                     var cell = _gridCells[x, y];
                     cell.GetComponent<SpriteRenderer>().DOColor((x + y) % 2 == 0 ? color2 : color1, 2.0f);
                 }
             }
         }
    }
}
