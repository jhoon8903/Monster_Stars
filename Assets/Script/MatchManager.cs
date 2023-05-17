using System.Collections.Generic;
using Script;
using Script.CharacterManagerScript;
using UnityEngine;

public sealed class MatchManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;

    public bool IsMatched(GameObject swapCharacter, Vector3 swipeCharacterPosition)
    {
        var centerCharacterName = swapCharacter.GetComponent<CharacterBase>().name;
        var directions = new[]
        {
            (Vector3Int.left, Vector3Int.right, "Horizontal"), // Horizontal
            (Vector3Int.down, Vector3Int.up, "Vertical") // Vertical
        };

        var isMatchFound = false;
        var horizontalMatchCount = 0;
        var verticalMatchCount = 0;

        foreach (var (dir1, dir2, dirName) in directions)
        {
            var matchCount = 1; // To count the center character itself.
            var matchedObjects = new List<GameObject> { swapCharacter };

            foreach (var dir in new[] { dir1, dir2 })
            {
                var nextPosition = swipeCharacterPosition + dir;

                for (var i = 0; i < 2; i++)
                {
                    var nextCharacter = _spawnManager.GetCharacterAtPosition(nextPosition);
                    if (nextCharacter == null ||
                        nextCharacter.GetComponent<CharacterBase>().name != centerCharacterName)
                        break;

                    matchedObjects.Add(nextCharacter);
                    matchCount++;
                    nextPosition += dir;
                }
            }

            // Separate tracking for horizontal and vertical match counts
            if (dirName == "Horizontal")
                horizontalMatchCount += matchCount;
            else
                verticalMatchCount += matchCount;

            switch (matchCount)
            {
                case 3:
                case 4:
                case 5:
                    Debug.LogWarning($"Match count of {matchCount} found in {dirName} direction");
                    break;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 3)
        {
            switch (horizontalMatchCount)
            {
                case 3 :
                    Debug.LogWarning($"Match count of 3 found across horizontal directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 0 when verticalMatchCount == 3:
                    Debug.LogWarning($"Match count of 3 found across vertical directions");
                    isMatchFound = true;
                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 4)
        {
            if (horizontalMatchCount == 3 && verticalMatchCount == 1)
            {
                Debug.LogWarning($"Match count of 3 found across horizontal directions");
                isMatchFound = true;
                return isMatchFound;
            }
            if (horizontalMatchCount == 1 && verticalMatchCount == 3)
            {
                Debug.LogWarning($"Match count of 3 found across vertical directions");
                isMatchFound = true;
                return isMatchFound;
            }

            if (horizontalMatchCount == 4)
            {
                Debug.LogWarning($"Match count of 4 found across horizontal directions");
                isMatchFound = true;
                return isMatchFound;
            }

            if (horizontalMatchCount < 2 && verticalMatchCount == 4)
            {
                Debug.LogWarning($"Match count of 4 found across vertical directions");
                isMatchFound = true;
                return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 5)
        {

        }

        if (horizontalMatchCount + verticalMatchCount == 6)
        {
            if (horizontalMatchCount == 3 && verticalMatchCount ==3 )
            {
                Debug.LogWarning($"Match count of 5 found across vertical directions");
                isMatchFound = true;
                return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 7)
        {

        }

        if (horizontalMatchCount + verticalMatchCount == 8)
        {

        }

        if (!isMatchFound)
        {
            Debug.LogWarning($"No handler found for match count");

        }

        return isMatchFound;
    }
}
