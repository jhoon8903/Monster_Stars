using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.PuzzleManagerGroup
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private CommonRewardManager rewardManger;
        [SerializeField] private CountManager countManager;
        [SerializeField] private EnforceManager enforceManager;
        [SerializeField] private SwipeManager swipeManager;
        private float _totalPos;
        private int _currentGroupIndex;
        public bool isTutorial;
        private static readonly object Lock = new object();
        public List<GameObject> movedObjects = new List<GameObject>();
        public int count;
        private readonly Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();
        public bool bossClearRule;
        private void Start()
        {
            if (bool.Parse(PlayerPrefs.GetString("TutorialKey", "true")))
            {
                isTutorial = true;
            }
        }
        public event Action OnMatchFound;
        private void TriggerOnMatchFound()
        {
            OnMatchFound?.Invoke();
        }
        public void AddToQueue(IEnumerator coroutine)
        {
            _coroutineQueue.Enqueue(coroutine);
            if (_coroutineQueue.Count == 1)
            {
                StartCoroutine(ProcessQueue());
            }
        }
        private IEnumerator ProcessQueue()
        {
            while (_coroutineQueue.Count > 0)
            {
                yield return StartCoroutine(_coroutineQueue.Dequeue());
            }
        }
        public static GameObject CharacterObject(Vector3 spawnPosition)
        {
            var spawnCharacters = CharacterPool.Instance.UsePoolCharacterList();
            return (from character in spawnCharacters
                where character.transform.position == spawnPosition
                select character.gameObject).FirstOrDefault();
        }
        private List<(GameObject, Vector3Int)> CalculateMoves()
        {
            lock (Lock)
            {
                swipeManager.isBusy = true;
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
                        else
                        {
                            movedObjects.Add(currentObject);
                        }
                        if (emptyCellCount <= 0) continue;
                        var targetPosition = new Vector3Int(x, y + emptyCellCount, 0);
                        moves.Add((currentObject, targetPosition));
                    }
                }
                return moves;
            }
        }
        private IEnumerator PerformMoves(IEnumerable<(GameObject, Vector3Int)> moves)
        {
            swipeManager.isBusy = true;
            lock (Lock)
            {
                var moveCoroutines
                    = moves.Select(move => StartCoroutine(MoveCharacter(move.Item1, move.Item2))).ToList();
                foreach (var moveCoroutine in moveCoroutines)
                {
                    yield return moveCoroutine;
                }
            }
        }
        private static IEnumerator MoveCharacter(GameObject gameObject, Vector3 targetPosition, float duration = 0.25f)
        {
            lock (Lock)
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
        }
        private IEnumerator SpawnAndMoveNewCharacters()
        {
            swipeManager.isBusy = true;
            lock (Lock)
            {
                yield return new WaitForSecondsRealtime(0.14f);
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
                        var coroutine = StartCoroutine(MoveCharacter(newCharacter, currentPosition, 0.35f));
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
        }
        private IEnumerator CheckPosition()
        {
            swipeManager.isBusy = true;
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
        private GameObject SpawnNewCharacter(Vector3Int position)
        {
            return isTutorial ? SpawnTutorialCharacter(position) : SpawnMainGameCharacter(position);
        }
        private GameObject SpawnMainGameCharacter(Vector3Int position)
        {
            var notUsePoolCharacterList = CharacterPool.Instance.NotUsePoolCharacterList();
            var randomIndex = Random.Range(0, notUsePoolCharacterList.Count);
            var newCharacter = notUsePoolCharacterList[randomIndex];
            newCharacter.transform.localScale = Vector3.one;
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
            movedObjects.Add(newCharacter);
            notUsePoolCharacterList.RemoveAt(randomIndex);
            return newCharacter;
        }
        public IEnumerator PositionUpCharacterObject()
        {
            movedObjects.Clear();
            lock (Lock)
            {
                yield return new WaitForSecondsRealtime(0.52f);
                var moves = CalculateMoves();
                if (moves.Count > 0)
                {
                    movedObjects.AddRange(moves.Select(move => move.Item1).ToList());
                }
                yield return StartCoroutine(PerformMoves(moves));
                yield return StartCoroutine(SpawnAndMoveNewCharacters());
                yield return StartCoroutine(CheckPosition());
                foreach (var movedObject in movedObjects.Where(movedObject => movedObject != null))
                {
                    movedObject.transform.position = movedObject.transform.position;
                }
                var isMatched = movedObjects.Where(movedObject => movedObject != null).ToList().Any(movedObject => matchManager.IsMatched(movedObject.gameObject));

                if (isMatched)
                {
                    yield return StartCoroutine(PositionUpCharacterObject());
                }
                else
                {
                    count = 0;
                    if (rewardManger.PendingTreasure.Count != 0)
                    {
                        yield return StartCoroutine(rewardManger.EnqueueTreasure());
                    }
                    if (isTutorial)
                    {
                        TriggerOnMatchFound();
                    }
                    swipeManager.isBusy = false;
                }

                if (countManager.TotalMoveCount <= 0 && !CommonRewardManager.Instance.isOpenBox)
                {
                    yield return new WaitForSecondsRealtime(1f);
                    if (countManager.TotalMoveCount <= 0 && !CommonRewardManager.Instance.isOpenBox)
                    {
                        yield return StartCoroutine(GameManager.Instance.Count0Call());
                    }
                }
            }

            if (GameManager.Instance.isInitWave)
            {
                GameManager.Instance.isInitWave = false;
                SaveUnitState();
                PlayerPrefs.SetInt("moveCount", countManager.TotalMoveCount);
                Debug.Log("Init Save");
            }
            yield return null;
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
                if (notUsePoolCharacterList.Count > 0) 
                {
                    var randomIndex = Random.Range(0, notUsePoolCharacterList.Count);
                    newCharacter = notUsePoolCharacterList[randomIndex];
                }
                attempts++;
            }
            if (newCharacter == null) return newCharacter;
            newCharacter.transform.position = position;

            newCharacter.GetComponent<CharacterBase>().Initialize();
            newCharacter.SetActive(true);
            newCharacter.transform.localScale = Vector3.one;
            movedObjects.Add(newCharacter);
            return newCharacter;
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
            Debug.Log($"{StageManager.Instance.latestStage}Stage / {StageManager.Instance.currentWave}Wave Save");
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
        public void BossStageClearRule()
        {
            Debug.Log("Call Boss Clear");
            if (bossClearRule) return;
            bossClearRule = true;
            Debug.Log("Boss Clear Activate");
            lock (Lock)
            {
                if (gridManager.addRowActivate) return;
                gridManager.addRowActivate = true;
                swipeManager.isBusy = true;
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
                    currentColumn = 0;
                    currentRow--;
                }
                StartCoroutine(PerformMoves(moves));
                moves.Clear();
                SaveUnitState();
                swipeManager.isBusy = false;
                bossClearRule = false;
            }
        }
    }
}
