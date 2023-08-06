using System.Collections;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;
using System.Collections.Generic;
using Script.RewardScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using TMPro;

namespace Script.PuzzleManagerGroup
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private GameObject tutorialCoverPrefab;
        [SerializeField] private GameObject guidePointerPrefab;
        [SerializeField] private TextMeshProUGUI textPopup;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private SwipeManager swipeManager;

        private readonly Dictionary<Vector2Int, GameObject> _covers = new Dictionary<Vector2Int, GameObject>();
        private Coroutine _pointerCoroutine;
        private GameObject _currentGuidePointer;
        private Queue<TutorialStep> _tutorialSteps;
        private TutorialStep _currentTutorialStep;
        private TextMeshProUGUI _currentTextPopup;

        private struct TutorialStep
        {
            public readonly Vector2Int[] PositionsToRemove;
            public readonly Vector3 PointerStartPosition;
            public readonly Vector3 PointerEndPosition;
            public readonly int TutorialStepCount;
            public readonly string PopupText;

            public TutorialStep(Vector2Int[] positionsToRemove, Vector3 pointerStartPosition, Vector3 pointerEndPosition, int tutorialStepCount, string popupText)
            {
                PositionsToRemove = positionsToRemove;
                PointerStartPosition = pointerStartPosition;
                PointerEndPosition = pointerEndPosition;
                TutorialStepCount = tutorialStepCount;
                PopupText = popupText;
            }
        }
        private void Start()
        {
            if (PlayerPrefs.GetInt("TutorialKey") != 1) return;
            matchManager.OnMatchFound += HandleMatchFound;
            _tutorialSteps = new Queue<TutorialStep>();
            commonRewardManager.OnRewardSelected += HandleMatchFound;
            
            // 3Matched
            _tutorialSteps.Enqueue(new TutorialStep(
                new[] { new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4), new Vector2Int(4, 4) },
                new Vector3(4, 4, 0),
                new Vector3(3, 4, 0),
                1,
                "Move units by swiping them")
            );
            // 4Matched
            _tutorialSteps.Enqueue(new TutorialStep(
                new []{new Vector2Int(1,4), new Vector2Int(2,4), new Vector2Int(3,4), new Vector2Int(4,4), new Vector2Int(3,5)},
                new Vector3(3,5,0), 
                new Vector3(3,4,0),
                2,
                "Swipe once more to combine units\nMerging 4 units will level up 2 units.")
            );
            // 5Matched
            _tutorialSteps.Enqueue(new TutorialStep(
                new[] { new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4), new Vector2Int(0,5), new Vector2Int(1, 3) },
                new Vector3(1, 3, 0),
                new Vector3(0, 3, 0),
                3,
                "Combining 5 makes a higher tier unit")
            );
            // Power Up Matched
            _tutorialSteps.Enqueue(new TutorialStep(
                new []{new Vector2Int(1,5), new Vector2Int(2,5), new Vector2Int(3,5), new Vector2Int(4,5)},
                new Vector3(4,5,0), 
                new Vector3(3,5,0),
                4,
                "By combining coins, you can obtain a reinforcement box.")
                
            );
            // Choose Power Up
            _tutorialSteps.Enqueue(new TutorialStep(
                new []{new Vector2Int(1,1), new Vector2Int(2,1), new Vector2Int(3,1), new Vector2Int(4,1)},
                new Vector3(3,2,0), 
                new Vector3(3,1.6f,0), 
                5,
                "Strengthen your units by pressing the Enhance Select button.")
            );
            // Press Long Object
            _tutorialSteps.Enqueue(new TutorialStep(
                new [] {new Vector2Int(5,3)},
                new Vector3(5,4,0), 
                new Vector3(5,3.4f, 0),
                6,
                "Units can be removed by holding down the unit for 2 seconds.")
            );
            // Null Swap
            _tutorialSteps.Enqueue(new TutorialStep(
                new []{new Vector2Int(5,4), new Vector2Int(6,4)},
                new Vector3(5,4,0), new Vector3(6,4,0),
                7,
                "You can remove units by swiping off the tile.")
            );
            ProcessTutorialStep(_tutorialSteps.Dequeue());
        }
        private void ProcessTutorialStep(TutorialStep step)
        {
            _currentTutorialStep = step;
            MatchCover(step.PositionsToRemove);
            var guidePointer = Instantiate(guidePointerPrefab, step.PointerStartPosition, Quaternion.identity);
            _pointerCoroutine = StartCoroutine(MoveGuidePointer(guidePointer, step.PointerStartPosition, step.PointerEndPosition, 2f, 0.5f));
            TextPopup(step.PopupText);
        }
        private void OnDestroy()
        {
            matchManager.OnMatchFound -= HandleMatchFound;
            commonRewardManager.OnRewardSelected -= HandleMatchFound;
        }
        private void HandleMatchFound()
        {
            if (_pointerCoroutine != null)
            {
                StopCoroutine(_pointerCoroutine);
                Destroy(_currentGuidePointer);
                _currentGuidePointer = null;
            }
            StartCoroutine(DelayedAction(ProcessNextTutorialStep));
        }

        public void EndTutorial()
        {
            foreach (var cover in _covers.Values)
            {
                Destroy(cover);
            }
            _covers.Clear();

            if (_currentGuidePointer != null)
            {
                Destroy(_currentGuidePointer);
                _currentGuidePointer = null;
            }
            Destroy(_currentTextPopup);
            PlayerPrefs.SetInt("TutorialKey", 0);
        }
        private void ProcessNextTutorialStep()
        {
            if (_tutorialSteps.Count > 0)
            {
                ProcessTutorialStep(_tutorialSteps.Dequeue());
            }
        }
        private static IEnumerator DelayedAction(System.Action callback)
        {
            yield return new WaitForSeconds(1f);
            Time.timeScale = 1f;// 1초 딜레이
            callback?.Invoke(); // 딜레이 후 액션 실행
        }
        public IEnumerator TutorialState()
        {
            const string tutorialPattern = "FNNBND\n" +
                                           "FEEBED\n" +
                                           "DBBFFN\n" +
                                           "FDEDND\n" +
                                           "FFENFB\n" +
                                           "DBDEBE";
            var rows = tutorialPattern.Split('\n');

            var notUsePoolCharacterList = characterPool.NotUsePoolCharacterList();

            for (var y = 0; y < rows.Length; y++)
            {
                var row = rows[y];
                for (var x = 0; x < row.Length; x++)
                {
                    var groupChar = row[x];
                    var unitGroups = ConvertToUnitGroup(groupChar);
                    var setUnit = notUsePoolCharacterList.FirstOrDefault(t => t.GetComponent<CharacterBase>().unitGroup == unitGroups && t.activeSelf == false);
                    if (setUnit == null) continue;
                    var setUnitBase = setUnit.GetComponent<CharacterBase>();
                    setUnitBase.Initialize();
                    setUnitBase.unitGroup = unitGroups;
                    setUnitBase.GetComponent<SpriteRenderer>().sprite = setUnitBase.GetSprite(1);
                    setUnitBase.transform.position = new Vector3Int(x, 5 - y, 0); 
                    setUnitBase.gameObject.SetActive(true);
                }
            }
            yield return null;
        }
        private static CharacterBase.UnitGroups ConvertToUnitGroup(char groupChar)
        {
            return groupChar switch
            {
                'F' => CharacterBase.UnitGroups.F,
                'B' => CharacterBase.UnitGroups.B,
                'D' => CharacterBase.UnitGroups.D,
                'E' => CharacterBase.UnitGroups.E,
                'N' => CharacterBase.UnitGroups.None
            };
        }
        private IEnumerator MoveGuidePointer(GameObject guidePointer, Vector3 startPosition, Vector3 endPosition, float speed = 1f, float waitTime = 1f)
        {
            _currentGuidePointer = guidePointer;
            while (true)
            {
                var startTime = Time.time;
                while (Time.time - startTime <= 1)
                {
                    var t = (Time.time - startTime) * speed;
                    _currentGuidePointer.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                    yield return null;
                }
                yield return new WaitForSeconds(waitTime);
                startTime = Time.time;
                while (Time.time - startTime <= 1)
                {
                    var t = (Time.time - startTime) * speed;
                    _currentGuidePointer.transform.position = Vector3.Lerp(endPosition, startPosition, t);
                    yield return null;
                }
                yield return new WaitForSeconds(waitTime);
            }
        }
        private void MatchCover(params Vector2Int[] disablePositions)
        {
            for (var x = -2; x <= 7; x++)
            {
                for (var y = -4; y <= 11; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (_covers.TryGetValue(pos, out var cover)) continue;
                    var position = new Vector3(x, y, 0);
                    var coverInstance = Instantiate(tutorialCoverPrefab, position, Quaternion.identity);
                    coverInstance.transform.SetParent(transform);
                    var boxCollider2D = coverInstance.AddComponent<BoxCollider2D>();
                    boxCollider2D.isTrigger = false;
                    var cov = coverInstance.AddComponent<Rigidbody2D>();
                    cov.isKinematic = true;
                    _covers.Add(pos, coverInstance);
                }
            }
            foreach (var cover in _covers.Values)
            {
                cover.SetActive(true);
            }
            foreach (var pos in disablePositions)
            {
                if (_covers.TryGetValue(pos, out var cover))
                {
                    cover.SetActive(false);
                }
            }
        }
        private void TextPopup(string popupText)
        {
            if (_currentTextPopup == null)
            {
                _currentTextPopup = textPopup;
                _currentTextPopup.gameObject.SetActive(false); // Initially set to inactive
            }
            _currentTextPopup.text = popupText;
            _currentTextPopup.gameObject.SetActive(true); // Enable it with the new text
        }
    }
}
