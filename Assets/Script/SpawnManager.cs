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
        // CharacterPool에서 사용 가능한 Pool 객체를 반환
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
        public GameObject SpawnCharacterAtPosition(int x, int y)
        {
            var spawnPosition = new Vector2(x, y);

            if (IsCharacterAtPosition(spawnPosition)) return null;
            var pooledCharacter = characterPool.GetPooledCharacter();

            if (pooledCharacter == null) return null;
            pooledCharacter.transform.position = spawnPosition;
            pooledCharacter.SetActive(true);

            return pooledCharacter;
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
        public IEnumerator MoveCharactersEmptyGrid(Vector2 emptyGridPosition)
        {
            Tween lastTween = null;
            foreach (var character in characterPool.GetPooledCharacters())
            {
                if (character.transform.position.x != emptyGridPosition.x ||
                    !(character.transform.position.y < emptyGridPosition.y)) continue;
                Vector2 newPosition = character.transform.position;
                newPosition.y += 1;
                lastTween = character.transform.DOMove(newPosition, 0.4f);
            }
            // Wait for the last character to reach its new position before spawning new characters.
            if (lastTween != null)
            {
                yield return lastTween.WaitForCompletion();
            }
            yield return StartCoroutine(RespawnCharacter((int)emptyGridPosition.x));
        }

        private IEnumerator RespawnCharacter(int column)
        {
            var inactiveCharacters = characterPool.GetPooledCharacters().Where(character => !character.activeInHierarchy).ToList();
            var freeYPositions = Enumerable.Range(0, gridManager.gridHeight)
                .Select(y => new Vector2(column, y))
                .Where(pos => characterPool.GetPooledCharacters().All(character => new Vector2(character.transform.position.x, character.transform.position.y) != pos))

                .ToList();
            foreach (var position in freeYPositions)
            {
                if (inactiveCharacters.Count == 0) break;
                var randomCharacterIndex = Random.Range(0, inactiveCharacters.Count);
                var character = inactiveCharacters[randomCharacterIndex];
                character.transform.position = position;
                character.SetActive(true);
                inactiveCharacters.RemoveAt(randomCharacterIndex);
                // Wait for the character to "fall" into position before continuing to the next one.
                yield return character.transform.DOMove(position, 0.2f).WaitForCompletion();
            }
        }
    }
}