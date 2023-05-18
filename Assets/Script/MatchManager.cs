using System.Collections;
using System.Collections.Generic;
using Script;
using Script.CharacterManagerScript;
using UnityEngine;

public sealed class MatchManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;

    private IEnumerator ReturnAndMoveCharacter(GameObject character)
    {
        yield return StartCoroutine(CharacterPool.ReturnToPool(character));
        yield return StartCoroutine(_spawnManager.MoveCharactersEmptyGrid(character.transform.position));
    }

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
                    StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                    StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                    
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                    isMatchFound = true;
                    return isMatchFound;
                case 3:
                    if (swipeCharacterPosition.x == matchedCharacters[1].transform.position.x)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                        matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }
                    
                    if (swipeCharacterPosition.x == matchedCharacters[2].transform.position.x)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                        matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }
                    
                    if (swipeCharacterPosition.x == matchedCharacters[3].transform.position.x)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                        matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }

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
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                        matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }

                    if (swipeCharacterPosition.y > matchedCharacters[3].transform.position.y &&
                        swipeCharacterPosition.y < matchedCharacters[4].transform.position.y)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                        matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }

                    return isMatchFound;

                case 2:
                    if (swipeCharacterPosition == matchedCharacters[2].transform.position)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                        matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }
                    return isMatchFound;

                case 3:
                    if (swipeCharacterPosition == matchedCharacters[3].transform.position)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                        matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }

                    return isMatchFound;

                case 4:
                    if (swipeCharacterPosition == matchedCharacters[4].transform.position && 
                        matchedCharacters[1].transform.position.x < swipeCharacterPosition.x && 
                        matchedCharacters[2].transform.position.x > swipeCharacterPosition.x)
                    {
                        Debug.Log("2번 스왑");
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                        matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }

                    if (swipeCharacterPosition == matchedCharacters[4].transform.position &&
                        matchedCharacters[1].transform.position.x < swipeCharacterPosition.x &&
                        matchedCharacters[3].transform.position.x > swipeCharacterPosition.x)
                    {
                        Debug.Log("3번 스왑");
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                        matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }

                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 6)
        {
            switch (horizontalMatchCount)
            {
                case 1:
                    if (swipeCharacterPosition != matchedCharacters[1].transform.position) return isMatchFound;
                    StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                    StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                    StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                    StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                    isMatchFound = true;
                    return isMatchFound;

                case 2:

                    if (swipeCharacterPosition.y > matchedCharacters[3].transform.position.y && 
                        swipeCharacterPosition.y < matchedCharacters[5].transform.position.y)
                    {
                        if (swipeCharacterPosition.y > matchedCharacters[4].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                            matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (swipeCharacterPosition.y < matchedCharacters[4].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }

                    return isMatchFound;

                case 3:

                    // ㄱ Pattern
                    if (swipeCharacterPosition == matchedCharacters[3].transform.position && 
                        swipeCharacterPosition.y > matchedCharacters[4].transform.position.y)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                        matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }
                    
                    // ㄴ Pattern
                    if (swipeCharacterPosition == matchedCharacters[3].transform.position &&
                        swipeCharacterPosition.y < matchedCharacters[4].transform.position.y)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                        matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }
                    return isMatchFound;

                case 4:

                    if (swipeCharacterPosition == matchedCharacters[4].transform.position &&
                        swipeCharacterPosition.x < matchedCharacters[2].transform.position.x &&
                        swipeCharacterPosition.x > matchedCharacters[1].transform.position.x)
                    {
                        if (swipeCharacterPosition.y < matchedCharacters[5].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (swipeCharacterPosition.y > matchedCharacters[5].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }

                    if (swipeCharacterPosition == matchedCharacters[4].transform.position && 
                        swipeCharacterPosition.x < matchedCharacters[3].transform.position.x &&
                        swipeCharacterPosition.x > matchedCharacters[1].transform.position.x)
                    {
                        if (swipeCharacterPosition.y < matchedCharacters[5].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (swipeCharacterPosition.y > matchedCharacters[5].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }
                    return isMatchFound;

                case 5:
                    if (swipeCharacterPosition == matchedCharacters[5].transform.position)
                    {
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                        matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                        matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                        StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                        matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                        isMatchFound = true;
                        return isMatchFound;
                    }
                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 7)
        {
            switch (horizontalMatchCount)
            {
                case 3:
                    if (swipeCharacterPosition == matchedCharacters[3].transform.position &&
                        matchedCharacters[3].transform.position.y > matchedCharacters[5].transform.position.y)
                    {
                        if (matchedCharacters[3].transform.position.x > matchedCharacters[1].transform.position.x)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (matchedCharacters[3].transform.position.x < matchedCharacters[1].transform.position.x)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }

                    if (swipeCharacterPosition == matchedCharacters[3].transform.position &&
                        matchedCharacters[3].transform.position.y < matchedCharacters[5].transform.position.y)
                    {
                        if (matchedCharacters[3].transform.position.x > matchedCharacters[1].transform.position.x)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (matchedCharacters[3].transform.position.x < matchedCharacters[1].transform.position.x)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }
                    return isMatchFound;

                case 4: 

                    if (swipeCharacterPosition == matchedCharacters[4].transform.position &&
                        matchedCharacters[2].transform.position.x < matchedCharacters[4].transform.position.x)
                    {
                        if (matchedCharacters[4].transform.position.y > matchedCharacters[5].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (matchedCharacters[4].transform.position.y < matchedCharacters[5].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }

                    if (swipeCharacterPosition == matchedCharacters[4].transform.position &&
                        matchedCharacters[2].transform.position.x > matchedCharacters[4].transform.position.x)
                    {
                        if (matchedCharacters[4].transform.position.y > matchedCharacters[5].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (matchedCharacters[4].transform.position.y < matchedCharacters[5].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            matchedCharacters[2].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }
                    return isMatchFound;
            }
        }

        if (horizontalMatchCount + verticalMatchCount == 8)
        {
            switch (horizontalMatchCount)
            {
                case 3:
                    if (swipeCharacterPosition == matchedCharacters[3].transform.position)
                    {
                        if (matchedCharacters[3].transform.position.x < matchedCharacters[1].transform.position.x)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[7]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            matchedCharacters[6].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (matchedCharacters[3].transform.position.x > matchedCharacters[1].transform.position.x)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[7]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[5]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            matchedCharacters[6].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[4].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[6]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }
                    return isMatchFound;

                case 5:
                    if (swipeCharacterPosition == matchedCharacters[5].transform.position)
                    {
                        if (matchedCharacters[5].transform.position.y < matchedCharacters[6].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[7]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[6].GetComponent<CharacterBase>().LevelUp();
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }

                        if (matchedCharacters[5].transform.position.y > matchedCharacters[6].transform.position.y)
                        {
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[2]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[4]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[7]));
                            matchedCharacters[1].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[3].GetComponent<CharacterBase>().LevelUp();
                            matchedCharacters[6].GetComponent<CharacterBase>().LevelUp();
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[1]));
                            StartCoroutine(ReturnAndMoveCharacter(matchedCharacters[3]));
                            matchedCharacters[5].GetComponent<CharacterBase>().LevelUp();
                            isMatchFound = true;
                            return isMatchFound;
                        }
                    }
                    return isMatchFound;
            }  
        }
        return isMatchFound;
    }
}
