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
        var matchedCharacters = new List<GameObject>();

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
            
            if (dirName == "Horizontal")
                horizontalMatchCount += matchCount;
            else
                verticalMatchCount += matchCount;
            
            switch (matchCount)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    break;
            }
            matchedCharacters.AddRange(matchedObjects);
        }

        if (horizontalMatchCount + verticalMatchCount == 4)
        {
            switch (horizontalMatchCount)
            {
                case 1:
                    Debug.Log($"swipeCharacterPosition: {swipeCharacterPosition}");
                    Debug.Log($"matchedCharacters Index \n[0]:{matchedCharacters[0].transform.position} \n[1]:{matchedCharacters[1].transform.position} \n[2]:{matchedCharacters[2].transform.position} \n[3]:{matchedCharacters[3].transform.position} ");
                    // if (swipeCharacterPosition == matchedCharacters[0].transform.position)
                    // {
                    //     matchedCharacters[0].GetComponent<CharacterBase>().LevelUp();
                    // }
                    Debug.LogWarning($"Match count of 3 found vertical directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 3:
                    Debug.LogWarning($"Match count of 3 found horizontal directions");
                    isMatchFound = true;
                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 5)
        {
            switch (horizontalMatchCount)
            {
                case 1:
                    Debug.LogWarning($"Match count of 4 found vertical directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 2:
                    Debug.LogWarning($"Match count of 3 found vertical directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 3:
                    Debug.LogWarning($"Match count of 3 found horizontal directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 4:
                    Debug.LogWarning($"Match count of 4 found horizontal directions");
                    isMatchFound = true;
                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 6)
        {
            switch (horizontalMatchCount)
            {
                case 1:
                    Debug.LogWarning($"Match count of 5 found vertical directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 2:
                    Debug.LogWarning($"Match count of 4 found vertical directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 3:
                    Debug.LogWarning($"Match count of 3 x 3  found cross directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 4:
                    Debug.LogWarning($"Match count of 4  found horizontal directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 5:
                    Debug.LogWarning($"Match count of 5  found horizontal directions");
                    isMatchFound = true;
                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 7)
        {
            switch (horizontalMatchCount)
            {
                case 3:
                    Debug.LogWarning($"Match count of h3 x v4  found cross directions");
                    isMatchFound = true;
                    return isMatchFound;
                case 4: 
                    Debug.LogWarning($"Match count of h4 x v3  found cross directions");
                    isMatchFound = true;
                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount != 8) return isMatchFound;
        switch (horizontalMatchCount)
        {
            case 3:
                Debug.LogWarning($"Match count of h3 x v5  found cross directions");
                isMatchFound = true;
                return isMatchFound;
            case 5:
                Debug.LogWarning($"Match count of h5 x v3  found cross directions");
                isMatchFound = true;
                return isMatchFound;
        }
        return isMatchFound;
    }
}
