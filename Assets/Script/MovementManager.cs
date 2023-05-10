using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MovementManager : MonoBehaviour
{
    public float moveDuration = 0.3f;
    public CountManager countManager;
    public float hoverScaleFactor = 1.1f;

    private GameObject selectedCharacter;
    private Vector3 originalScale;

    private void Update()
    {
        //if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit;
            int layerMask = LayerMask.GetMask("Character");
            Debug.Log(MousePos);
            hit = Physics2D.Raycast(MousePos, transform.forward, 10f, layerMask);
            if (hit.collider != null) 
            {
                Debug.Log("Click");
                if (hit.collider.gameObject.CompareTag("Character") && countManager.CanMove())
                {
                    selectedCharacter = hit.collider.gameObject;
                    originalScale = selectedCharacter.transform.localScale;
                    selectedCharacter.transform.localScale *= hoverScaleFactor;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && selectedCharacter != null)
        {
            selectedCharacter.transform.localScale = originalScale;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //int layerMask = LayerMask.GetMask("Character");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject targetCharacter = hit.collider.gameObject;

                if (targetCharacter.CompareTag("Character") && CanMove(selectedCharacter.transform.position, targetCharacter.transform.position))
                {
                    StartCoroutine(SwapCharacters(selectedCharacter, targetCharacter));
                    countManager.DecreaseMoveCount();
                }
            }

            selectedCharacter = null;
        }
    }

    private bool CanMove(Vector3 pos1, Vector3 pos2)
    {
        float distance = Vector3.Distance(pos1, pos2);
        return distance <= 1.5f && countManager.CanMove();
    }

    public IEnumerator SwapCharacters(GameObject character1, GameObject character2)
    {
        Vector3 initialPos1 = character1.transform.position;
        Vector3 initialPos2 = character2.transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            character1.transform.position = Vector3.Lerp(initialPos1, initialPos2, elapsedTime / moveDuration);
            character2.transform.position = Vector3.Lerp(initialPos2, initialPos1, elapsedTime / moveDuration);

            yield return null;
        }

        character1.transform.position = initialPos2;
        character2.transform.position = initialPos1;

        Debug.Log($"Moved character to ({initialPos2.x}, {initialPos2.y})");
    }
}
