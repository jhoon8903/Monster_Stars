using System.Collections;
using System.Collections.Generic;
using Script;
using Script.CharacterManagerScript;
using UnityEngine;

public sealed class MatchManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;

    private static void ReturnAndMoveCharacter(GameObject character)
    {
        CharacterPool.ReturnToPool(character);
    }
    public void IsMatched(GameObject swapCharacter)
    {
        var swapCharacterName = swapCharacter.GetComponent<CharacterBase>()._characterName;
        var swapCharacterPosition = swapCharacter.transform.position;
        var directions = new[]
        {
            (Vector3Int.left, Vector3Int.right, "Horizontal"), // Horizontal
            (Vector3Int.down, Vector3Int.up, "Vertical") // Vertical
        };
        var horizontalMatchCount = 0;
        var verticalMatchCount = 0;
        var matchedCharacters = new List<GameObject>();

        foreach (var (dir1, dir2, dirName) in directions)
        {
            var matchCount = 1; // To count the center character itself.
            var matchedObjects = new List<GameObject> { swapCharacter };
            foreach (var dir in new[] { dir1, dir2 })
            {
                var nextPosition = swapCharacterPosition + dir;
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
            
            if (dirName == "Horizontal") horizontalMatchCount += matchCount;
            else verticalMatchCount += matchCount;
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
                    Matches3Case1(matchedCharacters);
                    break;

                case 3:
                    Matches3Case2(matchedCharacters);
                    break;
            }
        }
        if (horizontalMatchCount + verticalMatchCount == 5)
        {
            switch (horizontalMatchCount)
            {
                case 1:
                    Matches4Case1(matchedCharacters);
                    break;
                case 2:
                    Matches3Case3(matchedCharacters);
                    break;
                case 3:
                    Matches3Case4(matchedCharacters);
                    break;
                case 4:
                    Matches4Case2(matchedCharacters);
                    break;
            }
        }
        if (horizontalMatchCount + verticalMatchCount == 6)
        {
            switch (horizontalMatchCount)
            {
                case 1:
                    Matches5Case1(matchedCharacters);
                    break;
                case 2:
                    Matches4Case3(matchedCharacters);
                    break;
                case 3:
                    Matches3X3Case(matchedCharacters);
                    break;
                case 4:
                    Matches4Case4(matchedCharacters);
                    break;
                case 5:
                    Matches5Case2(matchedCharacters);
                    break;
            }
        }
        if (horizontalMatchCount + verticalMatchCount == 7)
        {
            switch (horizontalMatchCount)
            {
                case 3:
                    Matches3X4Case(matchedCharacters);
                    break;
                case 4: 
                    Matches4X3Case(matchedCharacters);
                   break; 
            }
        }
        if (horizontalMatchCount + verticalMatchCount == 8)
        {
            switch (horizontalMatchCount)
            {
                case 3:
                    Matches3X5Case(matchedCharacters);
                    break;
                case 5:
                    Matches5X3Case(matchedCharacters);
                    break;
            }  
        }
    }
    private static void Matches3Case1(IReadOnlyList<GameObject> matchedCharacters)
    {
       ReturnAndMoveCharacter(matchedCharacters[2]);
       ReturnAndMoveCharacter(matchedCharacters[3]);
       matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
    }
    private static void Matches3Case2(IReadOnlyList<GameObject> matchedCharacters)
    {
        if (matchedCharacters[0].transform.position.x == matchedCharacters[1].transform.position.x)
        {
            ReturnAndMoveCharacter(matchedCharacters[2]);
            ReturnAndMoveCharacter(matchedCharacters[3]);
            matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
        }
        
        if (matchedCharacters[0].transform.position.x == matchedCharacters[2].transform.position.x)
        {
            ReturnAndMoveCharacter(matchedCharacters[1]);
            ReturnAndMoveCharacter(matchedCharacters[3]);
            matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
        }
                    
        if (matchedCharacters[0].transform.position.x == matchedCharacters[3].transform.position.x)
        {
            ReturnAndMoveCharacter(matchedCharacters[1]);
            ReturnAndMoveCharacter(matchedCharacters[2]);
            matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
        }
    }
    private static void Matches3Case3(IReadOnlyList<GameObject> matchedCharacters)
    { 
        ReturnAndMoveCharacter(matchedCharacters[3]); 
        ReturnAndMoveCharacter(matchedCharacters[4]);
        matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
    }
    private static void Matches3Case4(IReadOnlyList<GameObject> matchedCharacters)
    {
        ReturnAndMoveCharacter(matchedCharacters[1]);
        ReturnAndMoveCharacter(matchedCharacters[2]);
        matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
    }
    private static void Matches4Case1(IReadOnlyList<GameObject> matchedCharacters)
    {
        if (matchedCharacters[0].transform.position.y > matchedCharacters[2].transform.position.y && 
            matchedCharacters[0].transform.position.y < matchedCharacters[3].transform.position.y)
        {
            ReturnAndMoveCharacter(matchedCharacters[2]);
            ReturnAndMoveCharacter(matchedCharacters[4]);
            matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
            matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
        }

        if (matchedCharacters[0].transform.position.y > matchedCharacters[3].transform.position.y &&
            matchedCharacters[0].transform.position.y < matchedCharacters[4].transform.position.y)
        {
            ReturnAndMoveCharacter(matchedCharacters[3]);
            ReturnAndMoveCharacter(matchedCharacters[4]);
            matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
            matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
        }
    }
    private static void Matches4Case2(IReadOnlyList<GameObject> matchedCharacters)
    {

        if (matchedCharacters[2].transform.position.x > matchedCharacters[0].transform.position.x)
        { 
            ReturnAndMoveCharacter(matchedCharacters[1]); 
            ReturnAndMoveCharacter(matchedCharacters[3]); 
            matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
            matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
        }
        else
        {
            ReturnAndMoveCharacter(matchedCharacters[2]);
            ReturnAndMoveCharacter(matchedCharacters[3]);
            matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
            matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
        }
    }
    private static void Matches4Case3(IReadOnlyList<GameObject> matchedCharacters)
    {
        if (matchedCharacters[0].transform.position.y > matchedCharacters[4].transform.position.y)
        {
            ReturnAndMoveCharacter(matchedCharacters[5]);
            ReturnAndMoveCharacter(matchedCharacters[4]);
            matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
            matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
        }
        else
        {
            ReturnAndMoveCharacter(matchedCharacters[5]);
            ReturnAndMoveCharacter(matchedCharacters[3]); 
            matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]); 
            matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
        }
    }
    private static void Matches4Case4(IReadOnlyList<GameObject> matchedCharacters)
    {
        if (matchedCharacters[0].transform.position.x < matchedCharacters[2].transform.position.x &&
            matchedCharacters[0].transform.position.x > matchedCharacters[1].transform.position.x)
        {
            ReturnAndMoveCharacter(matchedCharacters[1]);
            ReturnAndMoveCharacter(matchedCharacters[3]);
            matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
            matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);

        }
        else
        {
            ReturnAndMoveCharacter(matchedCharacters[2]);
            ReturnAndMoveCharacter(matchedCharacters[3]);
            matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
            matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
        }
    }
    private static void Matches5Case1(IReadOnlyList<GameObject> matchedCharacters)
    {
        ReturnAndMoveCharacter(matchedCharacters[3]);
        ReturnAndMoveCharacter(matchedCharacters[5]);
        matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
        matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
        matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
        ReturnAndMoveCharacter(matchedCharacters[2]);
        ReturnAndMoveCharacter(matchedCharacters[4]);
        matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);;
    }
    private static void Matches5Case2(IReadOnlyList<GameObject> matchedCharacters)
    {
        ReturnAndMoveCharacter(matchedCharacters[2]);
        ReturnAndMoveCharacter(matchedCharacters[4]);
        matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
        matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
        matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
        ReturnAndMoveCharacter(matchedCharacters[1]);
        ReturnAndMoveCharacter(matchedCharacters[3]);
        matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
    }
    private static void Matches3X3Case(IReadOnlyList<GameObject> matchedCharacters)
    {
        ReturnAndMoveCharacter(matchedCharacters[2]);
        ReturnAndMoveCharacter(matchedCharacters[1]);
        ReturnAndMoveCharacter(matchedCharacters[5]);
        matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
        matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
    }
    private static void Matches3X4Case(IReadOnlyList<GameObject> matchedCharacters)
    {
        if (matchedCharacters[3].transform.position.y > matchedCharacters[5].transform.position.y)
        {
            ReturnAndMoveCharacter(matchedCharacters[2]);
            ReturnAndMoveCharacter(matchedCharacters[6]);
            ReturnAndMoveCharacter(matchedCharacters[5]);
            matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
            matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
            matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
        }
        else
        {
            ReturnAndMoveCharacter(matchedCharacters[2]);
            ReturnAndMoveCharacter(matchedCharacters[6]);
            ReturnAndMoveCharacter(matchedCharacters[4]);
            matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
            matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
            matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
        }
    }
    private static void Matches4X3Case(IReadOnlyList<GameObject> matchedCharacters)
    {
        if (matchedCharacters[2].transform.position.x < matchedCharacters[4].transform.position.x)
        {
            ReturnAndMoveCharacter(matchedCharacters[2]); 
            ReturnAndMoveCharacter(matchedCharacters[3]); 
            ReturnAndMoveCharacter(matchedCharacters[6]); 
            matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
            matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
            matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
        }
        else
        {
            ReturnAndMoveCharacter(matchedCharacters[1]); 
            ReturnAndMoveCharacter(matchedCharacters[3]); 
            ReturnAndMoveCharacter(matchedCharacters[6]); 
            matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
            matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
            matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
        }
    }
    private static void Matches3X5Case(IReadOnlyList<GameObject> matchedCharacters)
    {
        ReturnAndMoveCharacter(matchedCharacters[7]);
        ReturnAndMoveCharacter(matchedCharacters[5]); 
        ReturnAndMoveCharacter(matchedCharacters[2]); 
        matchedCharacters[6].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[6]);
        matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
        matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
        matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
        ReturnAndMoveCharacter(matchedCharacters[6]); 
        ReturnAndMoveCharacter(matchedCharacters[4]); 
        matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
    }
    private static void Matches5X3Case(IReadOnlyList<GameObject> matchedCharacters)
    {
        ReturnAndMoveCharacter(matchedCharacters[2]);
        ReturnAndMoveCharacter(matchedCharacters[4]);
        ReturnAndMoveCharacter(matchedCharacters[7]);
        matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
        matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
        matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
        matchedCharacters[6].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[6]);
        ReturnAndMoveCharacter(matchedCharacters[1]);
        ReturnAndMoveCharacter(matchedCharacters[3]);
        matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
    }
}
