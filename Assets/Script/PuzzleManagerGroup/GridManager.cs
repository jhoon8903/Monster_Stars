using DG.Tweening;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.PuzzleManagerGroup
{
    public sealed class GridManager : MonoBehaviour
    {
        public int gridHeight = 6;  // 그리드의 높이
        public int gridWidth = 6;  // 그리드의 너비
        public GameObject grid1Sprite;  // 그리드 스프라이트 1
        public GameObject grid2Sprite;  // 그리드 스프라이트 2
        private int _currentRowType = 1;  // 현재 행의 타입
        private const int MaxRows = 8;  // 최대 행 개수
        [SerializeField] private CharacterPool characterPool; // 캐릭터 풀
        // 보스 스폰 영역 정보를 저장하는 변수
        public Vector3Int bossSpawnArea;
        // 그리드 스프라이트를 관리하는 2차원 배열
        private GameObject[,] _gridCells;


        // 그리드 초기 생성
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
            if (gridHeight >= MaxRows) return; // Do not create additional rows if the maximum number of rows is exceeded

            var newGridCells = new GameObject[gridWidth, gridHeight+1];

            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    newGridCells[x, y + 1] = _gridCells[x, y]; // move rows one step up
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
                var spritePrefab = (x + _currentRowType) % 2 == 0 ? grid1Sprite : grid2Sprite;
                var newCell = Instantiate(spritePrefab, new Vector3(x, 0, 0), Quaternion.identity, transform);
                newGridCells[x, 0] = newCell; // Store the new cell in the newGridCells array
            }

            _gridCells = newGridCells; // Update _gridCells to point to the new array
            ResetBossSpawnColor();
        }
        public void ApplyBossSpawnColor(Vector3Int bossArea)
         {
             bossSpawnArea = bossArea;
             var orangeColor = new Color32(255,147, 0, 255); // RGB로 주황색 정의
             var brownColor = new Color32(217, 191, 156, 255); // RGB로 갈색 정의

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

         // 보스 스폰 위치의 색상을 원래대로 복원하는 함수
         private void ResetBossSpawnColor()
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
