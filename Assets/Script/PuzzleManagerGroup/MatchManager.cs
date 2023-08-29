using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Utilities;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.PuzzleManagerGroup
{
    public sealed class MatchManager : MonoBehaviour
    {
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private EnforceManager enforceManager;
        [SerializeField] private GridManager gridManager;
        
        public event Action OnMatchCheckComplete;
        public bool IsMatched(GameObject swapCharacter)
        {
            var matched = false;
            var swapCharacterBase = swapCharacter.GetComponent<CharacterBase>();
            var swapCharacterGroup = swapCharacterBase.unitGroup;
            var swapCharPuzzleLevel = swapCharacterBase.unitPuzzleLevel;
            switch (swapCharacterBase.UnitGrade)
            {
                case CharacterBase.UnitGrades.G when swapCharPuzzleLevel == 5:
                case CharacterBase.UnitGrades.B when swapCharPuzzleLevel == 6:
                case CharacterBase.UnitGrades.P when swapCharPuzzleLevel == 7:
                    return false;
            }
            var swapCharacterPosition = swapCharacterBase.transform.position;
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
                        var nextCharacter = SpawnManager.CharacterObject(nextPosition);
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
                matchedCharacters.AddRange(matchedObjects);
            }

            if (horizontalMatchCount + verticalMatchCount == 8)
            {
                matched = horizontalMatchCount switch
                {
                    3 => Matches3X5Case(matchedCharacters),
                    5 => Matches5X3Case(matchedCharacters),
                    _ => false
                };
            }

            if (horizontalMatchCount + verticalMatchCount == 7)
            {
                matched = horizontalMatchCount switch
                {
                    2 => Matches2X5Case(matchedCharacters),
                    3 => Matches3X4Case(matchedCharacters),
                    4 => Matches4X3Case(matchedCharacters),
                    5 => Matches5X2Case(matchedCharacters),
                    _ => false
                };
            }

            if (horizontalMatchCount + verticalMatchCount == 6)
            {
                matched = horizontalMatchCount switch
                {
                    1 => Matches5Case1(matchedCharacters),
                    2 => Matches4Case3(matchedCharacters),
                    3 => Matches3X3Case(matchedCharacters),
                    4 => Matches4Case4(matchedCharacters),
                    5 => Matches5Case2(matchedCharacters),
                    _ => false
                };
            }

            if (horizontalMatchCount + verticalMatchCount == 5)
            {
                matched = horizontalMatchCount switch
                {
                    1 => Matches4Case1(matchedCharacters),
                    2 => Matches3Case3(matchedCharacters),
                    3 => Matches3Case4(matchedCharacters),
                    4 => Matches4Case2(matchedCharacters),
                    _ => false
                };
            }

            if (horizontalMatchCount + verticalMatchCount == 4)
            {
               matched = horizontalMatchCount switch
                {
                    1 => Matches3Case1(matchedCharacters),
                    3 => Matches3Case2(matchedCharacters),
                    _ => false
                };
            }
            return matched;
        }
        public IEnumerator CheckMatches()
        {
            var characterList = CharacterPool.Instance.UsePoolCharacterList();
            var count = 0;
            foreach (var character in characterList)
            {
                if (IsMatched(character))
                {
                    count++;
                    if (count >= 1)
                    {
                        countManager.IncrementComboCount();
                    }
                    yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
                }
                yield return null;
            }
            OnMatchCheckComplete?.Invoke();
            yield return null;
        }
        private static void ReturnObject(GameObject character)
        {   
            CharacterPool.ReturnToPool(character);
        }
        private bool Matches3Case1(IReadOnlyList<GameObject> matchedCharacters)
        {        
            SoundManager.Instance.MatchSound(3);
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
            SoundManager.Instance.MatchSound(3);
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
            SoundManager.Instance.MatchSound(3);
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
            SoundManager.Instance.MatchSound(3);
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
            SoundManager.Instance.MatchSound(4);
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
            SoundManager.Instance.MatchSound(4);
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
            SoundManager.Instance.MatchSound(4);
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
            SoundManager.Instance.MatchSound(4);
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
            SoundManager.Instance.MatchSound(5);
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
                    EnforceManager.Instance.addGoldCount++;
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
                EnforceManager.Instance.addGoldCount++;
            }
            return true;
        }
        private bool Matches5Case2(IReadOnlyList<GameObject> matchedCharacters)
        {
            SoundManager.Instance.MatchSound(5);
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
                EnforceManager.Instance.addGoldCount++;
            }
            return true;
        }
        private bool Matches2X5Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            SoundManager.Instance.MatchSound(5);
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
                EnforceManager.Instance.addGoldCount++;
            }
            return true;
        }
        private bool Matches5X2Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            SoundManager.Instance.MatchSound(5);
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
                EnforceManager.Instance.addGoldCount++;
            }
            return true;
        }
        private bool Matches3X3Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            SoundManager.Instance.MatchSound(3);
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
            SoundManager.Instance.MatchSound(4);
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
            SoundManager.Instance.MatchSound(4);
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
            SoundManager.Instance.MatchSound(5);
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
                EnforceManager.Instance.addGoldCount++;
            }
            return true;
        }
        private bool Matches5X3Case(IReadOnlyList<GameObject> matchedCharacters)
        {
            SoundManager.Instance.MatchSound(5);
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
                EnforceManager.Instance.addGoldCount++;
            }
            return true;
        }
    }
}