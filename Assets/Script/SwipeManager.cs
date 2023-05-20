using System;
using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using UnityEngine;
using DG.Tweening;

namespace Script
{
    public class SwipeManager : MonoBehaviour
    {
        private GameObject _startObject; // 초기에 터치된 객체를 추적하는 데 사용됩니다.
        private GameObject _returnObject; // 원래 위치로 돌아갈 객체를 추적하는 데 사용됩니다.
        private Vector2 _firstTouchPosition; // 첫 터치의 위치를 저장합니다.
        private Vector2 _emptyGridPosition; // 빈 그리드의 위치를 저장합니다.
        [SerializeField] private float minSwipeLength = 1.0f; // 스와이프로 인식되는 최소 길이입니다.
        [SerializeField] private SpawnManager spawnManager; // 스폰매니저를 참조합니다.
        [SerializeField] private CountManager countManager; // 카운트매니저를 참조합니다.
        [SerializeField] private LayerMask characterLayer; // 캐릭터 레이어를 저장합니다.
        [SerializeField] private MatchManager matchManager;


        /**
         * Camera && RayCast Swipe Function
         * if Object Switch => Swipe
         * if Null Switch Object => NullSwap
         */
        private void Update()   
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var point2D = new Vector2(worldPoint.x, worldPoint.y);
                var hit = Physics2D.Raycast(point2D, Vector2.zero, 0f, characterLayer);
                if (hit.collider != null) 
                {
                    if (_startObject != null) return;
                    _startObject = hit.collider.gameObject;
                    _startObject.transform.DOScale(new Vector3(0.8f,0.8f,0.8f), 0.2f);
                    _firstTouchPosition = point2D; 
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_startObject != null) 
                {
                    _startObject.transform.DOScale(new Vector3(0.6f,0.6f,0.6f), 0.2f);
                    _startObject = null;
                }
            }

            if (!Input.GetMouseButton(0)) return;
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var point2D = new Vector2(worldPoint.x, worldPoint.y);
                var swipe = point2D - _firstTouchPosition;
                if (!(swipe.sqrMagnitude > minSwipeLength * minSwipeLength)) return;
                _firstTouchPosition = point2D;
                Swipe(swipe);
            }
        }

        /**
         * Check MoveCount
         */
        private bool CanMove()  
        {
            return countManager.CanMove();
        }

        /**
         * Swipe (DOTween)
         * Angle 90 Degree Rotate 
         */
        private void Swipe(Vector2 swipe)
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
                    if (startY <= -1) return;
                        endY -=1;
                        break;
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

        /**
         * Null Swipe is NullSwap
         * Return Object FadeOut(0.5) finished => Return Pool 
         */
        private IEnumerator NullSwap(GameObject startObject, int endX, int endY)
        {
            if (endY < 0) yield break;
            if (startObject == null) yield break;
            var _nullPosition = new Vector3Int(endX, endY, 0);
            Tween moveTween = startObject.transform.DOMove(_nullPosition, 0.3f);
            yield return moveTween.WaitForCompletion();
            countManager.DecreaseMoveCount();
            var spriteRenderer = startObject.GetComponent<SpriteRenderer>();
            Tween fadeOut = spriteRenderer.DOFade(0f, 0.3f);
            yield return fadeOut.WaitForCompletion();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            CharacterPool.ReturnToPool(startObject);
            StartCoroutine(spawnManager.PositionUpCharacterObject());
        }
        
        /**
         * Switch Object 
         */
        private IEnumerator SwitchAndMatches(GameObject startObject, GameObject endObject)
        {
            if (startObject && endObject == null) yield break;
    
            var startObjectPosition = startObject.transform.position;
            var endObjectPosition = endObject.transform.position;

            Tween _ = startObject.transform.DOMove(endObjectPosition, 0.3f);
            Tween switch2 = endObject.transform.DOMove(startObjectPosition, 0.3f);
            countManager.DecreaseMoveCount();
            yield return switch2.WaitForCompletion();
    
            var swapCharacterObjects = new List<GameObject> { startObject, endObject };

            var matchCheckCounter = swapCharacterObjects.Count;
            foreach (var characterObject in swapCharacterObjects)
            {
                StartCoroutine(MatchesCheck(characterObject, () => matchCheckCounter--));
            }
            yield return new WaitUntil(() => matchCheckCounter <= 0);
            StartCoroutine(spawnManager.PositionUpCharacterObject());
        }

        private IEnumerator MatchesCheck(GameObject characterObjects, Action onComplete = null)
        {
            matchManager.IsMatched(characterObjects);
            yield return null;
            onComplete?.Invoke();
        }

        public static IEnumerator OneWayMove(GameObject gameObject, Vector3Int nullPosition)
        {
            Tween complete = gameObject.transform.DOMove(nullPosition, 0.3f);
            yield return complete.WaitForCompletion();
        }

    }
}