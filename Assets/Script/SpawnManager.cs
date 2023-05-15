using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField]
        private GridManager gridManager;
        [SerializeField]
        private CharacterPool characterPool;
        
        // CharacterPool에서 사용 가능한 Pool 객체를 반환
        public List<GameObject> GetPooledCharacters()
        {
            return characterPool.GetPooledCharacters();
        }

        // Grid 전체에 케릭터 Object를 생성하는 메소드
        public void SpawnCharacters()
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
            return list.FirstOrDefault(character => character.activeInHierarchy && character.transform.position == position);
        }

        // 비어있는 Grid 위에 Character를 이동 시키는 메소드
        public void MoveCharactersEmptyGrid(Vector2 emptyGridPosition)
        {
            foreach (var character in characterPool.GetPooledCharacters())
            {
                if (character.transform.position.x != emptyGridPosition.x ||
                    !(character.transform.position.y < emptyGridPosition.y)) continue;
                Vector2 newPosition = character.transform.position;
                newPosition.y += 1;
                character.transform.position = newPosition;
            }
            RespawnCharacter();
        }

        // Pool에 활성화 되지 않은 CharacterObject를 확인하고 호출하는 메소드
        private void RespawnCharacter()
        {
            var inactiveCharacters = characterPool.GetPooledCharacters().Where(character => !character.activeInHierarchy).ToList();
            var freeXPositions = gridManager.GetFreeXPositions();
            var index = 0;
            for (; index < freeXPositions.Count; index++)
            {
                var t = freeXPositions[index];
                var randomCharacterIndex = Random.Range((float)0, inactiveCharacters.Count);
                var character = inactiveCharacters[(int)randomCharacterIndex];
                character.transform.position = new Vector3(t, 0, 0);
                character.SetActive(true);
                inactiveCharacters.RemoveAt((int)randomCharacterIndex);
            }
        }
    }
}