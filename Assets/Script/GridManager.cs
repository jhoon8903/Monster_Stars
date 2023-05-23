using Script.CharacterManagerScript;
using UnityEngine;

namespace Script
{
    public sealed class GridManager : MonoBehaviour
    {
        public int gridHeight = 6;  // 그리드의 높이
        public int gridWidth = 6;  // 그리드의 너비
        public GameObject grid1Sprite;  // 그리드 스프라이트 1
        public GameObject grid2Sprite;  // 그리드 스프라이트 2
        private int _currentRowType = 1;  // 현재 행의 타입
        private const int MaxRows = 9;  // 최대 행 개수
        [SerializeField] private CharacterPool characterPool;  // 캐릭터 풀
        [SerializeField] private SpawnManager spawnManager;  // 스폰 매니저


        // 그리드 초기 생성
        public void GenerateInitialGrid()
        {
            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    var spritePrefab = (x + y) % 2 == 0 ? grid1Sprite : grid2Sprite;
                    Instantiate(spritePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                }
            }
        }
        
         //AddRow() 메소드 호출 시 새로운 행을 생성하고 그리드에 새로운 캐릭터를 스폰함
         public void AddRow()
        {
            if (gridHeight >= MaxRows) return;  // 최대 행 개수를 초과하면 추가 행을 생성하지 않음
            gridHeight++;  // 그리드 높이 증가
            
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
                Instantiate(spritePrefab, new Vector3(x, 0, 0), Quaternion.identity, transform);
            }
            _currentRowType = _currentRowType == 1 ? 2 : 1;  // 현재 행의 타입 변경
            
            var notUseCharacters = characterPool.NotUsePoolCharacterList();
            for (var x = 0; x < gridWidth; x++)
            {
                var randomCharacterIndex = Random.Range(0, notUseCharacters.Count);
                var character = notUseCharacters[randomCharacterIndex];
                character.transform.position = new Vector3(x, 0, 0);
                character.SetActive(true);
                notUseCharacters.RemoveAt(randomCharacterIndex);
            }
        }
    }
}
