using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private CharacterPool characterPool;  // 캐릭터 풀
        [SerializeField] private GridManager gridManager;  // 그리드 매니저
        [SerializeField] private MatchManager matchManager;  // 매치 매니저


         // 스폰 위치에 있는 캐릭터 객체를 가져옴
        public GameObject CharacterObject(Vector3 spawnPosition)
        {
            var spawnCharacters = characterPool.UsePoolCharacterList();
            return (from character in spawnCharacters 
                where character.transform.position == spawnPosition 
                select character.gameObject).FirstOrDefault();
        }

        // 캐릭터 객체들을 상승시키는 동작 수행
        public IEnumerator PositionUpCharacterObject()
        {
            var moves = new List<(GameObject, Vector3Int)>();
            for (var x = 0; x < gridManager.gridWidth; x++) 
            { 
                var emptyCellCount = 0;
                for (var y = gridManager.gridHeight - 1; y >= 0; y--) 
                {
                    var currentPosition = new Vector3Int(x, y, 0);
                    var currentObject = CharacterObject(currentPosition);
                    if (currentObject == null)
                    {
                        emptyCellCount++;
                    }
                    if (emptyCellCount <= 0) continue;
                    var targetPosition = new Vector3Int(x, y + emptyCellCount, 0);
                    moves.Add((currentObject, targetPosition));
                }
            }
            yield return StartCoroutine(PerformMoves(moves));
            yield return StartCoroutine(SpawnAndMoveNewCharacters());
            yield return StartCoroutine(matchManager.CheckMatches());
        }

        // 지정된 위치에 새로운 캐릭터를 스폰함
        private GameObject SpawnNewCharacter(Vector3Int position)
        {
            var notUsePoolCharacterList = characterPool.NotUsePoolCharacterList();
            if (notUsePoolCharacterList.Count <= 0) return null;
            var randomIndex = Random.Range(0, notUsePoolCharacterList.Count);
            var newCharacter = notUsePoolCharacterList[randomIndex];
            newCharacter.transform.position = position;
            newCharacter.SetActive(true);
            notUsePoolCharacterList.RemoveAt(randomIndex);
            return newCharacter;
        }

        // 새로운 캐릭터를 대상 위치로 이동시킴
        private IEnumerator MoveNewCharacter(GameObject newCharacter, Vector3Int targetPosition)
        {
            yield return StartCoroutine(SwipeManager.NewCharacterMove(newCharacter, targetPosition));
        }

        // 새로운 캐릭터를 스폰하고 이동시키는 동작 수행
        private IEnumerator SpawnAndMoveNewCharacters()
        {
            var moveCoroutines = new List<Coroutine>();
            var moves = new List<(GameObject, Vector3Int)>();
            if (moves == null) throw new ArgumentNullException(nameof(moves));
            for (var x = 0; x < gridManager.gridWidth; x++)
            {
                var emptyCellCount = 0;
                for (var y = gridManager.gridHeight - 1; y >= 0; y--)
                {
                    var currentPosition = new Vector3Int(x, y, 0);
                    var currentObject = CharacterObject(currentPosition);
                    if (currentObject == null)
                    {
                        emptyCellCount++;
                    }
                    if (emptyCellCount <= 0) continue;
                    var spawnPosition = new Vector3Int(currentPosition.x, -2-emptyCellCount, 0);
                    var newCharacter = SpawnNewCharacter(spawnPosition);
                    moves.Add((newCharacter, spawnPosition));
                    if (newCharacter == null) continue;
                    var coroutine = StartCoroutine(MoveNewCharacter(newCharacter, currentPosition));
                    moveCoroutines.Add(coroutine);
                }
            }
            foreach(var coroutine in moveCoroutines)
            {
                yield return coroutine;
            }
        }

        // 케릭터가 이동할때 자연스러운 효과를 적용시킴
        private IEnumerator PerformMoves(IEnumerable<(GameObject, Vector3Int)> moves)
        {
            var coroutines = moves.Select(move => StartCoroutine(SwipeManager.OneWayMove(move.Item1, move.Item2))).ToList();
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }
        }
    }
}
