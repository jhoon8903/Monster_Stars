using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    private GameObject _startObject;
    private GameObject _returnObject;
    private Vector2 _initialScale;
    private Vector2 _hoveredScale = new Vector2(1.3f, 1.3f);
    private Vector2 _firstTouchPosition; 
    private float _minSwipeLength = 0.2f; 

    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private CountManager countManager;
    [SerializeField] private CharacterPool characterPool;
    [SerializeField] LayerMask _characterLayer;

    public void Initialize(int initialMoveCount)
    {
        countManager.Initialize(initialMoveCount);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 point2D = new Vector2(worldPoint.x, worldPoint.y);

            RaycastHit2D hit = Physics2D.Raycast(point2D, Vector2.zero, 0f, _characterLayer);

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

        if (Input.GetMouseButtonUp(0))
        {
            HandleTouchEnd();
        }

        if (Input.GetMouseButton(0))
        {
            if (_startObject != null)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 point2D = new Vector2(worldPoint.x, worldPoint.y);

                Vector2 swipe = point2D - _firstTouchPosition;
                if (swipe.sqrMagnitude > _minSwipeLength * _minSwipeLength)
                {
                    HandleSwipe(swipe);
                    _firstTouchPosition = point2D;
                }
            }
        }
    }

    public bool CanMove()
    {
        return countManager.CanMove();
    }

    public void HandleSwipe(Vector2 swipe)
    {
        if (!CanMove()) return;
        if (_startObject == null) return;

        float swipeAngle = Mathf.Atan2(swipe.y, swipe.x) * Mathf.Rad2Deg;
        swipeAngle = (swipeAngle < 0) ? swipeAngle + 360 : swipeAngle;

        int startX = (int)_startObject.transform.position.x;
        int startY = (int)_startObject.transform.position.y;
        int endX = startX;
        int endY = startY;

        if (swipeAngle >= 315 || swipeAngle < 45)
            endX += 1;
        else if (swipeAngle >= 45 && swipeAngle < 135)
            endY += 1;
        else if (swipeAngle >= 135 && swipeAngle < 225)
            endX -= 1;
        else if (swipeAngle >= 225 && swipeAngle < 315)
            endY -= 1;

        GameObject endObject = spawnManager.GetCharacterAtPosition(new Vector3(endX, endY, 0f));

        if (endObject != null)
        {
            HandleSwap(startX, startY, endX, endY, endObject);
        }
        else
        {
            StartCoroutine(NullSwap(startX, startY, endX, endY));
            spawnManager.RespawnCharacters();

        }

        _startObject = null;
    }

    private void HandleSwap(int startX, int startY, int endX, int endY, GameObject endObject)
    {
        if (_startObject == null) return;

        Vector2 startPosition = _startObject.transform.position;
        Vector2 endPosition = new Vector3(endX, endY, 0f);

        _startObject.transform.position = endPosition;
        endObject.transform.position = startPosition;
   

        _startObject.transform.localScale = _initialScale;
        countManager.DecreaseMoveCount();
    }

    public void HandleTouchEnd()
    {
        if (_startObject != null)
        {
            _startObject.transform.localScale = _initialScale;
            _startObject = null;
        }
    }

    public IEnumerator NullSwap(int startX, int startY, int endX, int endY)
    {
        Vector2 startPosition = _startObject.transform.position;
        Vector2 endPosition = new Vector3(endX, endY, 0f);
        _startObject.transform.position = endPosition;
        _returnObject = _startObject;
        _startObject.transform.localScale = _initialScale;
        
        yield return new WaitForSeconds(0.1f);

        characterPool.ReturnToPool(_returnObject);
    }

}
