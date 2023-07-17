using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;

namespace Script.PuzzleManagerGroup
{

    public sealed class MatchManager : MonoBehaviour
    {
        private struct MatchedPair
        {
            public GameObject gameObject;
            public List<GameObject> rowList;
            public List<GameObject> columnList;
        }

        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private CountManager countManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private EnforceManager enforceManager;
        [SerializeField] private SwipeManager swipeManager;
        private bool _isMatched;
        public bool isMatchActivated;

        public bool IsMatched(GameObject swapCharacter)
        {
            var matchFound = false;
            var swapCharacterGroup = swapCharacter.GetComponent<CharacterBase>().unitGroup;
            var swapCharPuzzleLevel = swapCharacter.GetComponent<CharacterBase>().unitPuzzleLevel;
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
                            (nextCharacter.GetComponent<CharacterBase>().unitGroup != swapCharacterGroup
                             || nextCharacter.GetComponent<CharacterBase>().unitPuzzleLevel != swapCharPuzzleLevel))
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
        public IEnumerator CheckMatches()
        {
            if (_isMatched) yield break;
            _isMatched = true;
            FindConsecutiveTilesInColumn(characterPool.SortPoolCharacterList());
            yield return new WaitForSeconds(0.2f);

            var characterList = characterPool.SortPoolCharacterList();
            var count = 0;
            isMatchActivated = false;
            foreach (var character in FindConsecutiveCharacters(characterList))
            {
                yield return StartCoroutine(swipeManager.AllMatchesCheck(character));
                count++;
                if (count <= 1) continue;
                countManager.IncrementComboCount();
                isMatchActivated = true;
            }
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
            _isMatched = false;
        }
        private static void ReturnObject(GameObject character)
        {   
            CharacterPool.ReturnToPool(character);
        }
        private bool Matches3Case1(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[2]); 
                ReturnObject(matchedCharacters[3]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure) 
                { 
                    ReturnObject(matchedCharacters[1]); 
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[2]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[3]);
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
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[3]); 
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[2]);
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
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[2]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[3]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[1]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[2]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[1]); 
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[4]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[1]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[4]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[5]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[2]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[5]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[4]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[2]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[4]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[2]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[4]);
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
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[3]);
                ReturnObject(matchedCharacters[5]);
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                if (EnforceManager.Instance.addGold)
                {
                    EnforceManager.Instance.AddGold();
                }
            }
            else
            {
                ReturnObject(matchedCharacters[3]);
                ReturnObject(matchedCharacters[5]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                if (enforceManager.match5Upgrade)
                { 
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                }
            }
            if (EnforceManager.Instance.addGold)
            {
                EnforceManager.Instance.AddGold();
            }
            return true;
        }
        private bool Matches5Case2(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[3]);
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[5]);
            }
            else
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                if (enforceManager.match5Upgrade)
                {
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                }
            }
            if (EnforceManager.Instance.addGold)
            {
                EnforceManager.Instance.AddGold();
            }
            return true;
        }
        private bool Matches2X5Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[6]);
                ReturnObject(matchedCharacters[4]);
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[3]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[5]);
            }
            else
            {
                ReturnObject(matchedCharacters[6]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                if (enforceManager.match5Upgrade)
                {
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                }
            }
            if (EnforceManager.Instance.addGold)
            {
                EnforceManager.Instance.AddGold();
            }
            return true;
        }
        private bool Matches5X2Case(IReadOnlyList<GameObject> matchedCharacters)
        {

            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[3]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[5]);
            }
            else
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                if (enforceManager.match5Upgrade)
                {
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                }
            }
            if (EnforceManager.Instance.addGold)
            {
                EnforceManager.Instance.AddGold();
            }
            return true;
        }
        private bool Matches3X3Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[5]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[3]);
                matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[4]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[6]);
                    ReturnObject(matchedCharacters[5]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[4]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[2]);
                    ReturnObject(matchedCharacters[6]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[3]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[5]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[2]); 
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[4]);
                    ReturnObject(matchedCharacters[6]);
                    matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[5]);
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
                if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    ReturnObject(matchedCharacters[1]);
                    ReturnObject(matchedCharacters[3]);
                    ReturnObject(matchedCharacters[6]);
                    ReturnObject(matchedCharacters[4]);
                    matchedCharacters[2].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[2]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[2]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[5]);
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
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[7]);
                ReturnObject(matchedCharacters[5]); 
                ReturnObject(matchedCharacters[2]); 
                ReturnObject(matchedCharacters[6]);
                ReturnObject(matchedCharacters[4]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[3]);
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
                if (enforceManager.match5Upgrade)
                {
                    matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                }
            }
            if (EnforceManager.Instance.addGold)
            {
                EnforceManager.Instance.AddGold();
            }
            return true;
        }
        private bool Matches5X3Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[2]);
                ReturnObject(matchedCharacters[4]);
                ReturnObject(matchedCharacters[7]);
                ReturnObject(matchedCharacters[1]);
                ReturnObject(matchedCharacters[3]);
                matchedCharacters[6].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[6]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[6]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[5]);
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
                if (enforceManager.match5Upgrade)
                {
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                }
            }
            if (EnforceManager.Instance.addGold)
            {
                EnforceManager.Instance.AddGold();
            }
            return true;
        }
        private static List<List<GameObject>> FindConsecutiveTilesInRow(IReadOnlyList<GameObject> characters)
        {
            var result = new List<List<GameObject>>();
            var tempLevel = 0; // 캐릭터의 레벨 기준을 현재 인덱스와 비교하기위해 잠시 저장합니다
            var tempUnitGroup = CharacterBase.UnitGroups.None; // 캐릭터의 종류를 판단하기 위해 식별 인덱스를 잠시 저장
            var currentFloor = 1; // 현재 계산 중인 캐릭터가 몇 층인지를 판단
            var sameCount = 1; // 현재 인덱스까지 몇 개의 캐릭터가 동일하게 연결됐는지를 "갱신"

            for (var i = 0; i < characters.Count; i++)
            {
                if(i == characters.Count - 1)
                {
                    if (tempLevel == characters[i].GetComponent<CharacterBase>().unitPuzzleLevel 
                        && tempUnitGroup == characters[i].GetComponent<CharacterBase>().unitGroup)
                    {
                        sameCount++;
                        var currentList = new List<GameObject>();
                        for (var j = i - sameCount + 1; j <= i; j++)
                        {
                            currentList.Add(characters[j]);
                        }
                        result.Add(currentList);// 후보에 추가합니다
                    }
                    else
                    {
                        var currentList = new List<GameObject>();
                        for (var j = i - sameCount; j <= i - 1; j++)
                        {
                            currentList.Add(characters[j]);
                        }
                        result.Add(currentList);// 후보에 추가합니다
                    }
                }
                if ((i / 6) < currentFloor) // "6/6이 되기 전까진 1층 보다 낮다"꼴 의 흐름입니다.
                {
                    // 이전 인덱스의 정보와 동일하다?
                    if (tempLevel == characters[i].GetComponent<CharacterBase>().unitPuzzleLevel 
                        && tempUnitGroup == characters[i].GetComponent<CharacterBase>().unitGroup)
                    {
                        sameCount++;
                    }
                    else // 동일 식별자를 공유하는 캐릭터의 연속이 깨졌으므로 이전까지의 흐름을 저장합니다.
                    {
                        if (sameCount >= 3)//이전 층수까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                        {
                            var currentList = new List<GameObject>();
                            for (var j = i - (sameCount); j <= i-1; j++)
                            {
                                currentList.Add(characters[j]);
                            }
                            result.Add(currentList);// 후보에 추가합니다
                        }
                        // 다른 식별인자를 가진 캐릭터의 시작임으로 무조건 캐릭터 식별인자 갱신이 필요합니다
                        tempLevel = characters[i].GetComponent<CharacterBase>().unitPuzzleLevel;
                        // 새로운 층의 시작임으로 무조건 캐릭터 식별인자 갱신이 필요합니다
                        tempUnitGroup = characters[i].GetComponent<CharacterBase>().unitGroup;
                        sameCount = 1; // 동일 캐릭터의 배열길이는 1부터 시작
                    }
                }
                else // 층수가 바뀐 그 한 순간만 작동되는 코드입니다
                {
                    if(sameCount >= 3)//이전 층수까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                    {
                        var currentList = new List<GameObject>();
                        for (var j = i - sameCount; j <= i-1; j++)
                        {
                            currentList.Add(characters[j]);
                        }
                        result.Add(currentList);// 후보에 추가합니다
                    }
                    currentFloor++;// 층수가 바뀜을 적용합니다. 대상들의 y좌표 값이 +1 증가하는 시기임을 반영한 값입니다.
                    // 새로운 층의 시작임으로 무조건 캐릭터 식별인자 갱신이 필요합니다
                    tempLevel = characters[i].GetComponent<CharacterBase>().unitPuzzleLevel;
                    // 새로운 층의 시작임으로 무조건 캐릭터 식별인자 갱신이 필요합니다
                    tempUnitGroup = characters[i].GetComponent<CharacterBase>().unitGroup;
                    sameCount = 1;
                }
            }
            return result;
        }
        private static List<List<GameObject>> FindConsecutiveTilesInColumn(IReadOnlyList<GameObject> characters)
        {
            // 결과를 저장할 List를 초기화합니다
            var result = new List<List<GameObject>>();
            var tempLevel = 0; // 캐릭터의 레벨 기준을 현재 인덱스와 비교하기 위해 잠시 저장합니다
            var tempUnitGroup = CharacterBase.UnitGroups.None; // 캐릭터의 종류를 판단하기 위해 식별 인덱스를 잠시 저장
            var sameCount = 1; // 현재 인덱스까지 몇 개의 캐릭터가 동일하게 연결됐는지를 "갱신"
            var totalRows = characters.Count / 6;
            const int totalColumns = 6;

            // 가로 방향으로 탐색(i가 x축이 됩니다)
            for (var i = 0; i < totalColumns; i++)
            {
                // 세로 방향으로 탐색
                for (var j = 0; j < totalRows; j++)
                {
                    var index = j * totalColumns + i;
                    //가장 마지막 인덱스인 경우 예외처리를 해줍니다
                    if(index == characters.Count - 1)
                    {
                        if (tempLevel == characters[index].GetComponent<CharacterBase>().unitPuzzleLevel && tempUnitGroup == characters[index].GetComponent<CharacterBase>().unitGroup)
                        {
                            sameCount++;
                            if (sameCount >= 3) // 방금 위에서 ++한 값을 포함해서 이번 열까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                            {
                                var currentList = new List<GameObject>();
                                for (var k = i + (totalColumns * (j - sameCount)); k <= i + (totalColumns * j - 1); k += totalColumns)
                                {
                                    currentList.Add(characters[k]);
                                }
                                result.Add(currentList);
                            }
                        }
                        else
                        {
                            // 동일 식별자를 공유하는 캐릭터의 연속이 깨졌으므로 이전까지의 흐름을 저장합니다
                            if (sameCount >= 3) // 이전 열까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                            {
                                var currentList = new List<GameObject>();
                                for (var k = i + (totalColumns * (j - sameCount)); k <= i + (totalColumns * j - 1); k += totalColumns)
                                {
                                    currentList.Add(characters[k]);
                                }
                                result.Add(currentList);
                            }
                        }
                        //이전 요소에 대한 처리로 넘어가지 않도록 break로 끊어줍니다. 따라서 Log를 여기서만 따로 발생시킵니다.
                        break;
                    }
                    // j가 0일 때는 새롭게 1층, 즉 컬럼이 전환된 시점이므로 이전 열에 대한 처리를 무조건 적으로 검토합니다
                    if (j == 0)
                    {
                        // 이전까지의 마지막 요소 처리
                        if (sameCount >= 3)
                        {
                            var currentList = new List<GameObject>();
                            for (var k = (i-1) + (totalColumns * (totalRows - sameCount)); k <= (i-1) + (totalColumns * (totalRows - 1)); k += totalColumns)
                            {
                                currentList.Add(characters[k]);
                            }
                            result.Add(currentList);
                        }
                        tempLevel = characters[index].GetComponent<CharacterBase>().unitPuzzleLevel;
                        tempUnitGroup = characters[index].GetComponent<CharacterBase>().unitGroup;
                        sameCount = 1; // 새로운 열로 이동한 후, 동일한 캐릭터의 수를 다시 1로 초기화합니다
                    }
                    else
                    {
                        // 이전 인덱스의 정보와 동일하다 (첫 번째 행 제외)
                        if (tempLevel == characters[index].GetComponent<CharacterBase>().unitPuzzleLevel && tempUnitGroup == characters[index].GetComponent<CharacterBase>().unitGroup)
                        {
                            sameCount++;
                        }
                        else
                        {
                            // 동일 식별자를 공유하는 캐릭터의 연속이 깨졌으므로 이전까지의 흐름을 저장합니다
                            if (sameCount >= 3) // 이전 열까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                            {
                                var currentList = new List<GameObject>();
                                for (var k = i + (totalColumns * (j - sameCount)); k <= i + (totalColumns * j-1); k += totalColumns)
                                {
                                    currentList.Add(characters[k]);
                                }
                                result.Add(currentList);
                            }
                            tempLevel = characters[index].GetComponent<CharacterBase>().unitPuzzleLevel;
                            tempUnitGroup = characters[index].GetComponent<CharacterBase>().unitGroup;
                            sameCount = 1; // 동일 캐릭터 배열 길이는 1부터 시작
                        }
                    }
                }
            }
            return result;
        }
        private static List<GameObject> FindMatchingGameObjects(List<List<GameObject>> rowResults, List<List<GameObject>> columnResults)
        {
            var matchingGameObjects = new List<GameObject>();
            var matchedPairs = new List<MatchedPair>();

            foreach (var rowList in rowResults)
            {
                foreach (var columnList in columnResults)
                {
                    matchedPairs.AddRange(from rowObject in rowList where columnList
                        .Any(columnObject =>
                        {
                            Vector3 position;
                            var position1 = rowObject.transform.position;
                            return position1.x.Equals((position = columnObject.transform.position).x ) &&
                                   position1.y.Equals(position.y);
                        }) 
                        select new MatchedPair { gameObject = rowObject, rowList = rowList, columnList = columnList });
                }
            }

            // 처리하지 못한 rowList 인덱스 구함
            foreach (var gameObject in from rowList in rowResults 
                     let foundMatchedPair = matchedPairs.Any(matchedPair => matchedPair.rowList == rowList) 
                     where !foundMatchedPair 
                     let index = Mathf.FloorToInt(rowList.Count / 2f) 
                     select rowList[index] 
                     into gameObject 
                     where !matchingGameObjects.Contains(gameObject) 
                     select gameObject)
            {
                matchingGameObjects.Add(gameObject);
            }

            // 처리하지 못한 columnList의 인덱스 구함
            foreach (var gameObject in from columnList in columnResults 
                     let foundMatchedPair = matchedPairs.Any(matchedPair => matchedPair.columnList == columnList) 
                     where !foundMatchedPair 
                     let tempIndex = columnList.Count() 
                     let index = Mathf.FloorToInt(columnList.Count / 2f) 
                     select columnList[index] 
                     into gameObject 
                     where !matchingGameObjects.Contains(gameObject) 
                     select gameObject)
            {
                matchingGameObjects.Add(gameObject);
            }

            // 공통 GameObject를 포함하는 matchedPairs에서 GameObject를 추가합니다.
            foreach (var matchedPair in matchedPairs.Where(matchedPair => !matchingGameObjects.Contains(matchedPair.gameObject)))
            {
                matchingGameObjects.Add(matchedPair.gameObject);
            }
            return matchingGameObjects;
        }
        private static List<GameObject> FindConsecutiveCharacters(IReadOnlyList<GameObject> characters)
        {
            // For each value, find the consecutive tiles
                var rowsResult = FindConsecutiveTilesInRow(characters);
                var colsResult = FindConsecutiveTilesInColumn(characters);
                return FindMatchingGameObjects(rowsResult, colsResult);
        }
    }
}