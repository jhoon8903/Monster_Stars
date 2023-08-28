using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.PuzzleManagerGroup
{
    public sealed class SwipeManager : MonoBehaviour
    {
        private readonly WaitForSeconds _checkSoonWait = new WaitForSeconds(0.5f);
        private Coroutine _findUnitCoroutine;
        private readonly List<GameObject> _activatedLines = new List<GameObject>();
        private bool _isSwipe;
        private bool _isSoon;
        public bool isBusy;
        public bool isUp;
        private GameObject _startObject;
        private GameObject _returnObject;
        private Vector2 _firstTouchPosition;
        private Vector2 _emptyGridPosition;
        [SerializeField] private float minSwipeLength;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private CommonRewardManager rewardManager;
        [SerializeField] private EnforceManager enforceManager;
        [SerializeField] private GameObject pressObject;
        [SerializeField] private TutorialManager tutorialManager;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private GameObject levelUpPanel;
        [SerializeField] private GameObject commonPanel;
        [SerializeField] private GameObject pausePanel;

        private int _pointer;
        private void Start()
        {
            #if UNITY_EDITOR
            _pointer = -1;
            #elif UNITY_ANDROID
            _pointer = 0;
            #endif
        }

        private void Update()
        {
            if (levelUpPanel.activeInHierarchy || commonPanel.activeInHierarchy || pausePanel.activeInHierarchy || !CanMove()) return;
            var point2D = GetTouchPoint();

            if (Input.GetMouseButtonDown(0))
            {
                if (_startObject == null)
                {
                    HandleTouchDown(point2D);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleTouchUp();
            }
            else if (Input.GetMouseButton(0) && _startObject != null)
            {
                HandleDrag(point2D);
            }
        }
        private bool CanMove()  
        {
            return countManager.CanMove();
        }
        private static Vector2 GetTouchPoint()
        {
            var worldPoint = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(worldPoint.x, worldPoint.y);
        }
        public void StartFindUnit(CharacterBase characterBase)
        {
            if (_findUnitCoroutine != null || isBusy || GameManager.Instance.IsBattle)
            {
                StopCoroutine(_findUnitCoroutine);
                foreach (var line in _activatedLines)
                {
                    line.SetActive(false);
                }
                _activatedLines.Clear();
            }
            _findUnitCoroutine = StartCoroutine(FindUnit(characterBase));
        }
        private IEnumerator FindUnit(CharacterBase characterBase)
        {
            var unitList = characterPool.UsePoolCharacterList();

            foreach (var yellowLine in from unit in unitList select unit.GetComponent<CharacterBase>() into unitBase where unitBase.unitGroup == characterBase.unitGroup &&
                         unitBase.unitPuzzleLevel == characterBase.unitPuzzleLevel select unitBase.transform.Find("Yellow_Line").gameObject)
            {
                yellowLine.SetActive(true);
                _activatedLines.Add(yellowLine);
            }

            yield return new WaitForSecondsRealtime(2f);

            foreach (var line in _activatedLines)
            {
                line.SetActive(false);
            }
            _activatedLines.Clear();
        }
        private void HandleTouchDown(Vector2 point2D)
        {
            if (isBusy || _isSoon) return;
            SoundManager.Instance.PlaySound(SoundManager.Instance.touchUnitSound);
            var coverLayerMask = LayerMask.GetMask("Cover");
            var hitCover = Physics2D.Raycast(point2D, Vector2.zero, Mathf.Infinity, coverLayerMask);

            // If a Cover object was hit, don't interact with the Character
            if (hitCover.collider != null) return;

            var characterLayerMask = LayerMask.GetMask("Character");
            var hitCharacter = Physics2D.Raycast(point2D, Vector2.zero, Mathf.Infinity, characterLayerMask);
            
            if (hitCharacter.collider == null) return;
            _startObject = hitCharacter.collider.gameObject;
            ScaleObject(_startObject, new Vector3(1.2f,1.2f,1.2f), 0.2f);
            _startObject.GetComponent<CharacterBase>().IsClicked = true;
            
            _firstTouchPosition = point2D;
            StartCoroutine(CheckSoon());
            if (_startObject == null) return;
            if (spawnManager.isTutorial && tutorialManager.CurrentTutorialStep.TutorialStepCount < 7) return;
            StartCoroutine(CheckForLongPress());
        }
        private void HandleTouchUp()
        {
            ScaleObject(_startObject, Vector3.one, 0.2f);
            if (!_isSwipe && _startObject != null && !GameManager.Instance.IsBattle)
            {
                StartFindUnit(_startObject.GetComponent<CharacterBase>());
            }
            var allObjects = characterPool.UsePoolCharacterList();
            foreach (var character in allObjects.Where(character => character.GetComponent<CharacterBase>().IsClicked))
            {
                ScaleObject(character, Vector3.one, 0.2f);
                character.GetComponent<CharacterBase>().IsClicked = false;
            }
            _startObject = null;
            _isSwipe = false;
        }
        private void HandleDrag(Vector2 point2D)
        {
            if (isBusy || isUp ) return;
            var swipe = point2D - _firstTouchPosition;
            if (!(swipe.sqrMagnitude > minSwipeLength * minSwipeLength)) return;
            if (!(Mathf.Abs(swipe.x) > 0.5f) && !(Mathf.Abs(swipe.y) > 0.5f)) return;
            _firstTouchPosition = point2D;
            Swipe(swipe);
            _isSwipe = true;
            HandleTouchUp();
        }
        private void Swipe(Vector2 swipe)
        {
            if (isBusy || isUp ) return;
            if (_startObject == null) return;
            var swipeAngle = Mathf.Atan2(swipe.y, swipe.x) * Mathf.Rad2Deg;
            swipeAngle = (swipeAngle < 0) ? swipeAngle + 360 : swipeAngle;
            var position = _startObject.transform.position;
            var startX = (int)position.x;
            var startY = (int)position.y;
            var endX = startX;
            var endY = startY;
            if (spawnManager.isTutorial)
            {
                switch (tutorialManager.CurrentTutorialStep.TutorialStepCount)
                {
                  
                    case 1 when startX == 4 && startY == 4:
                    {
                        if (swipeAngle is < 135 or >= 225)
                        {
                            return;
                        }
                        break;
                    }
                    case 1:
                        return;
                    case 2 when startX == 3 && startY == 5:
                    {
                        if (swipeAngle is < 225 or >= 315)
                        {
                            return;
                        }
                        break;
                    }
                    case 2:
                        return;
                    case 3 when startX == 1 && startY == 3:
                    {
                        if (swipeAngle is < 135 or >= 225)
                        {
                            return;
                        }
                        break;
                    }
                    case 3:
                        return;
                    case 4 when startX == 4 && startY == 5:
                    {
                        if (swipeAngle is < 135 or >= 225)
                        {
                            return;
                        }
                        break;
                    }             
                    case 4:
                        return;
                    case 5 :
                        return;
                    case 6 when startX == 5 && startY == 4:
                    {
                        if (swipeAngle is > 45 or >= 315)
                        {
                            return;
                        }
                        break;
                    }
                    case 7:
                        return;
                }
            }
            // If diagonal movement is enabled, handle 8 directions
            if (enforceManager.diagonalMovement)
            {
                switch (swipeAngle)
                {
                    case >= 337.5f or < 22.5f:
                        endX += 1;
                        break;
                    case >= 22.5f and < 67.5f:
                        endX += 1;
                        endY += 1;
                        break;
                    case >= 67.5f and < 112.5f:
                        endY += 1;
                        break;
                    case >= 112.5f and < 157.5f:
                        endX -= 1;
                        endY += 1;
                        break;
                    case >= 157.5f and < 202.5f:
                        endX -= 1;
                        break;
                    case >= 202.5f and < 247.5f:
                        endX -= 1;
                        endY -= 1;
                        break;
                    case >= 247.5f and < 292.5f:
                        endY -=1;
                        break;
                    case >= 292.5f and < 337.5f:
                        endX += 1;
                        endY -=1;
                        break;
                }
            }
            else // else handle 4 directions as usual
            {
                switch (swipeAngle)
                {
                    case >= 315 or < 45:
                        endX += 1;
                        break;
                    case >= 45 and < 135:
                        endY += 1;
                        break;
                    case >= 135 and < 225:
                        endX -= 1;
                        break;
                    case >= 225 and < 315:
                        endY -=1;
                        break;
                }
            }

            var startObject = SpawnManager.CharacterObject(new Vector3(startX, startY, 0));
            var endObject = SpawnManager.CharacterObject(new Vector3(endX, endY, 0));
            if (startObject && endObject != null)
            {
                StartCoroutine(SwitchAndMatches(startObject,endObject));
            }
            if (startObject == null || endObject != null) return;
            StartCoroutine(NullSwap(startObject, endX, endY));
            _startObject = null;
        }
        private static void ScaleObject(GameObject obj, Vector3 scale, float duration)
        {
            if (obj != null)
            {
                obj.transform.DOScale(scale, duration);
            }
        }
        private IEnumerator CheckForLongPress()
        {
            if (_startObject == null) // Ensure _startObject is not null
                yield break;
            yield return new WaitForSecondsRealtime(1f);
            if (_startObject == null || !_startObject.GetComponent<CharacterBase>().IsClicked)
                yield break;
            var position = _startObject.transform.position;
            var pressObjectInstance = Instantiate(pressObject, new Vector3(position.x, position.y + 0.5f, position.z), Quaternion.identity);
            var frontObject = pressObjectInstance.transform.GetChild(0).GetChild(0); 
            var fillImage = frontObject.GetComponent<Image>();
            var timer = 0f;
            while (_startObject != null && _startObject.GetComponent<CharacterBase>().IsClicked)
            {
                timer += Time.deltaTime;
                fillImage.fillAmount = timer / 2f; // fillAmount를 0에서 1까지 조정
                if (timer >= 2f) // 눌러진 상태를 2초 동안 확인
                {
                    CharacterPool.ReturnToPool(_startObject);
                    countManager.DecreaseMoveCount();
                    yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
                    break;
                }
                yield return null;
            }
            fillImage.fillAmount = 0f; // fillAmount를 초기화
            Destroy(pressObjectInstance); // 인스턴스를 파괴
        }
        private IEnumerator CheckSoon()
        {
            _isSoon = true;
            yield return _checkSoonWait;
            _isSoon = false;
        }
        private IEnumerator NullSwap(GameObject startObject, int endX, int endY)
        {
            if (isBusy || isUp ) yield break;
            if (endY < 0) yield break;
            isBusy = true;
            if (startObject == null) yield break;
            if (startObject.transform.position.y == 0) yield return null;
            var nullPosition = new Vector3Int(endX, endY, 0);
            Tween moveTween = startObject.transform.DOMove(nullPosition, 0.3f);
            yield return moveTween.WaitForCompletion();
            countManager.DecreaseMoveCount();
            var spriteRenderer = startObject.GetComponent<SpriteRenderer>();
            Tween fadeOut = spriteRenderer.DOFade(0f, 0.3f);
            yield return fadeOut.WaitForCompletion();
            var color = spriteRenderer.color;
            color = new Color(color.r, color.g, color.b, 1f);
            spriteRenderer.color = color;
            CharacterPool.ReturnToPool(startObject);
            StartCoroutine(spawnManager.PositionUpCharacterObject());
        }
        private IEnumerator SwitchAndMatches(GameObject startObject, GameObject endObject)
        {
            if (isBusy || isUp) yield break;
            isBusy = true;
            if (startObject == null || endObject == null) yield break;
            var startObjectPosition = startObject.transform.position;
            var endObjectPosition = endObject.transform.position;
            startObject.transform.DOMove(endObjectPosition, 0.1f);
            Tween switch2 = endObject.transform.DOMove(startObjectPosition, 0.1f);
            yield return switch2.WaitForCompletion();
            countManager.IsSwapOccurred = true;
            yield return StartCoroutine(MatchesCheck(startObject));
            yield return StartCoroutine(MatchesCheck(endObject));
            countManager.IsSwapOccurred = false;
            countManager.DecreaseMoveCount();
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
        }
        private IEnumerator MatchesCheck(GameObject characterObject)
        {
            if (characterObject == null) yield break;
            if (!matchManager.IsMatched(characterObject)) yield break;
            yield return null;
        }
    }
}