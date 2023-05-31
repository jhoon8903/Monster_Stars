using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.PowerUpScript;
using Script.UIManager;
using UnityEngine;

namespace Script
{
    public sealed class MatchManager : MonoBehaviour
    {
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private CountManager countManager;
        [SerializeField] public TreasureManager treasureManager;
        
        // 이 메소드는 주어진 캐릭터 객체가 매치되는지 확인하는 기능을 수행합니다. 매치 여부를 반환합니다.
        public bool IsMatched(GameObject swapCharacter)
        {
            var matchFound = false;
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
                        var nextCharacter = spawnManager.CharacterObject(nextPosition);
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

            if (horizontalMatchCount + verticalMatchCount == 8)
            {
                matchFound = horizontalMatchCount switch
                {
                    3 => Matches3X5Case(matchedCharacters),
                    5 => Matches5X3Case(matchedCharacters),
                    _ => false
                };
            }

            if (horizontalMatchCount + verticalMatchCount == 7)
            {
                matchFound = horizontalMatchCount switch
                {
                    2 => Matches2X5Case(matchedCharacters),
                    3 => Matches3X4Case(matchedCharacters),
                    4 => Matches4X3Case(matchedCharacters),
                    5 => Matches5X2Case(matchedCharacters),
                    _ => matchFound
                };
            }

            if (horizontalMatchCount + verticalMatchCount == 6)
            {
                matchFound = horizontalMatchCount switch
                {
                    1 => Matches5Case1(matchedCharacters),
                    2 => Matches4Case3(matchedCharacters),
                    3 => Matches3X3Case(matchedCharacters),
                    4 => Matches4Case4(matchedCharacters),
                    5 => Matches5Case2(matchedCharacters),
                    _ => matchFound
                };
            }

            if (horizontalMatchCount + verticalMatchCount == 5)
            {
                matchFound = horizontalMatchCount switch
                {
                    1 => Matches4Case1(matchedCharacters),
                    2 => Matches3Case3(matchedCharacters),
                    3 => Matches3Case4(matchedCharacters),
                    4 => Matches4Case2(matchedCharacters),
                    _ => matchFound
                };
            }

            if (horizontalMatchCount + verticalMatchCount == 4)
            {
                matchFound = horizontalMatchCount switch
                {
                    1 => Matches3Case1(matchedCharacters),
                    3 => Matches3Case2(matchedCharacters),
                    _ => matchFound
                };
            }
            return matchFound;
        }

        // 이 메소드는 매치를 확인하는 동안 계속해서 매치가 발견되는지 확인합니다.
        // 매치가 발견되면 콤보 카운트를 증가시키고, 스왑이 발생하지 않았음을 표시한 후 캐릭터를 상승시킵니다.
        // 매치가 발견되지 않을 때까지 반복합니다.
        public IEnumerator CheckMatches()
        {
            do
            {
                foreach (var character in characterPool.UsePoolCharacterList().Where(IsMatched))
                {
                    if(!countManager.IsSwapOccurred)
                    {
                        countManager.IncrementComboCount();
                    }
                    countManager.IsSwapOccurred = false;
                    yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
                }
            }
            while (false);
        }

        // 이 메소드는 주어진 캐릭터 객체를 풀로 반환하는 기능을 수행합니다.
        private static void ReturnObject(GameObject character)
        {   
            CharacterPool.ReturnToPool(character);
        }
        // 강화 기능 OK
        private bool Matches3Case1(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[2]); 
                ReturnObject(matchedCharacters[3]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[1]);
            }
            else
            {
                ReturnObject(matchedCharacters[2]); 
                ReturnObject(matchedCharacters[3]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);

            }
            return true;
        }
        private bool Matches3Case2(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].transform.position.x.Equals(matchedCharacters[1].transform.position.x))
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                }
                else
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                }
                return true;
            }

            if (matchedCharacters[0].transform.position.x.Equals(matchedCharacters[2].transform.position.x))
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure) 
                { 
                    ReturnObject(matchedCharacters[1]); 
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[2]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[2]);
                }
                else
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                }
                return true;
            }

            if (matchedCharacters[0].transform.position.x.Equals(matchedCharacters[3].transform.position.x))
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[3]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[3]);
                }
                else
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                }
                return true;
            }
            return true;
        }
        private bool Matches3Case3(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[3]); 
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[2]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[2]);
            }
            else
            {
                ReturnObject(matchedCharacters[3]); 
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
            }
            return true;
        }
        private bool Matches3Case4(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[2]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[3]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[3]);
            }
            else
            {
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[2]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
            }
            return true;
        }
        private bool Matches4Case1(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].transform.position.y > matchedCharacters[2].transform.position.y && 
                matchedCharacters[0].transform.position.y < matchedCharacters[3].transform.position.y)
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                }
                else
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[4]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                }
                return true;
            }

            if (matchedCharacters[0].transform.position.y > matchedCharacters[3].transform.position.y &&
                matchedCharacters[0].transform.position.y < matchedCharacters[4].transform.position.y)
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[1]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[2]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[2]);
                }
                else
                {
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[4]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                }
                return true;
            }
            return true;
        }
        private bool Matches4Case2(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[2].transform.position.x > matchedCharacters[0].transform.position.x)
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[1]); 
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[4]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[4]);
                }
                else
                {
                    ReturnObject(matchedCharacters[1]); 
                    ReturnObject(matchedCharacters[3]); 
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                }
            }
            else
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[1]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[4]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[4]);
                }
                else
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                }
            }
            return true;
        }
        private bool Matches4Case3(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].transform.position.y > matchedCharacters[4].transform.position.y)
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[5]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[2]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[2]);
                }
                else
                {
                    ReturnObject(matchedCharacters[5]);
                    ReturnObject(matchedCharacters[4]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                }
            }
            else
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[5]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[4]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[2]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[2]);
                }
                else
                {
                    ReturnObject(matchedCharacters[5]);
                    ReturnObject(matchedCharacters[3]); 
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]); 
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                }
            }
            return true;
        }
        private bool Matches4Case4(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].transform.position.x < matchedCharacters[2].transform.position.x &&
                matchedCharacters[0].transform.position.x > matchedCharacters[1].transform.position.x)
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[4]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[4]);
                }
                else
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                }
            }
            else
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[4]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[4]);
                }
                else
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                }
            }
            return true;
        }
        private bool Matches5Case1(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[3]);
                ReturnObject(matchedCharacters[5]);
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[1]);
            }
            else
            {
                ReturnObject(matchedCharacters[3]);
                ReturnObject(matchedCharacters[5]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
            }
            return true;
        }
        private bool Matches5Case2(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[3]);
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[5]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[5]);
            }
            else
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
            }
            return true;
        }
        private bool Matches2X5Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[6]);
                ReturnObject(matchedCharacters[4]);
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[3]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[5]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[5]);
            }
            else
            {
                ReturnObject(matchedCharacters[6]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
            }
            return true;
        }
        private bool Matches5X2Case(IReadOnlyList<GameObject> matchedCharacters)
        {

            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[3]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[5]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[5]);
            }
            else
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
            }
            return true;
        }
        private bool Matches3X3Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[5]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[3]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[3]);
                matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[4]);
            }
            else
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[5]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]); 
            }
            return true;
        }
        private bool Matches3X4Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[3].transform.position.y > matchedCharacters[5].transform.position.y)
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[6]);
                    ReturnObject(matchedCharacters[5]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[4]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[4]);
                }
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[6]);
                ReturnObject(matchedCharacters[5]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
            }
            else
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[6]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[5]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[5]);
                }
                else
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[6]);
                    ReturnObject(matchedCharacters[4]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                }
            }
            return true;
        }
        private bool Matches4X3Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[2].transform.position.x < matchedCharacters[4].transform.position.x)
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[2]); 
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[6]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[5]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[5]);
                }
                else
                {
                    ReturnObject(matchedCharacters[2]); 
                    ReturnObject(matchedCharacters[3]); 
                    ReturnObject(matchedCharacters[6]); 
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                }
            }
            else
            {
                if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[6]);
                    ReturnObject(matchedCharacters[4]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[2]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[2]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[5]);
                    treasureManager.PendingTreasure.Enqueue(matchedCharacters[5]);
                }
                else
                {
                    ReturnObject(matchedCharacters[1]); 
                    ReturnObject(matchedCharacters[3]); 
                    ReturnObject(matchedCharacters[6]); 
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                }
            }
            return true;
        }
        private bool Matches3X5Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[7]);
                ReturnObject(matchedCharacters[5]); 
                ReturnObject(matchedCharacters[2]); 
                ReturnObject(matchedCharacters[6]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[1]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[3]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[3]);
            }
            else
            {
                ReturnObject(matchedCharacters[7]);
                ReturnObject(matchedCharacters[5]); 
                ReturnObject(matchedCharacters[2]); 
                matchedCharacters[6].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[6]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
            }
            return true;
        }
        private bool Matches5X3Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>()._type == CharacterBase.Type.treasure)
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                ReturnObject(matchedCharacters[7]);
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[3]);
                matchedCharacters[6].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[6]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[6]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[6]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                // treasureManager.EnqueueAndCheckTreasure(matchedCharacters[5]);
                treasureManager.PendingTreasure.Enqueue(matchedCharacters[5]);
            }
            else
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                ReturnObject(matchedCharacters[7]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[6].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[6]);
            }
            return true;
        }
    }
}