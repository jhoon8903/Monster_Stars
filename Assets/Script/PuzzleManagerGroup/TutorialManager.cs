using System;
using System.Collections;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using Script.RewardScript;
using TMPro;
using UnityEngine.UI;

namespace Script.PuzzleManagerGroup
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private GameObject tutorialCoverPrefab;
        [SerializeField] private GameObject uiTransform;
        [SerializeField] private GameObject guidePointerPrefab;
        [SerializeField] private TextMeshProUGUI textPopup;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private Sprite buttonDownSprite;
        [SerializeField] private Sprite buttonUpSprite;

        private readonly Dictionary<Vector2Int, GameObject> _covers = new Dictionary<Vector2Int, GameObject>();
        private Coroutine _pointerCoroutine;
        private GameObject _currentGuidePointer;
        private Queue<TutorialStep> _tutorialSteps;
        public TutorialStep CurrentTutorialStep;
        private TextMeshProUGUI _currentTextPopup;
        
        public static readonly CharacterBase.UnitGroups[] UnitGroupOrder = {
            CharacterBase.UnitGroups.Skeleton,
            CharacterBase.UnitGroups.Ogre, 
            CharacterBase.UnitGroups.Fishman,
            CharacterBase.UnitGroups.Orc,
            CharacterBase.UnitGroups.Ogre,
            CharacterBase.UnitGroups.Fishman,
            CharacterBase.UnitGroups.Orc,
            CharacterBase.UnitGroups.Skeleton,
            CharacterBase.UnitGroups.Fishman,
            CharacterBase.UnitGroups.Orc,
            CharacterBase.UnitGroups.Fishman,
            CharacterBase.UnitGroups.Orc,
            CharacterBase.UnitGroups.Ogre,
            CharacterBase.UnitGroups.Skeleton,
            CharacterBase.UnitGroups.Ogre
        };
        public struct TutorialStep
        {
            [CanBeNull] public readonly Vector2Int[] PositionsToRemove;
            public readonly Vector3 PointerStartPosition;
            public readonly Vector3 PointerEndPosition;
            public readonly int TutorialStepCount;
            public readonly string PopupText;

            public TutorialStep(Vector3 pointerStartPosition, Vector3 pointerEndPosition, int tutorialStepCount, string popupText, params Vector2Int[] positionsToRemove)
            {
                PositionsToRemove = positionsToRemove ?? Array.Empty<Vector2Int>(); // null 처리
                PointerStartPosition = pointerStartPosition;
                PointerEndPosition = pointerEndPosition;
                TutorialStepCount = tutorialStepCount;
                PopupText = popupText;
            }
        }
        private float _nextTriggerTime; // 다음 트리거가 발생할 수 있는 시간
        private const float TutorialTriggerCooldown = 1.5f;
        private void Start()
        {
            var isTutorial = bool.Parse(PlayerPrefs.GetString("TutorialKey", "true"));
            if (!isTutorial) return;
            spawnManager.OnMatchFound += HandleMatchFound;
            _tutorialSteps = new Queue<TutorialStep>();
            commonRewardManager.OnRewardSelected += HandleMatchFound;
                
            // 3Matched
            _tutorialSteps.Enqueue(new TutorialStep(
                new Vector3(4, 3.3f, 0), 
                new Vector3(3, 3.3f, 0), 
                1,
                "Move units by swiping them", 
                new Vector2Int(1, 4), new Vector2Int(2, 4), 
                new Vector2Int(3, 4), new Vector2Int(4, 4)));

            // 4Matched
            _tutorialSteps.Enqueue(new TutorialStep(
                    
                new Vector3(3,4.5f,0), 
                new Vector3(3,3.5f,0),
                2,
                "Swipe once more to combine units\nMerging 4 units will level up 2 units.", 
                new Vector2Int(1,4), 
                new Vector2Int(2,4), new Vector2Int(3,4), 
                new Vector2Int(4,4), new Vector2Int(3,5))
            );
            // 5Matched
            _tutorialSteps.Enqueue(new TutorialStep(
                    
                new Vector3(1, 2.3f, 0),
                new Vector3(0, 2.3f, 0),
                3,
                "Combining 5 makes a higher tier unit", 
                new Vector2Int(0, 1), new Vector2Int(0, 2), 
                new Vector2Int(0, 3), new Vector2Int(0, 4), 
                new Vector2Int(0,5), new Vector2Int(1, 3))
            );
            // Power Up Matched
            _tutorialSteps.Enqueue(new TutorialStep(
                    
                new Vector3(4,4.3f,0), 
                new Vector3(3,4.3f,0),
                4,
                "By combining coins, you can obtain a reinforcement box.", 
                new Vector2Int(1,5), new Vector2Int(2,5),
                new Vector2Int(3,5), new Vector2Int(4,5))
            );
            // Choose Power Up
            _tutorialSteps.Enqueue(new TutorialStep(
                new Vector3(3,0.5f,0), 
                new Vector3(3,1.5f,0), 
                5,
                "Strengthen your units by pressing the Enhance Select button.")
            );
            // // Press Long Object
            // _tutorialSteps.Enqueue(new TutorialStep(
            //         
            //     new Vector3(5,4f,0), 
            //     new Vector3(5,4.4f, 0),
            //     7,
            //     "Units can be removed by holding down the unit for 2 seconds.", 
            //     new Vector2Int(5,5))
            // );
            // Null Swap
            _tutorialSteps.Enqueue(new TutorialStep(
                    
                new Vector3(5,3.3f,0), new Vector3(6,3.3f,0),
                6,
                "You can remove units by swiping off the tile.", 
                new Vector2Int(5,4), new Vector2Int(6,4))
            );

            ProcessTutorialStep(_tutorialSteps.Dequeue());
        }
        private void ProcessTutorialStep(TutorialStep step)
        {
            CurrentTutorialStep = step;
            MatchCover(step.PositionsToRemove);
            TextPopup(step.PopupText);
            var guidePointer = Instantiate(guidePointerPrefab, step.PointerStartPosition, Quaternion.identity);
            _pointerCoroutine = StartCoroutine(MoveGuidePointer(guidePointer, step.PointerStartPosition, step.PointerEndPosition, 2f, 0.5f));
            guidePointer.GetComponent<Canvas>().sortingOrder = 3;
        }
        private void OnDestroy()
        {
            spawnManager.OnMatchFound -= HandleMatchFound;
            commonRewardManager.OnRewardSelected -= HandleMatchFound;
        }
        private void HandleMatchFound()
        {
            if (Time.time < _nextTriggerTime) return;
            if (_pointerCoroutine != null)
            {
                StopCoroutine(_pointerCoroutine);
                Destroy(_currentGuidePointer);
                _currentGuidePointer = null;
            }
            ProcessNextTutorialStep();
            _nextTriggerTime = Time.time + TutorialTriggerCooldown;
        }
        public IEnumerator EndTutorial()
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
            if (_currentTextPopup != null)
            {
                Destroy(_currentTextPopup);
                _currentTextPopup = null;
            }
            PlayerPrefs.SetString("TutorialKey", "false");
            spawnManager.isTutorial = false;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_complete");
            yield return null;
        }
        private void ProcessNextTutorialStep()
        {
            if (_tutorialSteps.Count > 0)
            {
                ProcessTutorialStep(_tutorialSteps.Dequeue());
            }
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
                'F' => CharacterBase.UnitGroups.Skeleton,
                'B' => CharacterBase.UnitGroups.Ogre,
                'D' => CharacterBase.UnitGroups.Orc,
                'E' => CharacterBase.UnitGroups.Fishman,
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
                    if (_currentGuidePointer == null) yield break;
                    _currentGuidePointer.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                    _currentGuidePointer.GetComponent<Image>().sprite = buttonDownSprite;
                    yield return null;
                }
                yield return new WaitForSeconds(waitTime);
                startTime = Time.time;
                while (Time.time - startTime <= 1)
                {
                    var t = (Time.time - startTime) * speed;
                    if (_currentGuidePointer == null) yield break;
                    _currentGuidePointer.transform.position = Vector3.Lerp(endPosition, startPosition, t);
                    _currentGuidePointer.GetComponent<Image>().sprite = buttonUpSprite;
                    yield return null;
                }
                yield return new WaitForSeconds(waitTime);
            }
        }
        private void MatchCover([CanBeNull] params Vector2Int[] disablePositions)
        {
            if (disablePositions == null) return;
            for (var x = -2; x <= 7; x++)
            {
                for (var y = -4; y <= 14; y++)
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
