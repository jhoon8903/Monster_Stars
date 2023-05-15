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

        public void IsMatched(Vector3 swipeCharacterPosition)
        {
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
                        Handle3MatchCenter(leftMatchedObjects, rightMatchedObjects, nearCharacter);
                        break;
                    case 2 when (leftMatchCount == 2 && rightMatchCount == 0) || (leftMatchCount == 0 && rightMatchCount == 2):
                        Handle3MatchSide(allMatchedObjects, nearCharacter);
                        break;
                    case 3:
                        Handle4Match(allMatchedObjects, nearCharacter);
                        break;
                    case 4:
                        Handle5Match(allMatchedObjects);
                        break;
                }
            }
        }

        // Handle3MatchCenter
        private void Handle3MatchCenter(List<GameObject> leftMatchedObjects, List<GameObject> rightMatchedObjects,
            GameObject nearCharacter)
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
        }

        private void Handle3MatchSide(IEnumerable<GameObject> matchedObjects, Object nearCharacter)
        {
            foreach (var t in matchedObjects)
            {
                if (t == nearCharacter) continue;
                CharacterPool.ReturnToPool(t);
                spawnManager.MoveCharactersEmptyGrid(t.transform.position);
            }
        }

        private void Handle4Match(IReadOnlyList<GameObject> matchedObjects, Object nearCharacter)
        {
            // Return all but the two center objects from the match
            for (var i = 0; i < matchedObjects.Count; i++)
            {
                if (matchedObjects[i] == nearCharacter || i == matchedObjects.Count / 2 || i == matchedObjects.Count / 2 - 1)
                    continue;

                CharacterPool.ReturnToPool(matchedObjects[i]);
                spawnManager.MoveCharactersEmptyGrid(matchedObjects[i].transform.position);
            }
        }

        
        private void Handle5Match(IReadOnlyList<GameObject> matchedObjects)
        {
            for (var i = 0; i < matchedObjects.Count; i++)
            {
                if (i == matchedObjects.Count / 2 || i == matchedObjects.Count / 2 - 1 ||
                    i == matchedObjects.Count / 2 + 1) continue;
                CharacterPool.ReturnToPool(matchedObjects[i]);
                spawnManager.MoveCharactersEmptyGrid(matchedObjects[i].transform.position);
            }
        }
    }
}
