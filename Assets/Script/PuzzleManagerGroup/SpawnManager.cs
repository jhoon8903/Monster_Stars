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
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private TutorialManager tutorialManager;
        public bool isWave10Spawning;
        private float _totalPos;
        private int _currentGroupIndex;
        public bool isTutorial;
        private static readonly object Lock = new object();
        private void Start()
        {
            if (!PlayerPrefs.HasKey("TutorialKey")) return;
            if (PlayerPrefs.GetInt("TutorialKey") == 1)
            {
                isTutorial = true;
            }
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
            yield return StartCoroutine(CheckPosition());
            if (rewardManger.PendingTreasure.Count != 0)
            {
                rewardManger.EnqueueTreasure();
            }
            if (isTutorial)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                TriggerOnMatchFound();
            }
            while (commonRewardManager.isOpenBox)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                yield return new WaitForSeconds(0.5f);
            }
            swipeManager.isBusy = false;
            if (countManager.TotalMoveCount == 0)
            {
                yield return StartCoroutine(gameManager.Count0Call());
            }
        }

        private IEnumerator CheckPosition()
        {
            var wait = new WaitForSeconds(0.3f);
            var maxRows = CharacterPool.Instance.UsePoolCharacterList().Count / 6;
            var maxCount = maxRows * (maxRows - 1) * 3;
            _totalPos = CharacterPool.Instance.UsePoolCharacterList().Sum(t=> t.transform.position.y);
            while (_totalPos < maxCount)
            {
                while(CharacterPool.Instance.SortPoolCharacterList().Count < CharacterPool.Instance.UsePoolCharacterList().Count)
                {
                    yield return wait;
                }
                _totalPos = CharacterPool.Instance.UsePoolCharacterList().Sum(t => t.transform.position.y);
            }
            yield return StartCoroutine(matchManager.CheckMatches());
        }
        private static IEnumerator MoveCharacter(GameObject gameObject, Vector3 targetPosition, float duration = 0.35f)
        {
            if (gameObject == null) yield break;

            Vector3 startPosition = gameObject.transform.position;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                float percentage = elapsedTime / duration;
                gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, percentage);
                elapsedTime += Time.deltaTime;
                yield return null; // 다음 프레임까지 기다립니다.
            }

            gameObject.transform.position = targetPosition; // 마지막으로 정확한 목표 위치로 설정합니다.
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
                    var newCharacter = SpawnNewCharacter(spawnPosition);
                    if (newCharacter == null) continue;
                   
                    var coroutine = StartCoroutine(MoveCharacter(newCharacter, currentPosition));
                    moveCoroutines.Add(coroutine);
                }
            }

            foreach (var coroutine in moveCoroutines)
            {
                yield return coroutine;
            }
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
            yield return StartCoroutine(gameManager.WaitForPanelToClose());
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
                moves.Clear();
            }
            yield return StartCoroutine(matchManager.CheckMatches());
            isWave10Spawning = false;
        }
    }
}
