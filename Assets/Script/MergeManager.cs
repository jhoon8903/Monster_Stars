using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    public GridManager gridManager;
    public CharacterPool characterPool;

    public bool CheckForMatches(GameObject character)
    {
        int matchCountHorizontal = CountMatches(character, Vector2.right) + CountMatches(character, Vector2.left) + 1;
        int matchCountVertical = CountMatches(character, Vector2.up) + CountMatches(character, Vector2.down) + 1;

        if (matchCountHorizontal >= 3 || matchCountVertical >= 3)
        {
            Debug.Log("Merging detected");

            List<GameObject> matchedCharacters = new List<GameObject> { character };

            if (matchCountHorizontal >= 3)
            {
                Debug.Log("Merging horizontally");
                CollectMatchedCharacters(matchedCharacters, character.transform.position, character.tag, Vector2.right);
                CollectMatchedCharacters(matchedCharacters, character.transform.position, character.tag, Vector2.left);
            }
            if (matchCountVertical >= 3)
            {
                Debug.Log("Merging vertically");
                CollectMatchedCharacters(matchedCharacters, character.transform.position, character.tag, Vector2.up);
                CollectMatchedCharacters(matchedCharacters, character.transform.position, character.tag, Vector2.down);
            }
            MergeMatchedCharacters(character, matchedCharacters); // Remove the 'direction2' parameter
            return true;
        }
        return false;
    }


    private int CountMatches(GameObject character, Vector2 direction)
    {
        int matchCount = 0;
        Vector2 currentPosition = character.transform.position;
        string characterPrefabName = character.transform.parent.name.Replace("(Clone)", "").Trim();
        Debug.Log($"characterPrefabName is {characterPrefabName}");
        while (true)
        {
            currentPosition += direction;
            GameObject otherCharacter = gridManager.GetCharacterAtPosition(currentPosition);
            Debug.Log($"otherCharacter is {otherCharacter}");
            if (otherCharacter != null)
            {
                string otherCharacterPrefabName = otherCharacter.transform.name.Replace("(Clone)", "").Trim();
                Debug.Log($"otherCharacterPrefabName is {otherCharacterPrefabName}");
                // Add a condition to check if the character should be considered for matching
                if (otherCharacterPrefabName == characterPrefabName && ShouldConsiderForMatching(otherCharacter))
                {
                    matchCount++;
                    Debug.Log("Match found at: " + currentPosition);
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
        // Add your custom condition here to decide if the character should be considered for matching.
        // For example, you can check the character's tag, layer, or any other property.

        // If the character should not be matched, return false.
        // Otherwise, return true.

        // Example: return character.tag != "NonMatchable";
        return true;
    }






    private void MergeMatchedCharacters(GameObject character, List<GameObject> matchedCharacters)
    {
        Vector2 currentPosition = character.transform.position;
        string characterTag = character.tag;
        int currentLevel = int.Parse(character.name.Substring(character.name.Length - 1));

        if (matchedCharacters.Count >= 3)
        {
            Debug.Log("Performing merge");

            foreach (GameObject matchedCharacter in matchedCharacters)
            {
                gridManager.RemoveCharacterFromGrid(matchedCharacter.transform.position);
                Destroy(matchedCharacter);
            }

            int newLevel = Mathf.Min(currentLevel + 1, characterPool.GetMaxLevel(characterTag));
            GameObject newCharacter = characterPool.GetCharacterPrefab(characterTag, newLevel);
            newCharacter.name = $"{characterTag} Level {newLevel}";
            newCharacter.tag = characterTag;
            newCharacter.transform.position = currentPosition;

            gridManager.AddCharacterToGrid(currentPosition, newCharacter);
            gridManager.FillEmptyGridPositions(); // Call this function after adding the new character
        }
    }



    private void CollectMatchedCharacters(List<GameObject> matchedCharacters, Vector2 startPosition, string characterTag, Vector2 direction)
    {
        Vector2 currentPosition = startPosition + direction;
        GameObject currentCharacter = gridManager.GetCharacterAtPosition(currentPosition);
        while (currentCharacter != null && currentCharacter.tag == characterTag)
        {
            matchedCharacters.Add(currentCharacter);
            currentPosition += direction;
            currentCharacter = gridManager.GetCharacterAtPosition(currentPosition);
        }
    }

}
