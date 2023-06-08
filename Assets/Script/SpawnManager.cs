using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.RewardScript;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private CharacterPool characterPool;  
        [SerializeField] private GridManager gridManager; 
        [SerializeField] private MatchManager matchManager;  
        [SerializeField] private CommonRewardManager rewardManger;
        [SerializeField] private GameManager gameManager;
        public bool isWave10Spawning = false;
        private int highLevelCharacterCount = 6;

        public bool isMatched = false;
        public GameObject CharacterObject(Vector3 spawnPosition)
        {
            var spawnCharacters = characterPool.UsePoolCharacterList();
            return (from character in spawnCharacters 
                where character.transform.position == spawnPosition 
                select character.gameObject).FirstOrDefault();
        }
        public IEnumerator PositionUpCharacterObject()
        {
            //if (isMatched) yield break;
            //isMatched = true;
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

            //isMatched = false;

            if (rewardManger.PendingTreasure.Count == 0) yield break;
            rewardManger.EnqueueTreasure(rewardManger.PendingTreasure.Dequeue());
        }
        
        private static IEnumerator MoveCharacter(GameObject gameObject, Vector3Int targetPosition, float duration = 0.3f)
        {
            if (gameObject == null) yield break;

            var moveTween 
                = gameObject.transform.DOMove(targetPosition, duration).SetEase(Ease.Linear);
    
            yield return moveTween.WaitForCompletion();
        }
        private IEnumerator PerformMoves(IEnumerable<(GameObject, Vector3Int)> moves)
        {
            var moveCoroutines
                = moves.Select(move 
                    => StartCoroutine(MoveCharacter(move.Item1, move.Item2)))
                    .ToList();
            foreach (var moveCoroutine in moveCoroutines)
            {
                yield return moveCoroutine;
            }
        }
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
                    var coroutine = StartCoroutine(MoveCharacter(newCharacter, currentPosition));
                    moveCoroutines.Add(coroutine);
                }
            }
            foreach(var coroutine in moveCoroutines)
            {
                yield return coroutine;
            }
        }
        private GameObject SpawnNewCharacter(Vector3Int position)
        {
            var notUsePoolCharacterList = characterPool.NotUsePoolCharacterList();
            if (notUsePoolCharacterList.Count <= 0) return null;
            var randomIndex = Random.Range(0, notUsePoolCharacterList.Count);
            var newCharacter = notUsePoolCharacterList[randomIndex];
            newCharacter.transform.position = position;
            var characterBase = newCharacter.GetComponent<CharacterBase>();
            if (characterBase.PermanentLevelUp && characterBase.UnitLevel == 1)
            {
                characterBase.LevelUpScale(newCharacter);
            }
            newCharacter.SetActive(true);
            notUsePoolCharacterList.RemoveAt(randomIndex);
            return newCharacter;
        }
        public IEnumerator Wave10Spawn()
        {
            isWave10Spawning = true;
            
            var saveCharacterList = characterPool.UsePoolCharacterList();
            var highLevelCharacters = saveCharacterList
                .OrderByDescending(character => character.GetComponent<CharacterBase>().UnitLevel)
                .Take(highLevelCharacterCount)
                .ToList();
            yield return StartCoroutine(gameManager.WaitForPanelToClose());
            foreach (var character in saveCharacterList
                         .Where(character => !highLevelCharacters.Contains(character)))
            {
                character.GetComponent<CharacterBase>().CharacterReset();
                character.SetActive(false);
            }

            var highestY = gridManager.gridHeight - 1;
            var moves = new List<(GameObject, Vector3Int)>();

            for (var x = 0; x < highLevelCharacters.Count; x++)
            {
                var newGridPosition = new Vector3Int(x, highestY, 0);
                moves.Add((highLevelCharacters[x], newGridPosition));
            }

            yield return StartCoroutine(PerformMovesSequentially(moves));
            moves.Clear();
            for (var y = highestY - 1; y >= 0; y--)
            {
                for (var x = 0; x < gridManager.gridWidth; x++)
                {
                    var currentPos = new Vector3Int(x, y, 0);
                    if (CharacterObject(currentPos) != null) continue; // Skip if the cell is not empty
                    var spawnPosition = new Vector3Int(x, -2, 0); // Spawn position below the grid
                    var newCharacter = SpawnNewCharacter(spawnPosition);
                    if (newCharacter != null)
                    {
                        moves.Add((newCharacter, currentPos));
                    }
                }
                yield return StartCoroutine(PerformMoves(moves)); // Move all characters to the current row at once
                moves.Clear();
            } 
            yield return StartCoroutine(matchManager.CheckMatches());
            isWave10Spawning = false;
        }
        private IEnumerator PerformMovesSequentially(List<(GameObject, Vector3Int)> moves)
        {
            foreach (var (o, targetPosition) in moves)
            {
                yield return StartCoroutine(MoveCharacter(o, targetPosition));
            }
        }

        public void nextCharacterUpgrade(int moveCharacterCount)
        {
            highLevelCharacterCount += moveCharacterCount;
        }
    }
}
