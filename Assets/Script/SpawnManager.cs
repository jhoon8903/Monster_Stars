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
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private SwipeManager swipeManager;

        /**
         * Using Character Object Find
         */
        public GameObject CharacterObject(Vector3 spawnPosition)
        {
            var spawnCharacters = characterPool.UsePoolCharacterList();
            return (from character in spawnCharacters 
                where character.transform.position == spawnPosition 
                select character.gameObject).FirstOrDefault();
        }

        /**
         * If Match Or Disappear Object => Refill under Object
         */
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
                      else if (emptyCellCount > 0)
                      {
                          var targetPosition = new Vector3Int(x, y + emptyCellCount, 0);
                          moves.Add((currentObject, targetPosition));
                      }
                  }
              }
              // PerformMoves를 시작하고 PositionUpCharacterObject 코루틴을 일시 중지하고 PerformMoves가 완료될 때까지 기다립니다.
              yield return StartCoroutine(PerformMoves(moves));
              // PerformMoves가 완료된 후 PositionUpCharacterObject가 특정 지연을 기다리며 계속됩니다.
              // SpawnAndMoveNewCharacters를 시작하고 PositionUpCharacterObject 코루틴을 일시 중지하고
              // SpawnAndMoveNewCharacters가 완료될 때까지 기다립니다.
              yield return StartCoroutine(SpawnAndMoveNewCharacters());
              // SpawnAndMoveNewCharacters가 완료된 후 PositionUpCharacterObject가 특정 지연을 기다리며 계속됩니다. 
              // CheckMatchesAndMoveCharacters를 시작하고 PositionUpCharacterObject 코루틴을 일시 중지하고
              // CheckMatchesAndMoveCharacters가 완료될 때까지 기다립니다.
              yield return StartCoroutine(matchManager.CheckMatchesAndMoveCharacters());
        }

        /**
         * New Spawn characterObject Move to Grid 
         */
        private IEnumerator SpawnAndMoveNewCharacters() 
         {
             var moves = new List<(GameObject, Vector3Int)>();
             var newCharacters = new List<GameObject>();
             for (var x = 0; x < gridManager.gridWidth; x++)
             {
                 var emptyCellCount = 0;
                 for (var y = gridManager.gridHeight - 1; y >= 0; y--)
                 {
                     var currentPosition = new Vector3Int(x, y, 0);
                     var currentObject = CharacterObject(currentPosition);
                     if (currentObject != null) continue;
                     emptyCellCount++;
                     var newCharacter = SpawnNewCharacter(currentPosition, emptyCellCount);
                     if (newCharacter == null) continue;
                     newCharacters.Add(newCharacter);
                     moves.Add((newCharacter, currentPosition));
                 }
             }
             yield return StartCoroutine(NewPerformMoves(moves));
         }

        /**
         * New character Object Receive Character Pool
         */
        private GameObject SpawnNewCharacter(Vector3Int position, int yOffset)
        {
            var notUsePoolCharacterList = characterPool.NotUsePoolCharacterList();
            if (notUsePoolCharacterList.Count <= 0) return null;
            var randomIndex = Random.Range(0, notUsePoolCharacterList.Count);
            var newCharacter = notUsePoolCharacterList[randomIndex];
            newCharacter.transform.position = new Vector3Int(position.x,-yOffset - gridManager.gridHeight, position.z);
            newCharacter.SetActive(true);
            notUsePoolCharacterList.RemoveAt(randomIndex);
            return newCharacter;
        }

        /**
         * Move New Character List<moves> NewPerformMove
         */
        private IEnumerator NewPerformMoves(IEnumerable<(GameObject, Vector3Int)> moves)
        {
            var coroutines = moves
                .Select(move => StartCoroutine(swipeManager.NewCharacterMove(move.Item1, move.Item2)))
                .ToList();
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }
        }

        /**
         * Move Character List<moves> PerformMove
         */
        private IEnumerator PerformMoves(IEnumerable<(GameObject, Vector3Int)> moves)
        {
            var coroutines = moves
                .Select(move => StartCoroutine(SwipeManager.OneWayMove(move.Item1, move.Item2)))
                .ToList();
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }
        }
    }
}