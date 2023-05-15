using System.Collections;
using Script.CharacterManagerScript;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class SwipeManager : MonoBehaviour
    {
        private GameObject _startObject; // 초기에 터치된 객체를 추적하는 데 사용됩니다.
        private GameObject _returnObject; // 원래 위치로 돌아갈 객체를 추적하는 데 사용됩니다.
        private Vector2 _initialScale; // 터치된 객체의 초기 크기를 저장합니다.
        private Vector2 _firstTouchPosition; // 첫 터치의 위치를 저장합니다.
        private Vector2 _emptyGridPosition; // 빈 그리드의 위치를 저장합니다.
        [SerializeField] private float _minSwipeLength = 0.2f; // 스와이프로 인식되는 최소 길이입니다.
        [FormerlySerializedAs("spawnManager")] [SerializeField] private SpawnManager _spawnManager; // 스폰매니저를 참조합니다.
        [FormerlySerializedAs("countManager")] [SerializeField] private CountManager _countManager; // 카운트매니저를 참조합니다.
        [FormerlySerializedAs("_characterLayer")] [SerializeField]
        private LayerMask characterLayer; // 캐릭터 레이어를 저장합니다.
        [SerializeField] private MatchManager _matchManager;

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
            return _countManager.CanMove();
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
            var endObject = _spawnManager.GetCharacterAtPosition(new Vector3(endX, endY, 0f));

            if (endObject != null)
            {
                HandleSwap(startX, startY, endX, endY, endObject);
                var endPosition = new Vector3(endX, endY, 0f);
                var _swipeCharacterPositon = endPosition;
                var _characterObjectName = endObject;
                _matchManager.IsMatched(_swipeCharacterPositon);
            }
            else
            {
                StartCoroutine(NullSwap(startX, startY, endX, endY));
            }
            _startObject = null;
        }
    
        // 스와이프 된 두 케릭터 오브젝트의 위치를 바꿉니다.
        private void HandleSwap(int startX, int startY, int endX, int endY, GameObject endObject)
        {
            if (_startObject == null) return;
            var startPosition = _startObject.transform.position;
            var endPosition = new Vector3(endX, endY, 0f);
            endObject.transform.position = startPosition;
            _startObject.transform.position = endPosition;
            _startObject.transform.localScale = _initialScale;
            _countManager.DecreaseMoveCount();
        }

        //  터치가 끝나면 초기화 합니다. (터치시 호버 작동 터치 완료 후 호버 초기화)
        private void HandleTouchEnd()
        {
            if (_startObject == null) return;
            _startObject.transform.localScale = _initialScale;
            _startObject = null;
        }

        //  케릭터 오브젝트가 빈 Grid로 스와이프되면 오브젝트를 Pool 반환 하는 기능
        private IEnumerator NullSwap(int startX, int startY, int endX, int endY)
        {
            Vector2 startPosition = _startObject.transform.position;
            Vector2 endPosition = new Vector3(endX, endY, 0f);
            _startObject.transform.position = endPosition;
            _countManager.DecreaseMoveCount();
            _returnObject = _startObject;
            _startObject.transform.localScale = _initialScale;
            _emptyGridPosition = new Vector2(startX, startY);
            // Debug.Log(_emptyGridPosition);
        
            yield return new WaitForSeconds(0.1f);

            CharacterPool.ReturnToPool(_returnObject);
            _spawnManager.MoveCharactersEmptyGrid(_emptyGridPosition);
        }

    }
}
