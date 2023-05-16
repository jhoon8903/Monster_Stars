using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public sealed class MatchManager : MonoBehaviour
    {
        [FormerlySerializedAs("_spawnManager")] [SerializeField]
        private SpawnManager spawnManager;
        
        public bool IsMatched(Vector3 swipeCharacterPosition)
        {
            var matchFound = false;
            var nearCharacter = spawnManager.GetCharacterAtPosition(swipeCharacterPosition);
            var directions = new Vector3Int[]
            {
                Vector3Int.up,
                Vector3Int.down,
                Vector3Int.left,
                Vector3Int.right
            };

            foreach (var direction in directions)
            {
                var leftMatchCount = 0;
                var rightMatchCount = 0;
                var leftMatchedObjects = new List<GameObject>();
                var rightMatchedObjects = new List<GameObject>();

                // Check left side
                for (var i = 1; i <= 2; i++)
                {
                    var checkPos = swipeCharacterPosition - direction * i;
                    var checkCharacter = spawnManager.GetCharacterAtPosition(checkPos);
                    if (checkCharacter == null || checkCharacter.name != nearCharacter.name)
                    {
                        break;
                    }

                    leftMatchCount++;
                    leftMatchedObjects.Add(checkCharacter);
                }
                
                for (var i = 1; i <= 2; i++)
                {
                    var checkPos = swipeCharacterPosition + direction * i;
                    var checkCharacter = spawnManager.GetCharacterAtPosition(checkPos);

                    if (checkCharacter == null || checkCharacter.name != nearCharacter.name)
                    {
                        break;
                    }

                    rightMatchCount++;
                    rightMatchedObjects.Add(checkCharacter);
                }

                // Total match count
                var matchCount = leftMatchCount + rightMatchCount;
                if (matchCount < 2) continue;

                // Combine all matched objects into one list
                var matchedObjects = new List<GameObject>();
                matchedObjects.AddRange(leftMatchedObjects);
                matchedObjects.Add(nearCharacter);
                matchedObjects.AddRange(rightMatchedObjects);

                var totalMatchCount = leftMatchCount + rightMatchCount;
                var allMatchedObjects = leftMatchedObjects.Concat(rightMatchedObjects).ToList();

                switch (totalMatchCount)
                {
                    case 2 when leftMatchCount == 1 && rightMatchCount == 1:
                        Handle3MatchCenter(leftMatchedObjects, rightMatchedObjects, nearCharacter); // pass rightMatchedObjects instead of rightMatchCount
                        matchFound = true; // set matchFound to true when a match is found
                        break;
                    case 2 when (leftMatchCount == 2 && rightMatchCount == 0) || (leftMatchCount == 0 && rightMatchCount == 2):
                        Handle3MatchSide(allMatchedObjects, nearCharacter);
                        matchFound = true; // set matchFound to true when a match is found
                        break;
                    case 3:
                        Handle4Match(allMatchedObjects, nearCharacter);
                        matchFound = true; // set matchFound to true when a match is found
                        break;
                    case 4:
                        Handle5Match(allMatchedObjects);
                        matchFound = true; // set matchFound to true when a match is found
                        break;
                }
            }
            return matchFound;
        }
        
private void Handle3MatchCenter(List<GameObject> leftMatchedObjects, List<GameObject> rightMatchedObjects, GameObject nearCharacter)
{
    foreach (var matchedObject in leftMatchedObjects)
    {
        CharacterPool.ReturnToPool(matchedObject);
        spawnManager.MoveCharactersEmptyGrid(matchedObject.transform.position);
    }

    foreach (var matchedObject in rightMatchedObjects)
    {
        CharacterPool.ReturnToPool(matchedObject);
        spawnManager.MoveCharactersEmptyGrid(matchedObject.transform.position);
    }

    // Level up the remaining character
    nearCharacter.GetComponent<CharacterBase>().LevelUp();
}

private void Handle3MatchSide(IEnumerable<GameObject> matchedObjects, GameObject nearCharacter)
{
    foreach (var matchedObject in matchedObjects)
    {
        if (matchedObject == nearCharacter) continue;
        CharacterPool.ReturnToPool(matchedObject);
        spawnManager.MoveCharactersEmptyGrid(matchedObject.transform.position);
    }

    // Level up the remaining character
    nearCharacter.GetComponent<CharacterBase>().LevelUp();
}

private void Handle4Match(IReadOnlyList<GameObject> matchedObjects, GameObject nearCharacter)
{
    // Return all but the two center objects from the match
    for (var i = 0; i < matchedObjects.Count; i++)
    {
        if (matchedObjects[i] == nearCharacter || i == matchedObjects.Count / 2 || i == matchedObjects.Count / 2 - 1)
            continue;

        CharacterPool.ReturnToPool(matchedObjects[i]);
        spawnManager.MoveCharactersEmptyGrid(matchedObjects[i].transform.position);
    }

    // Level up the remaining character
    nearCharacter.GetComponent<CharacterBase>().LevelUp();
}

private void Handle5Match(IReadOnlyList<GameObject> matchedObjects)
{
    GameObject remainingCharacter = null;
    for (var i = 0; i < matchedObjects.Count; i++)
    {
        if (i == matchedObjects.Count / 2 || i == matchedObjects.Count / 2 - 1 ||
            i == matchedObjects.Count / 2 + 1)
        {
            remainingCharacter = matchedObjects[i];
            continue;
        }

        CharacterPool.ReturnToPool(matchedObjects[i]);
        spawnManager.MoveCharactersEmptyGrid(matchedObjects[i].transform.position);
    }

    // Level up the remaining character
    remainingCharacter?.GetComponent<CharacterBase>().LevelUp();
}

    }
}
