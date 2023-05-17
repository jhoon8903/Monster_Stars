using System.Collections.Generic;
using Script;
using Script.CharacterManagerScript;
using UnityEngine;

public sealed class MatchManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;

    public bool IsMatched(GameObject swapCharacter, Vector3 swipeCharacterPosition)
    {
        var swapCharacterName = swapCharacter.GetComponent<CharacterBase>()._characterName;
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
                        nextCharacter.GetComponent<CharacterBase>()._characterName != swapCharacterName)
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
                    CharacterPool.ReturnToPool(matchedCharacters[2]);
                    CharacterPool.ReturnToPool(matchedCharacters[3]);
                    StartCoroutine(_spawnManager.MoveCharactersEmptyGrid(matchedCharacters[2].transform.position));
                    StartCoroutine(_spawnManager.MoveCharactersEmptyGrid(matchedCharacters[3].transform.position));
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                    isMatchFound = true;
                    return isMatchFound;
                case 3:
                    if (swipeCharacterPosition.x == matchedCharacters[1].transform.position.x)
                    {
                        CharacterPool.ReturnToPool(matchedCharacters[2]);
                        CharacterPool.ReturnToPool(matchedCharacters[3]);
                        matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                    }
                    
                    // Debug.Log($"matchedCharacters[1].transform.position.x: {matchedCharacters[2].transform.position.x}");
                    if (swipeCharacterPosition.x == matchedCharacters[2].transform.position.x)
                    {
                        CharacterPool.ReturnToPool(matchedCharacters[1]);
                        CharacterPool.ReturnToPool(matchedCharacters[3]);
                        matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                    }
                    
                    // Debug.Log($"matchedCharacters[1].transform.position.x: {matchedCharacters[3].transform.position.x}");
                    if (swipeCharacterPosition.x == matchedCharacters[3].transform.position.x)
                    {
                        CharacterPool.ReturnToPool(matchedCharacters[1]);
                        CharacterPool.ReturnToPool(matchedCharacters[2]);
                        matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                    }
                    isMatchFound = true;
                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 5)
        {
            switch (horizontalMatchCount)
            {
                case 1:
                    if (swipeCharacterPosition.y > matchedCharacters[2].transform.position.y && swipeCharacterPosition.y < matchedCharacters[3].transform.position.y)
                    {
                        CharacterPool.ReturnToPool(matchedCharacters[2]);
                        CharacterPool.ReturnToPool(matchedCharacters[4]);
                        matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                    }

                    if (swipeCharacterPosition.y > matchedCharacters[3].transform.position.y &&
                        swipeCharacterPosition.y < matchedCharacters[4].transform.position.y)
                    {
                        CharacterPool.ReturnToPool(matchedCharacters[3]);
                        CharacterPool.ReturnToPool(matchedCharacters[4]);
                        matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                    }

                    isMatchFound = true;
                    return isMatchFound;

                case 2:
                    // Debug.Log($"swipeCharacterPosition: {swipeCharacterPosition}");
                    // Debug.Log($"[2]: {matchedCharacters[2].transform.position} \n" +
                    //           $"[3]: {matchedCharacters[3].transform.position} \n" +
                    //           $"[4]: {matchedCharacters[4].transform.position} \n");
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

        if (horizontalMatchCount + verticalMatchCount == 8)
        {
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
        }
        return isMatchFound;
    }
}
