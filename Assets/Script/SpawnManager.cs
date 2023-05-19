using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace Script
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField]
        private GridManager gridManager;
        [SerializeField]
        private CharacterPool characterPool;

        private bool spawnDone = false;
        public List<GameObject> GetPooledCharacters()
        {
            return characterPool.GetPooledCharacters();
        }
        // Grid 전체에 케릭터 Object를 생성하는 메소드
        public bool SpawnCharacters()
        {
            var availablePositions = new List<Vector2Int>();
            for (var x = 0; x < gridManager.gridWidth; x++)
            {
                for (var y = 0; y < gridManager.gridWidth; y++)
                {
                    availablePositions.Add(new Vector2Int(x, y));
                }
            }
            var totalGridPositions = gridManager.gridWidth * gridManager.gridHeight;
            for (var i = 0; i < totalGridPositions; i++)
            {
                var randomPositionIndex = Random.Range(0, availablePositions.Count);
                var randomPosition = availablePositions[randomPositionIndex];
                availablePositions.RemoveAt(randomPositionIndex);
                SpawnCharacterAtPosition(randomPosition.x, randomPosition.y);
            }
            spawnDone = true;
            return spawnDone;
        }
        // 특정 Grid 좌표에 케릭터를 생성하는 메소드
        public void SpawnCharacterAtPosition(int x, int y)
        {
            var spawnPosition = new Vector2(x, y);
            if (IsCharacterAtPosition(spawnPosition)) return;
            var pooledCharacter = characterPool.GetPooledCharacter();
            if (pooledCharacter == null) return;
            pooledCharacter.transform.position = spawnPosition;
            pooledCharacter.SetActive(true);
        }
        // 특정 위치에 Character가 존재하는지 확인하는 메소드
        public bool IsCharacterAtPosition(Vector3 position)
        {
            return GetCharacterAtPosition(position) != null;
        }
        // 특정 위치에 있는 케릭터를 반환하는 메소드
        public GameObject GetCharacterAtPosition(Vector3 position)
        {
            var list = characterPool.GetPooledCharacters();
            return list.FirstOrDefault(character => 
                character.activeInHierarchy && character.transform.position == position);
        }
        // 비어있는 Grid 위에 Character를 이동 시키는 메소드

        public static void MoveCharacter(Vector2 emptyGridPosition)
        {
            
        }
        private void RespawnCharacter(int column)
        {
            var inactiveCharacters = characterPool.GetPooledCharacters()
                .Where(character => !character.activeInHierarchy)
                .ToList();

            if (inactiveCharacters.Count <= 0) return;
            {
                // Find the highest empty position in the column
                var maxY = gridManager.gridHeight - 1;
                for (; maxY >= 0; maxY--)
                {
                    var checkPos = new Vector2(column, maxY);
                    if (characterPool.GetPooledCharacters().All(character 
                            => new Vector2(character.transform.position.x, character.transform.position.y) != checkPos))
                    {
                        break;
                    }
                }

                var position = new Vector2(column, maxY);
                var initialPosition = new Vector2(column, -1);
                var randomCharacterIndex = Random.Range(0, inactiveCharacters.Count);
                var character = inactiveCharacters[randomCharacterIndex];
                character.transform.position = initialPosition;
                character.SetActive(true);
                character.transform.DOMove(position, 0.2f).WaitForCompletion();
            }
        }
    }
}