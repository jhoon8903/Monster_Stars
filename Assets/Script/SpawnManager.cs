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
        [SerializeField] private float matchDelayTime = 1.5f;

        /**
         * CharacterObject is Spawning Character Spawn on Grid
         * give to Random Position to Character Object
         */
        public void SpawnCharacters()
        {
            var availablePositions = new List<Vector3Int>();

            for (var x = 0; x < gridManager.gridWidth; x++)
            {
                for (var y = 0; y < gridManager.gridHeight; y++)
                {
                    availablePositions.Add(new Vector3Int(x, y,0));
                }
            }
            
            var totalGridPositions = gridManager.gridWidth * gridManager.gridHeight;
            
            for (var i = 0; i < totalGridPositions; i++)
            {
                var randomPositionIndex = Random.Range(0, availablePositions.Count);
                var randomPosition = availablePositions[randomPositionIndex];
                availablePositions.RemoveAt(randomPositionIndex);
                var position = new Vector3(randomPosition.x, randomPosition.y);
                ActivateSpawn(position);
            }
        }
        
        /**
         * ActivateSpawn is Receive CharacterPool
         * and Status Change SetActive(true);
         */
        private void ActivateSpawn(Vector3 position)
        {
            var spawnCharacter = characterPool.AddRandomIndexPool();
            spawnCharacter.transform.position = position;
            spawnCharacter.SetActive(true);
        }

        public GameObject CharacterObject(Vector3 SpawnPosition)
        {
            var spawnCharacters = characterPool.UsePoolCharacterList();
            return (from character in spawnCharacters 
                where character.transform.position == SpawnPosition 
                select character.gameObject).FirstOrDefault();
        }

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
            yield return StartCoroutine(PerformMoves(moves));
            yield return StartCoroutine(SpawnAndMoveNewCharacters());
            yield return new WaitForSeconds(matchDelayTime);
            yield return StartCoroutine(matchManager.CheckMatchesAndMoveCharacters());
       }

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
             yield return StartCoroutine(PerformMoves(moves));
         }

        private GameObject SpawnNewCharacter(Vector3Int position, int yOffset)
        {
            var notUsePoolCharacterList = characterPool.NotUsePoolCharacterList();
            if (notUsePoolCharacterList.Count <= 0) return null;
            var randomIndex = Random.Range(0, notUsePoolCharacterList.Count);
            var newCharacter = notUsePoolCharacterList[randomIndex];
            newCharacter.transform.position = new Vector3Int(position.x, -yOffset, position.z);
            newCharacter.SetActive(true);
            notUsePoolCharacterList.RemoveAt(randomIndex);
            return newCharacter;
        }

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