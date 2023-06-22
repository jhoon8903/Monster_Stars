using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;

namespace Script.PuzzleManagerGroup
{
    public sealed class MatchManager : MonoBehaviour
    {
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private CountManager countManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private EnforceManager enforceManager;
        [SerializeField] private SwipeManager swipeManager;
        private bool _isMatched = false;

        // 이 메소드는 주어진 캐릭터 객체가 매치되는지 확인하는 기능을 수행합니다. 매치 여부를 반환합니다.
        public bool IsMatched(GameObject swapCharacter)
        {
            var matchFound = false;
            var swapCharacterName = swapCharacter.GetComponent<CharacterBase>().CharacterName;
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
                            nextCharacter.GetComponent<CharacterBase>().CharacterName != swapCharacterName)
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

        public bool isMatchActivated = false;
        // 이 메소드는 매치를 확인하는 동안 계속해서 매치가 발견되는지 확인합니다.
        // 매치가 발견되면 콤보 카운트를 증가시키고, 스왑이 발생하지 않았음을 표시한 후 캐릭터를 상승시킵니다.
        // 매치가 발견되지 않을 때까지 반복합니다.
        public IEnumerator CheckMatches()
        {
            if (_isMatched) yield break;
            _isMatched = true;
            FindConsecutiveTilesInColumn(characterPool.SortPoolCharacterList());
            yield return new WaitForSeconds(0.2f);
            //var characterList = characterPool.SortPoolCharacterList().Where(IsMatched).ToList();
            //foreach (var character in characterList)
            //{
            //    if(!countManager.IsSwapOccurred)
            //    {
            //        countManager.IncrementComboCount();
            //    }
            //    countManager.IsSwapOccurred = false;
            //}
            //yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
            var characterList = characterPool.SortPoolCharacterList();
            int count = 0;
            isMatchActivated = false;
            foreach (GameObject character in FindConsecutiveCharacters(characterList))
            {
                yield return StartCoroutine(swipeManager.AllMatchesCheck(character));
                count++;
                if (count > 1)
                {
                    countManager.IncrementComboCount();
                    isMatchActivated = true;
                }
            }
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
            _isMatched = false;
        }

        public void StartCheckMatches()
        {
            //이상하게 이걸 한 번 실행해야 오류가 안납니다. 추후 수정 필요
            FindConsecutiveTilesInColumn(characterPool.SortPoolCharacterList());
            StartCoroutine(CheckMatches());
        }

        // 이 메소드는 주어진 캐릭터 객체를 풀로 반환하는 기능을 수행합니다.
        private void ReturnObject(GameObject character)
        {   
            CharacterPool.ReturnToPool(character);
        }
        // 강화 기능 OK
        private bool Matches3Case1(IReadOnlyList<GameObject> matchedCharacters)
        {
            if (matchedCharacters[0].GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                ReturnObject(matchedCharacters[2]); 
                ReturnObject(matchedCharacters[3]);
                matchedCharacters[1].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[1]);
                // commonRewardManager.EnqueueTreasure(matchedCharacters[1]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[1]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[2]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[3]);
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
                // commonRewardManager.EnqueueTreasure(matchedCharacters[2]);
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
                // commonRewardManager.EnqueueTreasure(matchedCharacters[3]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[1]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[2]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[4]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[4]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[2]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[2]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[4]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[4]);
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
                // commonRewardManager.EnqueueTreasure(matchedCharacters[5]);
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
                // commonRewardManager.EnqueueTreasure(matchedCharacters[5]);
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
                // commonRewardManager.EnqueueTreasure(matchedCharacters[5]);
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
                // commonRewardManager.EnqueueTreasure(matchedCharacters[3]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[3]);
                matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                // commonRewardManager.EnqueueTreasure(matchedCharacters[1]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[1]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    matchedCharacters[4].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[4]);
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[4]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[1]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[5]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[1]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[5]);
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
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[2]);
                    commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[2]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                    // commonRewardManager.EnqueueTreasure(matchedCharacters[5]);
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
                // commonRewardManager.EnqueueTreasure(matchedCharacters[1]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[1]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                matchedCharacters[3].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[3]);
                // commonRewardManager.EnqueueTreasure(matchedCharacters[3]);
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
                // commonRewardManager.EnqueueTreasure(matchedCharacters[6]);
                commonRewardManager.PendingTreasure.Enqueue(matchedCharacters[6]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                matchedCharacters[5].GetComponent<CharacterBase>().LevelUpScale(matchedCharacters[5]);
                // commonRewardManager.EnqueueTreasure(matchedCharacters[5]);
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
            return true;
        }
        List<List<GameObject>> FindConsecutiveTilesInRow(List<GameObject> characters)
        {
            List<List<GameObject>> result = new List<List<GameObject>>();
            int tempLevel = 0; // 캐릭터의 레벨 기준을 현재 인덱스와 비교하기위해 잠시 저장합니다
            CharacterBase.UnitGroups tempUnitGroup = CharacterBase.UnitGroups.None; // 캐릭터의 종류를 판단하기 위해 식별 인덱스를 잠시 저장
            int currentFloor = 1; // 현재 계산 중인 캐릭터가 몇 층인지를 판단
            int sameCount = 1; // 현재 인덱스까지 몇 개의 캐릭터가 동일하게 연결됐는지를 "갱신"

            for (int i = 0; i < characters.Count; i++)
            {
                if(i == characters.Count - 1)
                {
                    if (tempLevel == characters[i].GetComponent<CharacterBase>().Level && tempUnitGroup == characters[i].GetComponent<CharacterBase>().unitGroup)
                    {
                        sameCount++;
                        List<GameObject> currentList = new List<GameObject>();
                        for (int j = i - sameCount + 1; j <= i; j++)
                        {
                            currentList.Add(characters[j]);
                        }
                        result.Add(currentList);// 후보에 추가합니다
                    }
                    else
                    {
                        List<GameObject> currentList = new List<GameObject>();
                        for (int j = i - sameCount; j <= i - 1; j++)
                        {
                            currentList.Add(characters[j]);
                        }
                        result.Add(currentList);// 후보에 추가합니다
                    }
                }
                if ((i / 6) < currentFloor) // "6/6이 되기 전까진 1층 보다 낮다"꼴 의 흐름입니다.
                {
                    // 이전 인덱스의 정보와 동일하다?
                    if (tempLevel == characters[i].GetComponent<CharacterBase>().Level && tempUnitGroup == characters[i].GetComponent<CharacterBase>().unitGroup)
                    {
                        sameCount++;
                    }
                    else // 동일 식별자를 공유하는 캐릭터의 연속이 깨졌으므로 이전까지의 흐름을 저장합니다.
                    {
                        if (sameCount >= 3)//이전 층수까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                        {
                            List<GameObject> currentList = new List<GameObject>();
                            for (int j = i - (sameCount); j <= i-1; j++)
                            {
                                currentList.Add(characters[j]);
                            }
                            result.Add(currentList);// 후보에 추가합니다
                        }
                        // 다른 식별인자를 가진 캐릭터의 시작임으로 무조건 캐릭터 식별인자 갱신이 필요합니다
                        tempLevel = characters[i].GetComponent<CharacterBase>().Level;
                        // 새로운 층의 시작임으로 무조건 캐릭터 식별인자 갱신이 필요합니다
                        tempUnitGroup = characters[i].GetComponent<CharacterBase>().unitGroup;
                        sameCount = 1; // 동일 캐릭터의 배열길이는 1부터 시작
                    }
                }
                else // 층수가 바뀐 그 한 순간만 작동되는 코드입니다
                {
                    if(sameCount >= 3)//이전 층수까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                    {
                        List<GameObject> currentList = new List<GameObject>();
                        for (int j = i - sameCount; j <= i-1; j++)
                        {
                            currentList.Add(characters[j]);
                        }
                        result.Add(currentList);// 후보에 추가합니다
                    }
                    currentFloor++;// 층수가 바뀜을 적용합니다. 대상들의 y좌표 값이 +1 증가하는 시기임을 반영한 값입니다.
                    // 새로운 층의 시작임으로 무조건 캐릭터 식별인자 갱신이 필요합니다
                    tempLevel = characters[i].GetComponent<CharacterBase>().Level;
                    // 새로운 층의 시작임으로 무조건 캐릭터 식별인자 갱신이 필요합니다
                    tempUnitGroup = characters[i].GetComponent<CharacterBase>().unitGroup;
                    sameCount = 1;
                }
            }

            return result;
        }

        List<List<GameObject>> FindConsecutiveTilesInColumn(List<GameObject> characters)
        {
            // 결과를 저장할 List를 초기화합니다
            List<List<GameObject>> result = new List<List<GameObject>>();
            int tempLevel = 0; // 캐릭터의 레벨 기준을 현재 인덱스와 비교하기 위해 잠시 저장합니다
            CharacterBase.UnitGroups tempUnitGroup = CharacterBase.UnitGroups.None; // 캐릭터의 종류를 판단하기 위해 식별 인덱스를 잠시 저장
            int sameCount = 1; // 현재 인덱스까지 몇 개의 캐릭터가 동일하게 연결됐는지를 "갱신"

            int totalRows = characters.Count / 6;
            int totalColumns = 6;

            // 가로 방향으로 탐색(i가 x축이 됩니다)
            for (int i = 0; i < totalColumns; i++)
            {
                // 세로 방향으로 탐색
                for (int j = 0; j < totalRows; j++)
                {
                    int index = j * totalColumns + i;
                    //가장 마지막 인덱스인 경우 예외처리를 해줍니다
                    if(index == characters.Count - 1)
                    {
                        if (tempLevel == characters[index].GetComponent<CharacterBase>().Level && tempUnitGroup == characters[index].GetComponent<CharacterBase>().unitGroup)
                        {
                            sameCount++;
                            List<GameObject> currentList = new List<GameObject>();
                            for (int k = i + (totalColumns * (j - sameCount + 1)); k <= i + (totalColumns * j); k += totalColumns)
                            {
                                currentList.Add(characters[k]);
                            }
                            result.Add(currentList);
                        }
                        else
                        {
                            // 동일 식별자를 공유하는 캐릭터의 연속이 깨졌으므로 이전까지의 흐름을 저장합니다
                            if (sameCount >= 3) // 이전 열까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                            {
                                List<GameObject> currentList = new List<GameObject>();
                                for (int k = i + (totalColumns * (j - sameCount)); k <= i + (totalColumns * j - 1); k += totalColumns)
                                {
                                    currentList.Add(characters[k]);
                                }
                                result.Add(currentList);
                            }
                        }
                        break;
                    }
                    // j가 0일 때는 새롭게 1층, 즉 컬럼이 전환된 시점이므로 이전 열에 대한 처리를 무조건 적으로 검토합니다
                    if (j == 0)
                    {
                        // 이전까지의 마지막 요소 처리
                        if (sameCount >= 3)
                        {
                            List<GameObject> currentList = new List<GameObject>();
                            for (int k = (i-1) + (totalColumns * (totalRows - sameCount)); k <= (i-1) + (totalColumns * (totalRows - 1)); k += totalColumns)
                            {
                                currentList.Add(characters[k]);
                            }
                            result.Add(currentList);
                        } 

                        tempLevel = characters[index].GetComponent<CharacterBase>().Level;
                        tempUnitGroup = characters[index].GetComponent<CharacterBase>().unitGroup;
                        sameCount = 1; // 새로운 열로 이동한 후, 동일한 캐릭터의 수를 다시 1로 초기화합니다
                    }
                    else
                    {
                        // 이전 인덱스의 정보와 동일하다 (첫 번째 행 제외)
                        if (tempLevel == characters[index].GetComponent<CharacterBase>().Level && tempUnitGroup == characters[index].GetComponent<CharacterBase>().unitGroup)
                        {
                            sameCount++;
                        }
                        else
                        {
                            // 동일 식별자를 공유하는 캐릭터의 연속이 깨졌으므로 이전까지의 흐름을 저장합니다
                            if (sameCount >= 3) // 이전 열까지 누적된 동일 캐릭터의 연결이 3개 이상인지 확인합니다
                            {
                                List<GameObject> currentList = new List<GameObject>();
                                for (int k = i + (totalColumns * (j - sameCount)); k <= i + (totalColumns * j-1); k += totalColumns)
                                {
                                    currentList.Add(characters[k]);
                                }
                                result.Add(currentList);
                            }

                            tempLevel = characters[index].GetComponent<CharacterBase>().Level;
                            tempUnitGroup = characters[index].GetComponent<CharacterBase>().unitGroup;
                            sameCount = 1; // 동일 캐릭터 배열 길이는 1부터 시작
                        }
                    }
                }
            }
            return result;
        }

        struct MatchedPair
        {
            public GameObject gameObject;
            public List<GameObject> rowList;
            public List<GameObject> columnList;
        }

        List<GameObject> FindMatchingGameObjects(List<List<GameObject>> rowResults, List<List<GameObject>> columnResults)
        {
            List<GameObject> matchingGameObjects = new List<GameObject>();
            List<MatchedPair> matchedPairs = new List<MatchedPair>();

            foreach (List<GameObject> rowList in rowResults)
            {
                foreach (List<GameObject> columnList in columnResults)
                {
                    foreach (GameObject rowObject in rowList)
                    {
                        foreach (GameObject columnObject in columnList)
                        {
                            if (rowObject.transform.position.x == columnObject.transform.position.x &&
                                rowObject.transform.position.y == columnObject.transform.position.y)
                            {
                                matchedPairs.Add(new MatchedPair { gameObject = rowObject, rowList = rowList, columnList = columnList });
                                break;
                            }
                        }
                    }
                }
            }

            // 처리하지 못한 rowList 인덱스 구함
            foreach (List<GameObject> rowList in rowResults)
            {
                bool foundMatchedPair = false;

                //기존에 후보명단에 올라있는지 검토
                foreach (MatchedPair matchedPair in matchedPairs)
                {
                    if (matchedPair.rowList == rowList)
                    {
                        foundMatchedPair = true;
                        break;
                    }
                }

                if (!foundMatchedPair)
                {
                    int index = Mathf.FloorToInt(rowList.Count / 2f);//정해진 후보들 {a, b, c}읠 길이3에 대해 평균값을 구한다
                    GameObject gameObject = rowList[index];//평균값이 1라면 {a,b,c}에서 b를 구한다
                    if (!matchingGameObjects.Contains(gameObject))
                    {
                        matchingGameObjects.Add(gameObject);
                    }
                }
            }

            // 처리하지 못한 columnList의 인덱스 구함
            foreach (List<GameObject> columnList in columnResults)
            {
                bool foundMatchedPair = false;

                foreach (MatchedPair matchedPair in matchedPairs)
                {
                    if (matchedPair.columnList == columnList)
                    {
                        foundMatchedPair = true;
                        break;
                    }
                }

                if (!foundMatchedPair)
                {
                    int tempindex = 0;
                    foreach (GameObject item in columnList)
                    {
                        tempindex++;
                    }
                    int index = Mathf.FloorToInt(columnList.Count / 2f);
                    GameObject gameObject = columnList[index];
                    if (!matchingGameObjects.Contains(gameObject))
                    {
                        matchingGameObjects.Add(gameObject);
                    }
                }
            }

            // 공통 GameObject를 포함하는 matchedPairs에서 GameObject를 추가합니다.
            foreach (MatchedPair matchedPair in matchedPairs)
            {
                if (!matchingGameObjects.Contains(matchedPair.gameObject))
                {
                    matchingGameObjects.Add(matchedPair.gameObject);
                }
            }

            return matchingGameObjects;
        }


        List<GameObject> FindConsecutiveCharacters(List<GameObject> characters)
        {
            // For each value, find the consecutive tiles
                var rowsResult = FindConsecutiveTilesInRow(characters);
                var colsResult = FindConsecutiveTilesInColumn(characters);

            return FindMatchingGameObjects(rowsResult, colsResult);
        }

        // 선별된 GameObject character
        private IEnumerator AllMatchesCheck(List<GameObject> characters)
        {
            foreach(GameObject character in characters)
            {
                yield return StartCoroutine(swipeManager.AllMatchesCheck(character));
            }
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
        }
    }
}