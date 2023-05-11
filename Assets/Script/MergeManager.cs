using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private RespawnManager respawnManager;
    //[SerializeField] private float mergeDelay = 0.5f;

    public IEnumerator MergeCharactersAtPosition(Vector2 position1, Vector2 position2)
    {
        GameObject firstCharacter = gridManager.GetCharacterAtPosition(position1);
        GameObject secondCharacter = gridManager.GetCharacterAtPosition(position2);

        // Swap the characters and check for matches
        gridManager.RemoveCharacterFromGrid(position1);
        gridManager.RemoveCharacterFromGrid(position2);
        gridManager.AddCharacterToGrid(position1, secondCharacter);
        gridManager.AddCharacterToGrid(position2, firstCharacter);

        List<GameObject> matchedCharacters1 = gridManager.FindMatchedCharacters(position1, 3);
        List<GameObject> matchedCharacters2 = gridManager.FindMatchedCharacters(position2, 3);

        if (matchedCharacters1.Count > 0 || matchedCharacters2.Count > 0)
        {
            foreach (GameObject matchedCharacter in matchedCharacters1)
            {
                gridManager.RemoveCharacterFromGrid(matchedCharacter.transform.position);
            }
            foreach (GameObject matchedCharacter in matchedCharacters2)
            {
                gridManager.RemoveCharacterFromGrid(matchedCharacter.transform.position);
            }
        }
        else
        {
            // Swap back if no match found
            gridManager.RemoveCharacterFromGrid(position1);
            gridManager.RemoveCharacterFromGrid(position2);
            gridManager.AddCharacterToGrid(position1, firstCharacter);
            gridManager.AddCharacterToGrid(position2, secondCharacter);
        }

        yield return new WaitForSeconds(0.5f);
        respawnManager.FillEmptyGridPositions();
    }
}
