using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;

namespace Script.PuzzleManagerGroup
{
    public sealed class SwipeManager : MonoBehaviour
    {
        public bool isBusy = false;
        private GameObject _startObject; // 초기에 터치된 객체를 추적하는 데 사용됩니다.
        private GameObject _returnObject; // 원래 위치로 돌아갈 객체를 추적하는 데 사용됩니다.
        private Vector2 _firstTouchPosition; // 첫 터치의 위치를 저장합니다.
        private Vector2 _emptyGridPosition; // 빈 그리드의 위치를 저장합니다.
        [SerializeField] private float minSwipeLength = 1.0f; // 스와이프로 인식되는 최소 길이입니다.
        [SerializeField] private SpawnManager spawnManager; // 스폰매니저를 참조합니다.
        [SerializeField] private CountManager countManager; // 카운트매니저를 참조합니다.
        [SerializeField] private LayerMask characterLayer; // 캐릭터 레이어를 저장합니다.
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private CommonRewardManager rewardManager;
        [SerializeField] private EnforceManager enforceManager;

        // CountManager를 요청하여 캐릭터의 이동 허용 여부를 확인합니다.
        private bool CanMove()  
        {
            return countManager.CanMove();
        }

        // 프레임당 한 번씩 실행되는 Unity 콜백 메서드입니다. 특히 터치 다운, 터치 업 및 드래그 이벤트를 확인하여 사용자의 입력을 처리합니다.
        private void Update()
        {
            var point2D = GetTouchPoint();

            if (Input.GetMouseButtonDown(0) && _startObject == null)
            {
                HandleTouchDown(point2D);
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

        // 사용자의 터치 포인트 또는 마우스 클릭 포인트를 화면 좌표에서 세계 좌표로 변환합니다.
        private static Vector2 GetTouchPoint()
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(worldPoint.x, worldPoint.y);
        }
        
        // 선택된 게임 오브젝트를 식별하고 첫 번째 터치 위치를 저장하는 초기 터치 또는 클릭 이벤트를 처리합니다.
        private void HandleTouchDown(Vector2 point2D)
        {
            var hit = Physics2D.Raycast(point2D, Vector2.zero, Mathf.Infinity, characterLayer);
            if (hit.collider == null) return;
            _startObject = hit.collider.gameObject;
            ScaleObject(_startObject, new Vector3(1.2f,1.2f,1.2f), 0.2f);
            _startObject.GetComponent<CharacterBase>().IsClicked = true;
            _firstTouchPosition = point2D;
        }

        // 사용자가 손가락을 떼거나 마우스 버튼을 놓을 때 처리합니다. 시작 개체의 크기를 조정하고 다음 스 와이프를 위해 무효화합니다.
        private void HandleTouchUp()
        {
            ScaleObject(_startObject, Vector3.one, 0.2f);
            List<GameObject> allObject = FindObjectOfType<CharacterPool>().UsePoolCharacterList();

            foreach (var character in allObject)
            {
                if (character.GetComponent<CharacterBase>().IsClicked)
                {
                    ScaleObject(character, Vector3.one, 0.2f);
                    character.GetComponent<CharacterBase>().IsClicked = false;
                }
            }
            _startObject = null;
        }

        // 이 메서드는 사용자가 터치다운 후 손가락이나 마우스를 움직이는 드래그 이벤트를 처리합니다.
        // 움직임이 스와이프로 간주될 만큼 길면 스와이프 동작을 시작합니다.
        private void HandleDrag(Vector2 point2D)
        {
            if (isBusy || rewardManager.openBoxing) return;
            var swipe = point2D - _firstTouchPosition;
            if (!(swipe.sqrMagnitude > minSwipeLength * minSwipeLength)) return;
            if (!(Mathf.Abs(swipe.x) > 0.5f) && !(Mathf.Abs(swipe.y) > 0.5f)) return;
            _firstTouchPosition = point2D;
            Swipe(swipe);
            HandleTouchUp(); // Release mouse input
        }

        // 지정된 기간 동안 제공된 배율을 사용하여 주어진 객체의 크기를 조정합니다.
        private static void ScaleObject(GameObject obj, Vector3 scale, float duration)
        {
            if (obj != null)
            {
                obj.transform.DOScale(scale, duration);
            }
        }

         // 스와이프의 방향을 식별하고 스와이프의 시작 객체와 끝 객체를 결정합니다.
         // 개체가 식별되면 개체의 상태에 따라 SwitchAndMatches 코루틴 또는 NullSwap 코루틴을 시작합니다.
        private void Swipe(Vector2 swipe)
        {
            if (isBusy || rewardManager.openBoxing) return;
            if (!CanMove()) return;
            if (_startObject == null) return;

            var swipeAngle = Mathf.Atan2(swipe.y, swipe.x) * Mathf.Rad2Deg;
            swipeAngle = (swipeAngle < 0) ? swipeAngle + 360 : swipeAngle;
            var position = _startObject.transform.position;
            var startX = (int)position.x;
            var startY = (int)position.y;
            var endX = startX;
            var endY = startY;

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

            var startObject = spawnManager.CharacterObject(new Vector3(startX, startY, 0));
            var endObject = spawnManager.CharacterObject(new Vector3(endX, endY, 0));
            if (startObject && endObject != null)
            {
                StartCoroutine(SwitchAndMatches(startObject,endObject));
            }
            if (startObject == null || endObject != null) return;
            StartCoroutine(NullSwap(startObject, endX, endY));
            _startObject = null;
        }

        // 스와이프 끝에 빈 공간이 있는 null 스왑 시나리오를 처리합니다. 시작 개체는 빈 위치로 이동한 다음 개체 풀로 반환됩니다.
        private IEnumerator NullSwap(GameObject startObject, int endX, int endY)
        {
            if (isBusy || rewardManager.openBoxing) yield break;
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
        
         // 시작 개체와 끝 개체 사이의 전환을 시작합니다. 그런 다음 두 개체와 관련된 일치 항목을 확인합니다.
        private IEnumerator SwitchAndMatches(GameObject startObject, GameObject endObject)
        {
            if (isBusy || rewardManager.openBoxing) yield break;
            isBusy = true;
            if (startObject == null || endObject == null) yield break;
            var startObjectPosition = startObject.transform.position;
            var endObjectPosition = endObject.transform.position;
            Tween switch1 = startObject.transform.DOMove(endObjectPosition, 0.1f);
            Tween switch2 = endObject.transform.DOMove(startObjectPosition, 0.1f);
            yield return switch2.WaitForCompletion();
            countManager.IsSwapOccurred = true;
            StartCoroutine(MatchesCheck(startObject));
            yield return StartCoroutine(MatchesCheck(endObject));
            countManager.IsSwapOccurred = false;
            countManager.DecreaseMoveCount();
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
        }

         // 이 코루틴은 주어진 오브젝트가 MatchManager를 사용하여 매치의 일부인지 여부를 확인합니다.
        private IEnumerator MatchesCheck(GameObject characterObject)
        {
            if (characterObject == null) yield break;
            matchManager.IsMatched(characterObject);
            yield return null;
        }
    }
}