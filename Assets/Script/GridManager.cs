using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public sealed class GridManager : MonoBehaviour
    {
        [FormerlySerializedAs("_gridHeight")] public int gridHeight = 6;
        [FormerlySerializedAs("_gridWidth")] public int gridWidth = 6;
        public GameObject grid1Sprite;
        public GameObject grid2Sprite;
        public CharacterPool characterPool;
        private int _currentRowType = 1;
        [FormerlySerializedAs("_maxRows")] [SerializeField] private int maxRows = 9;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CharacterManager characterManager;

        private void Start()
        {
            GenerateInitialGrid();  // 초기 Grid 생성
            spawnManager.SpawnCharacters(); // CharacterObject 생성
        }

        //  기본 Grid 생성 메소드
        private void GenerateInitialGrid()
        {
            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    var spritePrefab = (x + y) % 2 == 0 ? grid1Sprite : grid2Sprite;
                    var cell = Instantiate(spritePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                    if (!spawnManager.IsCharacterAtPosition(new Vector3(x, y, 0)))
                    {
                        //int randomCharacterIndex = Random.Range(0, characterManager.characterGroup.Count);
                        spawnManager.SpawnCharacterAtPosition(x, y);
                    }
                }
            }
        }

        //  AddRow() 버튼 클릭 혹은 Method 호출 시 Grid Row 생성
        public void AddRow()
        {
            if (gridHeight >= maxRows) return;
            gridHeight++;

            // Move existing characters up
            foreach (var character in spawnManager.GetPooledCharacters())
            {
                if (!character.activeInHierarchy) continue;
                var newPosition = character.transform.position;
                newPosition.y += 1;
                character.transform.position = newPosition;
            }

            // Move the entire grid down
            foreach (Transform child in transform)
            {
                var newPosition = child.position;
                newPosition.y += 1;
                child.position = newPosition;
            }
            
            // Create new row and spawn characters
            for (var x = 0; x < gridWidth; x++)
            {
                var spritePrefab = (x + _currentRowType) % 2 == 0 ? grid1Sprite : grid2Sprite;
                var cell = Instantiate(spritePrefab, new Vector3(x, 0, 0), Quaternion.identity, transform);
            }
            _currentRowType = _currentRowType == 1 ? 2 : 1;
            var inactiveCharacters = characterPool.GetPooledCharacters().Where(character => !character.activeInHierarchy).ToList();
            for (var x = 0; x < gridWidth; x++)
            {
                if (spawnManager.IsCharacterAtPosition(new Vector3(x, 0, 0))) continue;
                var randomCharacterIndex = Random.Range(0, inactiveCharacters.Count);
                var character = inactiveCharacters[randomCharacterIndex];
                character.transform.position = new Vector3(x, 0, 0);
                character.SetActive(true);
                inactiveCharacters.RemoveAt(randomCharacterIndex);
            }
        }
        
        //  비어있는 Grid를 반환
        public GameObject GetEmptyGrid()
        {
            var emptyGrids = (from Transform child in transform where !child.gameObject.activeSelf select child.gameObject).ToList();
            if (emptyGrids.Count <= 0) return null;
            var randomIndex = Random.Range(0, emptyGrids.Count);
            var selectedGrid = emptyGrids[randomIndex];
            foreach (var character in spawnManager.GetPooledCharacters())
            {
                if (!character.activeInHierarchy ||
                    !(character.transform.position.y > selectedGrid.transform.position.y)) continue;
                var newPosition = character.transform.position;
                newPosition.y += 1;
                character.transform.position = newPosition;
            }
            return selectedGrid;
        }

        // 비어있는 Grid의 X 좌표를 반환
        public List<int> GetFreeXPositions()
        {
            var freeXPositions = new List<int>();
            for (var x = 0; x < gridWidth; x++)
            {
                if (!spawnManager.IsCharacterAtPosition(new Vector3(x, 0, 0)))
                {
                    freeXPositions.Add(x);
                }
            }
            return freeXPositions;
        }
    }
}
