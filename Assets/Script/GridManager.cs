using Script.CharacterManagerScript;
using UnityEngine;
using DG.Tweening;

namespace Script
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
            if (gridHeight >= MaxRows) return;  // 최대 행 개수를 초과하면 추가 행을 생성하지 않음
            
            var newGridCells = new GameObject[gridWidth, gridHeight + 1];
            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    newGridCells[x, y] = _gridCells[x, y];
                }
            }
            gridHeight++;
            foreach (var character in characterPool.UsePoolCharacterList())
            {
                var newPosition = character.transform.position;
                newPosition.y += 1;
                character.transform.position = newPosition;
            }
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
                newGridCells[x, gridHeight - 1] = newCell; // Store the new cell in the newGridCells array
            }
            _currentRowType = _currentRowType == 1 ? 2 : 1;  // 현재 행의 타입 변경
            _gridCells = newGridCells; // Update _gridCells to point to the new array
        }


         public void ApplyBossSpawnColor(Vector3Int bossArea)
         {
             Debug.Log("BossStage");
             bossSpawnArea = bossArea;
             var orangeColor = new Color(1.0f, 0.5f, 0.0f); // RGB로 주황색 정의
             var brownColor = new Color(0.6f, 0.4f, 0.2f); // RGB로 갈색 정의

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
         public void ResetBossSpawnColor()
         {
             var color1 = grid1Sprite.GetComponent<SpriteRenderer>().color; 
             var color2 = grid2Sprite.GetComponent<SpriteRenderer>().color;
             for (var x = bossSpawnArea.x - 1; x <= bossSpawnArea.x + 1; x++)
             {
                 for (var y = 0; y < gridHeight; y++)
                 {
                     if (x < 0 || x >= gridWidth) continue;
                     var cell = _gridCells[x, y];
                     cell.GetComponent<SpriteRenderer>().DOColor((x + y) % 2 == 0 ? color1 : color2, 1.0f);
                 }
             }
         }
    }
}
