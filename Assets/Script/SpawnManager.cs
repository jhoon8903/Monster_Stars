using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.PowerUpScript;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private CharacterPool characterPool;  // 캐릭터 풀
        [SerializeField] private GridManager gridManager;  // 그리드 매니저
        [SerializeField] private MatchManager matchManager;  // 매치 매니저
        [SerializeField] private TreasureManager treasureManger;

         // 이 함수는 인수로 주어진 위치에 있는 스폰 캐릭터 목록에서 캐릭터의 GameObject를 반환합니다.
        public GameObject CharacterObject(Vector3 spawnPosition)
        {
            var spawnCharacters = characterPool.UsePoolCharacterList();
            return (from character in spawnCharacters 
                where character.transform.position == spawnPosition 
                select character.gameObject).FirstOrDefault();
        }
        
        //  캐릭터 오브젝트를 상승시키는 작업을 수행하는 코루틴입니다.
        // 빈 그리드 공간을 기반으로 캐릭터의 이동을 계산하고, 이러한 이동을 수행하고, 새 캐릭터를 생성하고, 일치하는지 확인한 다음 완료 신호를 보냅니다.
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
            if (treasureManger.PendingTreasure.Count == 0) yield break;
            treasureManger.EnqueueAndCheckTreasure(treasureManger.PendingTreasure.Dequeue());
        }

        // 이 코루틴은 새 캐릭터의 생성 및 후속 이동을 관리합니다.
        // 새 캐릭터를 생성해야 하는 위치(빈 그리드 공간)를 결정하고 해당 위치에 새 캐릭터를 생성한 다음 그리드의 올바른 위치로 이동합니다.
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

        // 이 함수는 지정된 그리드 위치에서 새 캐릭터를 생성합니다.
        // 사용 가능한(사용되지 않은) 문자 목록에서 임의로 문자를 선택하여 지정된 위치에 배치합니다.
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
        
        // 이 코루틴은 새로 생성된 캐릭터를 그리드의 대상 위치로 이동하는 역할을 합니다.
        private IEnumerator MoveNewCharacter(GameObject newCharacter, Vector3Int targetPosition)
        {
            yield return StartCoroutine(SwipeManager.NewCharacterMove(newCharacter, targetPosition));

        }

        // 이 코루틴은 캐릭터가 움직일 때 자연스러운 효과를 적용합니다.
        // 이동 목록의 각 이동에 대해 게임 오브젝트를 그리드의 특정 위치로 이동하는 코루틴을 시작합니다.
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
