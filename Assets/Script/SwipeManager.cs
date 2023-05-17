using System.Collections;
using Script.CharacterManagerScript;
using UnityEngine;
using DG.Tweening;

namespace Script
{
    public class SwipeManager : MonoBehaviour
    {
        private GameObject _startObject; // 초기에 터치된 객체를 추적하는 데 사용됩니다.
        private GameObject _returnObject; // 원래 위치로 돌아갈 객체를 추적하는 데 사용됩니다.
        private Vector2 _initialScale; // 터치된 객체의 초기 크기를 저장합니다.
        private Vector2 _firstTouchPosition; // 첫 터치의 위치를 저장합니다.
        private Vector2 _emptyGridPosition; // 빈 그리드의 위치를 저장합니다.
        public float duration;
        [SerializeField] private float _minSwipeLength = 0.2f; // 스와이프로 인식되는 최소 길이입니다.
        [SerializeField] private SpawnManager spawnManager; // 스폰매니저를 참조합니다.
        [SerializeField] private CountManager countManager; // 카운트매니저를 참조합니다.
        [SerializeField] private LayerMask characterLayer; // 캐릭터 레이어를 저장합니다.
        [SerializeField] private MatchManager matchManager;


        // 터치를 처리하고, 스와이프를 감지하며, 해당 스와이프에 따라 캐릭터를 이동시킵니다.
        private void Update()   
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Camera.main != null)
                {
                    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var point2D = new Vector2(worldPoint.x, worldPoint.y);
                    var hit = Physics2D.Raycast(point2D, Vector2.zero, 0f, characterLayer);

                    if (hit.collider != null)
                    {
                        if (_startObject != null)
                        {
                            _startObject.transform.localScale = _initialScale;
                        }
                        _startObject = hit.collider.gameObject;
                        _initialScale = _startObject.transform.localScale;
                        _startObject.transform.localScale = _initialScale * 1.2f; 
                        _firstTouchPosition = point2D;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                HandleTouchEnd();
            }

            if (!Input.GetMouseButton(0)) return;
            {
                // if (_startObject == null) return;
                if (Camera.main == null) return;
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var point2D = new Vector2(worldPoint.x, worldPoint.y);

                var swipe = point2D - _firstTouchPosition;
                if (!(swipe.sqrMagnitude > _minSwipeLength * _minSwipeLength)) return;
                _firstTouchPosition = point2D;
                HandleSwipe(swipe);
            }
        }
    
        // CountManager의 Count를 참조하여 이동가능횟수를 확인
        private bool CanMove()  
        {
            return countManager.CanMove();
        }
    
        // 사용자 편의를 위해 스와이프 각도를 보정하여 스와이프 각도에 따라 처리합니다.
        private void HandleSwipe(Vector2 swipe)
        {
            if (!CanMove()) return;
            if (_startObject == null) return;

            var swipeAngle = Mathf.Atan2(swipe.y, swipe.x) * Mathf.Rad2Deg;
            swipeAngle = (swipeAngle < 0) ? swipeAngle + 360 : swipeAngle;

            var position = _startObject.transform.position;
            var startX = (int)position.x;
            var startY = (int)position.y;
            var endX = startX;
            var endY = startY;

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
                    endY -= 1;
                    break;
            }
            var endObject = spawnManager.GetCharacterAtPosition(new Vector3(endX, endY, 0f));

            if (endObject != null)
            {
                HandleSwap(startX, startY, endX, endY, endObject);


            }
            else
            {
                StartCoroutine(NullSwap(startX, startY, endX, endY));
            }
            _startObject = null;
        }
        
        //  터치가 끝나면 초기화 합니다. (터치시 호버 작동 터치 완료 후 호버 초기화)
        private void HandleTouchEnd()
        {
            if (_startObject == null) return;
            _startObject.transform.localScale = _initialScale;
            _startObject = null;
        }
        
        private void HandleSwap(int startX, int startY, int endX, int endY, GameObject endObject)
        {
            if (_startObject == null) return;
            var startPosition = new Vector3(startX, startY, 0);
            var endPosition = new Vector3(endX, endY, 0f);
            StartCoroutine(SwapAndMatch(_startObject, endObject, endPosition, startPosition));
            _startObject.transform.localScale = _initialScale;
            countManager.DecreaseMoveCount();
        }

        //  케릭터 오브젝트가 빈 Grid로 스와이프되면 오브젝트를 Pool 반환 하는 기능
        private IEnumerator NullSwap(int startX, int startY, int endX, int endY)
        {
            Vector2 startPosition = _startObject.transform.position;
            Vector2 endPosition = new Vector3(endX, endY, 0f);
            _startObject.transform.position = endPosition;
            countManager.DecreaseMoveCount();
            _returnObject = _startObject;
            _startObject.transform.localScale = _initialScale;
            _emptyGridPosition = new Vector2(startX, startY);

            yield return new WaitForSeconds(0.1f);

            CharacterPool.ReturnToPool(_returnObject);
            spawnManager.MoveCharactersEmptyGrid(_emptyGridPosition);
        }
        
        private IEnumerator SwapAndMatch(GameObject startObject, GameObject endObject, Vector3 endPosition, Vector3 startPosition)
        {
            yield return MoveOverTime(startObject, endPosition);
            yield return MoveOverTime(endObject, startPosition);
            matchManager.IsMatched(startObject,endPosition);
            matchManager.IsMatched(endObject, startPosition);
        }
        
        private static IEnumerator MoveOverTime(GameObject objectToMove, Vector3 destination)
        {
            var tween = objectToMove.transform.DOMove(destination, 0.2f);
            yield return null;
        }
    }
}
