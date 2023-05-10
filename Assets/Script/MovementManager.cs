using UnityEngine;
using System.Collections;

public class MovementManager : MonoBehaviour
{
    public float moveDuration = 0.3f;
    public CountManager countManager;
    public float hoverScaleFactor = 1.1f;
    public float minSwipeDistance = 1f;
    public MergeManager mergeManager;


    private GameObject selectedCharacter;
    private Vector3 originalScale;
    private Vector2 initialMousePos;
    public GridManager gridManager;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit;
            int layerMask = LayerMask.GetMask("Character");
            hit = Physics2D.Raycast(initialMousePos, Vector2.zero, layerMask);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Character") && countManager.CanMove())
                {
                    if (selectedCharacter != null)
                    {
                        selectedCharacter.transform.localScale = originalScale;
                    }
                    selectedCharacter = hit.collider.gameObject;
                    originalScale = selectedCharacter.transform.localScale;
                    selectedCharacter.transform.localScale *= hoverScaleFactor;
                }
            }
            else
            {
                if (selectedCharacter != null)
                {
                    selectedCharacter.transform.localScale = originalScale;
                    selectedCharacter = null;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && selectedCharacter != null)
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = currentMousePos - initialMousePos;
            float swipeDistance = direction.magnitude;

            if (swipeDistance >= minSwipeDistance)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360; // Convert to positive degrees
                Vector2 targetPosition = selectedCharacter.transform.position;

                if (angle >= 320 || angle < 20)
                {
                    targetPosition.x += 1;
                }
                else if (angle >= 70 && angle < 110)
                {
                    targetPosition.y += 1;
                }
                else if (angle >= 160 && angle < 200)
                {
                    targetPosition.x -= 1;
                }
                else if (angle >= 250 && angle < 290)
                {
                    targetPosition.y -= 1;
                }


                if (gridManager.IsPositionInsideGrid(targetPosition))
                {
                    if (CanMove(selectedCharacter.transform.position, targetPosition))
                    {
                        GameObject targetCharacter = GetCharacterAtPosition(targetPosition);
                        if (targetCharacter != null)
                        {
                            StartCoroutine(SwapCharacters(selectedCharacter, targetCharacter));
                            countManager.DecreaseMoveCount();
                        }
                    }
                }
            }

            selectedCharacter.transform.localScale = originalScale;
            selectedCharacter = null;
        }
    }


    private IEnumerator DestroyCharacterOutsideGrid(GameObject character, Vector2 targetPosition)
    {
        Vector2 initialPos = character.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            character.transform.position = Vector2.Lerp(initialPos, targetPosition, elapsedTime / moveDuration);
            yield return null;
        }

        // Destroy or return the character to the object pool
        Destroy(character);
    }


    private bool CanMove(Vector2 pos1, Vector2 pos2)
    {
        float distance = Vector2.Distance(pos1, pos2);
        return distance <= 1.5f && countManager.CanMove();
    }

    private GameObject GetCharacterAtPosition(Vector2 position)
    {
        RaycastHit2D hit;
        int layerMask = LayerMask.GetMask("Character");
        hit = Physics2D.Raycast(position, Vector2.zero, layerMask);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Character"))
        {
            return hit.collider.gameObject;
        }
        return null;
    }


    public IEnumerator SwapCharacters(GameObject character1, GameObject character2)
    {
        Vector2 initialPos1 = character1.transform.position;
        Vector2 initialPos2 = character2.transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            character1.transform.position = Vector2.Lerp(initialPos1, initialPos2, elapsedTime / moveDuration);
            character2.transform.position = Vector2.Lerp(initialPos2, initialPos1, elapsedTime / moveDuration);

            yield return null;
        }

        character1.transform.position = initialPos2;
        character2.transform.position = initialPos1;

        // Swap is complete, check for merges
        bool merge1 = mergeManager.CheckForMatches(character1);
        bool merge2 = mergeManager.CheckForMatches(character2);

        //if (merge1 || merge2)
        //{
        //    countManager.IncreaseMoveCount(); // 이동 횟수를 회복합니다. (선택 사항)
        //}

    }
}
