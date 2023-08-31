using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script.PuzzleManagerGroup
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private CommonRewardManager rewardManger;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private EnforceManager enforceManager;
        [SerializeField] private SwipeManager swipeManager;
        public bool isWave10Spawning;
        private float _totalPos;
        private int _currentGroupIndex;
        public bool isTutorial;
        private static readonly object Lock = new object();
        public List<GameObject> movedObjects = new List<GameObject>();
        private bool _gameStart;
        private int _count = 0;  
        private void Start()
        {
            if (!PlayerPrefs.HasKey("TutorialKey")) return;
            if (PlayerPrefs.GetInt("TutorialKey") == 1)
            {
                isTutorial = true;
            }

            _gameStart = true;
        }
        public static GameObject CharacterObject(Vector3 spawnPosition)
        {
            var spawnCharacters = CharacterPool.Instance.UsePoolCharacterList();
            return (from character in spawnCharacters
                where character.transform.position == spawnPosition
                select character.gameObject).FirstOrDefault();
        }
        public event Action OnMatchFound;
        private void TriggerOnMatchFound()
        {
            OnMatchFound?.Invoke();
        }
        public List<(GameObject, Vector3Int)> CalculateMoves()
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
            return moves;
        }
        public IEnumerator PerformMoves(IEnumerable<(GameObject, Vector3Int)> moves)
        {
            var moveCoroutines
                    = moves.Select(move => StartCoroutine(MoveCharacter(move.Item1, move.Item2))).ToList();
            foreach (var moveCoroutine in moveCoroutines)
            {
                yield return moveCoroutine;
            }
        }
        private static IEnumerator MoveCharacter(GameObject gameObject, Vector3 targetPosition, float duration = 0.35f)
        {
            if (gameObject == null || !gameObject.activeInHierarchy) 
            {
                yield break;
            }
            var startPosition = gameObject.transform.position;
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                var percentage = elapsedTime / duration;
                gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, percentage); 
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            gameObject.transform.position = targetPosition;
            yield return null;
        }
        public IEnumerator SpawnAndMoveNewCharacters()
        {
            var moveCoroutines = new List<Coroutine>();
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
                    var spawnPosition = new Vector3Int(currentPosition.x, -2 - emptyCellCount, 0);
                    if (CharacterObject(spawnPosition) != null) continue;
                    var newCharacter = SpawnNewCharacter(spawnPosition);
                    if (newCharacter == null) continue;
                    var coroutine = StartCoroutine(MoveCharacter(newCharacter, currentPosition));
                    moveCoroutines.Add(coroutine);
                    movedObjects.Add(newCharacter);
                }
            }
            foreach (var coroutine in moveCoroutines)
            {
                yield return coroutine;
            }
            yield return null;
        }
        private GameObject SpawnNewCharacter(Vector3Int position)
        {
            return isTutorial ? SpawnTutorialCharacter(position) : SpawnMainGameCharacter(position);
        }
        private static GameObject SpawnMainGameCharacter(Vector3Int position)
        {
            var notUsePoolCharacterList = CharacterPool.Instance.NotUsePoolCharacterList();
            var randomIndex = Random.Range(0, notUsePoolCharacterList.Count);
            var newCharacter = notUsePoolCharacterList[randomIndex];
            newCharacter.transform.position = position;
            if (EnforceManager.Instance.index.Count > 0)
            {
                foreach (var dummy in EnforceManager.Instance.index
                             .Select(index => EnforceManager.Instance.characterList[index].unitGroup)
                             .Where(unitGroup => unitGroup == newCharacter.GetComponent<CharacterBase>().unitGroup))
                {
                    newCharacter.GetComponent<CharacterBase>().SetLevel(2);
                }
            }
            else
            {
                newCharacter.GetComponent<CharacterBase>().Initialize();
            }
            newCharacter.SetActive(true);
            notUsePoolCharacterList.RemoveAt(randomIndex);
            return newCharacter;
        }
        public IEnumerator PositionUpCharacterObject()
        {
            Debug.Log("1");
            var moves = CalculateMoves();
            Debug.Log("2");
            if (moves.Count > 0)
            {
                movedObjects.AddRange(moves.Select(move => move.Item1).ToList());
                yield return StartCoroutine(PerformMoves(moves));
            }
            Debug.Log("3");
            yield return StartCoroutine(SpawnAndMoveNewCharacters());
            
            Debug.Log("4");
            if (_gameStart)
            {
                Debug.Log("5");
                var spawnCharacters = CharacterPool.Instance.UsePoolCharacterList();
                Debug.Log("6");
                foreach (var unit in spawnCharacters)
                {
                    Debug.Log("7");
                    matchManager.IsMatched(unit);
                }
                Debug.Log("8");
                var movesFirst = CalculateMoves();
                Debug.Log("9");
                yield return StartCoroutine(PerformMoves(movesFirst));
                Debug.Log("10");
                yield return StartCoroutine(SpawnAndMoveNewCharacters());
                _gameStart = false;
            }
            
            Debug.Log("11");
            yield return StartCoroutine(CheckPosition());
            
            Debug.Log("12");
            foreach (var movedObject in movedObjects.Where(movedObject => movedObject != null))
            {
                Debug.Log("13");
                movedObject.transform.position = movedObject.transform.position;
            }
            Debug.Log("14");
            var isMatched = movedObjects.Where(movedObject => movedObject != null).Any(movedObject => matchManager.IsMatched(movedObject.gameObject));
            Debug.Log("15");
            movedObjects.Clear();

            if (isMatched)
            {Debug.Log("16");
                _count++;
                if (_count > 1)
                {
                    Debug.Log("17");
                    countManager.IncrementComboCount();
                }
                Debug.Log("18");
                yield return new WaitForSecondsRealtime(0.3f);
                Debug.Log("19");
                yield return StartCoroutine(PositionUpCharacterObject());
            }
            else
            {
                Debug.Log("20");
                _count = 0;
                if (rewardManger.PendingTreasure.Count != 0)
                {
                    yield return StartCoroutine(rewardManger.EnqueueTreasure());
                }
                if (isTutorial)
                {
                    TriggerOnMatchFound();
                }
                Debug.Log("21");
                swipeManager.isBusy = false;
            }

            if (countManager.TotalMoveCount > 0 || CommonRewardManager.Instance.isOpenBox) yield break;
            Debug.Log("22");
            yield return StartCoroutine(GameManager.Instance.Count0Call());
            yield return null;
        }
        public IEnumerator CheckPosition()
        {
            var totalUnitCount = gridManager.gridHeight * gridManager.gridWidth;
            lock (Lock)
            {
                while (true)
                {
                    var units = CharacterPool.Instance.UsePoolCharacterList();
                    if (units.Count == totalUnitCount)
                    {
                        var allUnitsAtIntegerPositions = units.All(unit =>
                        {
                            var pos = unit.transform.position;
                            return Mathf.Approximately(pos.x, (int)pos.x) && Mathf.Approximately(pos.y, (int)pos.y);
                        });

                        if (allUnitsAtIntegerPositions)
                        {
                            break;
                        }
                    }
                    yield return null;
                }
            }
        }
        private GameObject SpawnTutorialCharacter(Vector3Int position)
        {
            GameObject newCharacter = null;
            var attempts = 0;

            while (newCharacter == null && attempts < TutorialManager.UnitGroupOrder.Length) 
            {
                var nextUnitGroupKey = TutorialManager.UnitGroupOrder[_currentGroupIndex];
                _currentGroupIndex = (_currentGroupIndex + 1) % TutorialManager.UnitGroupOrder.Length; // Always increment the index

                var notUsePoolCharacterList = CharacterPool.Instance.NotUsePoolCharacterList()
                    .Where(character => character.GetComponent<CharacterBase>().unitGroup == nextUnitGroupKey && !character.activeInHierarchy)
                    .ToList();
                if (notUsePoolCharacterList.Count > 0) {
                    var randomIndex = Random.Range(0, notUsePoolCharacterList.Count);
                    newCharacter = notUsePoolCharacterList[randomIndex];
                }
                attempts++;
            }
            if (newCharacter == null) return newCharacter;
            newCharacter.transform.position = position;
            newCharacter.GetComponent<CharacterBase>().Initialize();
            newCharacter.SetActive(true);
            return newCharacter;
        }
        private IEnumerator PerformMovesSequentially(List<(GameObject, Vector3Int)> moves)
        {
            foreach (var (o, targetPosition) in moves)
            {
                yield return StartCoroutine(MoveCharacter(o, targetPosition));
            }
        }
        public static void SaveUnitState()
        {
            if (PlayerPrefs.HasKey("unitState")) 
            {
                PlayerPrefs.DeleteKey("unitState");
            }
          
            var poolCharacterList = CharacterPool.Instance.UsePoolCharacterList();
            var unitState = "";
            foreach (var character in poolCharacterList)
            {
                var characterBase = character.GetComponent<CharacterBase>();
                var group = characterBase.unitGroup.ToString();
                var level = characterBase.unitPuzzleLevel;
                var position = Vector3Int.FloorToInt(character.transform.position);
                unitState += group + "|" + level + "|" + position.x + "," + position.y + "," + position.z + ";";
            }
            PlayerPrefs.SetString("unitState", unitState);
            PlayerPrefs.Save();
        }
        public static IEnumerator LoadGameState()
        {
            CharacterPool.Instance.theFirst = false;
            var useList = CharacterPool.Instance.UsePoolCharacterList();
            foreach (var useUnit in useList)
            {
                CharacterPool.ReturnToPool(useUnit);
            }

            var unitState = PlayerPrefs.GetString("unitState", "");
            var pieceData = unitState.Split(';');
            var notUsePoolCharacterList = CharacterPool.Instance.NotUsePoolCharacterList();
            foreach (var data in pieceData)
            {
                if (string.IsNullOrEmpty(data)) continue;
                var unit = data.Split('|');
                if (unit.Length < 3) continue;
                var group = unit[0];
                var unitGroups = CharacterBase.UnitGroups.None;
                if (Enum.TryParse(group, true, out CharacterBase.UnitGroups parsedGroup)) unitGroups = parsedGroup;
                var level = unit[1];
                var unitLevel = int.Parse(level);
                var position = Vector3Int.zero;
                var positionValue = unit[2].Split(',');
                if (positionValue.Length < 3) continue;
                position.x = int.Parse(positionValue[0]);
                position.y = int.Parse(positionValue[1]);
                position.z = int.Parse(positionValue[2]);
                var setUnit = notUsePoolCharacterList.FirstOrDefault(t => t.GetComponent<CharacterBase>().unitGroup == unitGroups && t.activeSelf == false);
                if (setUnit == null) continue;
                var setUnitBase = setUnit.GetComponent<CharacterBase>();
                setUnitBase.Initialize();
                setUnitBase.unitGroup = unitGroups;
                setUnitBase.unitPuzzleLevel = unitLevel;
                setUnitBase.GetComponent<SpriteRenderer>().sprite = setUnitBase.GetSprite(unitLevel);
                setUnitBase.transform.position = position;
                setUnitBase.gameObject.SetActive(true);
            }
            yield return null;
        }
        public IEnumerator BossStageClearRule()
        {
            isWave10Spawning = true;
            swipeManager.isBusy = true;
            yield return StartCoroutine(gameManager.WaitForPanelToClose());
            foreach (var unit in CharacterPool.Instance.pooledCharacters)
            {
                unit.GetComponent<CharacterBase>().cover.SetActive(false);
            }
            var saveCharacterList = CharacterPool.Instance.UsePoolCharacterList();
            var highLevelCharacters = saveCharacterList
                .OrderByDescending(character => character.GetComponent<CharacterBase>().unitPuzzleLevel)
                .Take(enforceManager.highLevelCharacterCount)
                .ToList();
            foreach (var character in saveCharacterList
                         .Where(character => !highLevelCharacters.Contains(character)))
            {
                CharacterPool.ReturnToPool(character);
            }
            var highestY = gridManager.gridHeight - 1;
            var moves = new List<(GameObject, Vector3Int)>();
            var currentColumn = 0;
            var currentRow = highestY;
            foreach (var character in highLevelCharacters)
            {
                var newGridPosition = new Vector3Int(currentColumn, currentRow, 0);
                moves.Add((character, newGridPosition));

                currentColumn++;
                if (currentColumn < gridManager.gridWidth) continue;
                currentColumn = 0; // Reset column
                currentRow--; // Move to the next row
            }
            yield return StartCoroutine(PerformMovesSequentially(moves));
            moves.Clear();
            for (var y = currentRow; y >= 0; y--) // Start from the current row
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
            }
            yield return StartCoroutine(PositionUpCharacterObject());
             isWave10Spawning = false;
            swipeManager.isBusy = false;
        }
    }
}
