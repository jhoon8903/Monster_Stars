using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private float mergeDelay = 0.5f;

    public bool CheckForMatches(GameObject character)
    {
        int horizontalMatches = CountMatches(character, Vector2.right) + CountMatches(character, Vector2.left) + 1;
        int verticalMatches = CountMatches(character, Vector2.up) + CountMatches(character, Vector2.down) + 1;

        if (horizontalMatches >= 3 || verticalMatches >= 3)
        {
            Debug.Log("Merging detected");
            if (horizontalMatches >= 3)
            {
                Debug.Log("Merging horizontally");
                StartCoroutine(MergeCharacters(character, Vector2.right, horizontalMatches - 1));
                StartCoroutine(MergeCharacters(character, Vector2.left, horizontalMatches - 1));
            }

            if (verticalMatches >= 3)
            {
                Debug.Log("Merging vertically");
                StartCoroutine(MergeCharacters(character, Vector2.up, verticalMatches - 1));
                StartCoroutine(MergeCharacters(character, Vector2.down, verticalMatches - 1));
            }

            return true;
        }

        return false;
    }

    private int CountMatches(GameObject character, Vector2 direction)
    {
        int matchCount = 0;
        Vector2 currentPosition = character.transform.position;
        string characterPrefabName = character.transform.parent.name.Replace("(Clone)", "").Trim();

        while (true)
        {
            currentPosition += direction;
            GameObject otherCharacter = gridManager.GetCharacterAtPosition(currentPosition);

            if (otherCharacter != null)
            {
                string otherCharacterPrefabName = otherCharacter.transform.name.Replace("(Clone)", "").Trim();
                if (otherCharacterPrefabName == characterPrefabName && ShouldConsiderForMatching(otherCharacter))
                {
                    matchCount++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return matchCount;
    }

    private bool ShouldConsiderForMatching(GameObject character)
    {
        // Add your condition here to determine if the character should be considered for matching
        return true;
    }

    private IEnumerator MergeCharacters(GameObject character, Vector2 direction, int count)
    {
        Vector2 currentPosition = character.transform.position;
        string characterPrefabName = character.transform.parent.name.Replace("(Clone)", "").Trim();

        for (int i = 0; i < count; i++)
        {
            currentPosition += direction;
            GameObject otherCharacter = gridManager.GetCharacterAtPosition(currentPosition);

            if (otherCharacter != null)
            {
                string otherCharacterPrefabName = otherCharacter.transform.name.Replace("(Clone)", "").Trim();
                if (otherCharacterPrefabName == characterPrefabName && ShouldConsiderForMatching(otherCharacter))
                {
                    // Perform the merge
                    gridManager.RemoveCharacterAtPosition(currentPosition);
                    Destroy(otherCharacter);
                }
            }
        }

        yield return new WaitForSeconds(mergeDelay);
    }

}
