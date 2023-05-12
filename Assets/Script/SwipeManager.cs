using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    private GameObject firstTouchObject;
    private Vector2 initialScale;
    private Vector2 hoveredScale = new Vector2(1.3f, 1.3f);
    private Vector2 firstTouchPosition; // Declare firstTouchPosition
    private float minSwipeLength = 0.2f; // Declare minSwipeLength

    [SerializeField] private GridManager gridManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private CountManager countManager;
    [SerializeField] LayerMask characterLayer;

    // Initialize the move count
    public void Initialize(int initialMoveCount)
    {
        countManager.Initialize(initialMoveCount);
    }
    private void Update()
    {
        // Handle mouse down event
        if (Input.GetMouseButtonDown(0))
        {
            // Convert mouse position to world position
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 point2D = new Vector2(worldPoint.x, worldPoint.y);

            // Perform a raycast
            RaycastHit2D hit = Physics2D.Raycast(point2D, Vector2.zero, 0f, characterLayer);
            Debug.Log(hit.point);


            if (hit.collider != null)
            {
                Debug.Log(hit.collider);
                // Handle touch start
                firstTouchObject = hit.collider.gameObject;
                initialScale = firstTouchObject.transform.localScale;
                firstTouchObject.transform.localScale = initialScale * 1.2f; // enlarge the object

                // Store the position of the first touch
                firstTouchPosition = point2D;
            }

        }

        // Handle mouse up event
        if (Input.GetMouseButtonUp(0))
        {
            // Handle touch end
            HandleTouchEnd();
        }

        // Handle mouse drag event
        if (Input.GetMouseButton(0))
        {
            if (firstTouchObject != null)
            {
                // Convert mouse position to world position
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 point2D = new Vector2(worldPoint.x, worldPoint.y);

                // Calculate swipe
                Vector2 swipe = point2D - firstTouchPosition;
                if (swipe.sqrMagnitude > minSwipeLength * minSwipeLength)
                {
                    // Handle swipe
                    HandleSwipe(swipe);
                    firstTouchPosition = point2D;
                }
            }
        }
    }

    // Check if there are moves left
    public bool CanMove()
    {
        return countManager.CanMove();
    }

    // Method to handle the swipe operation
    public void HandleSwipe(Vector2 swipe)
    {
        if (!CanMove()) return;
        if (firstTouchObject == null) return;

        // Calculate the swipe angle
        float swipeAngle = Mathf.Atan2(swipe.y, swipe.x) * Mathf.Rad2Deg;
        swipeAngle = (swipeAngle < 0) ? swipeAngle + 360 : swipeAngle;

        int startX = (int)firstTouchObject.transform.position.x;
        int startY = (int)firstTouchObject.transform.position.y;
        int endX = startX;
        int endY = startY;

        // Calculate the end position based on the swipe angle
        if (swipeAngle >= 315 || swipeAngle < 45)
            endX += 1;
        else if (swipeAngle >= 45 && swipeAngle < 135)
            endY += 1;
        else if (swipeAngle >= 135 && swipeAngle < 225)
            endX -= 1;
        else if (swipeAngle >= 225 && swipeAngle < 315)
            endY -= 1;

        // Perform the swap operation
        HandleSwap(startX, startY, endX, endY);
    }

    // New method to handle the swap operation
    private void HandleSwap(int startX, int startY, int endX, int endY)
    {
        // Get the game objects
        GameObject startObject = gridManager.GetCharacterAtPosition(new Vector3(startX, startY, 0f));
        GameObject endObject = gridManager.GetCharacterAtPosition(new Vector3(endX, endY, 0f));

        // Check if the swap is valid
        if (startObject == null || endObject == null) return;

        // Perform the swap
        StartCoroutine(AnimateSwap(startObject, endObject));

        // Decrement the move count
        countManager.DecreaseMoveCount();
    }

    // Method to handle the touch end operation
    public void HandleTouchEnd()
    {
        // If there is a hovered object, reset its scale
        if (firstTouchObject != null)
        {
            firstTouchObject.transform.localScale = initialScale;
            firstTouchObject = null;
        }
    }

    private IEnumerator AnimateSwap(GameObject startObject, GameObject endObject)
    {
        Vector3 startPosition = startObject.transform.position;
        Vector3 endPosition = endObject.transform.position;

        float startTime = Time.time;
        float overTime = 0.5f;
        float endTime = startTime + overTime;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / overTime;
            startObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            endObject.transform.position = Vector3.Lerp(endPosition, startPosition, t);
            yield return null;
        }

        startObject.transform.position = endPosition;
        endObject.transform.position = startPosition;
    }
}
