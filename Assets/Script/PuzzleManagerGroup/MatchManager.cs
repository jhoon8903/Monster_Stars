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
        [SerializeField] private CountManager countManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private EnforceManager enforceManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private MergeEffect mergePrefab;
        [SerializeField] private SwipeManager swipeManager;
        public List<MergeEffect> mergeEffectList = new List<MergeEffect>();

        private void Awake()
        {
            for (var i = 0; i < 20; i++)
            {
                var mergeEffect = Instantiate(mergePrefab, transform);
                mergeEffect.gameObject.SetActive(false);
                mergeEffectList.Add(mergeEffect);
            }
        }
        public bool IsMatched(GameObject swapCharacter)
        {
            swipeManager.isBusy = true;
            var matched = false;
            var swapCharacterBase = swapCharacter.GetComponent<CharacterBase>();
            var swapCharacterGroup = swapCharacterBase.unitGroup;
            var swapCharPuzzleLevel = swapCharacterBase.unitPuzzleLevel;
            var swapCharacterPosition = swapCharacter.transform.position;
            var horizontalMatchCount = 0;
            var verticalMatchCount = 0;
            var matchedCharacters = new List<GameObject>();
            switch (swapCharacterBase.UnitGrade)
            {
                case CharacterBase.UnitGrades.G when swapCharPuzzleLevel == 5:
                case CharacterBase.UnitGrades.B when swapCharPuzzleLevel == 6:
                case CharacterBase.UnitGrades.P when swapCharPuzzleLevel == 7:
                    return false;
            }
            var directions = new[]
            {
                (Vector3Int.left, Vector3Int.right, "Horizontal"),
                (Vector3Int.up, Vector3Int.down, "Vertical")
            };
            foreach (var (dir1, dir2, dirName) in directions)
            {
                var matchCount = 1;
                var matchedObjects = new List<GameObject>()
                {
                    swapCharacter
                };
                foreach (var dir in new[] { dir1, dir2 })
                {
                    if (!swapCharacter.activeInHierarchy) continue;
                    var nextPosition = swapCharacterPosition + dir;
                    for (var i = 0; i < 4; i++)
                    {
                        var nextCharacter = SpawnManager.CharacterObject(nextPosition);
                        if (nextCharacter == null 
                            || nextCharacter.GetComponent<CharacterBase>().unitGroup != swapCharacterGroup 
                            || nextCharacter.GetComponent<CharacterBase>().unitPuzzleLevel != swapCharPuzzleLevel)
                            break;
                        matchedObjects.Add(nextCharacter);
                        matchCount++;
                        nextPosition += dir;
                    }
                }

                if (dirName == "Horizontal")
                {
                    horizontalMatchCount += matchCount;
                }
                else
                {
                    verticalMatchCount += matchCount;
                }
                matchedCharacters.AddRange(matchedObjects);

                matched = (horizontalMatchCount + verticalMatchCount) switch
                {
                    8 => horizontalMatchCount switch
                    {
                        3 or 5 => Matches3X5Case(matchedCharacters, swapCharacterPosition),
                        _ => false
                    },
                    7 => horizontalMatchCount switch
                    {
                        2 => Matches5Case(matchedCharacters, "y", swapCharacterPosition),
                        3 or 4 => Matches3X4Case(matchedCharacters, swapCharacterPosition),
                        5 => Matches5Case(matchedCharacters, "x", swapCharacterPosition),
                        _ => false
                    },
                    6 => horizontalMatchCount switch
                    {
                        1 => Matches5Case(matchedCharacters, "y", swapCharacterPosition),
                        2 => Matches4Case(matchedCharacters, "y"),
                        3 => Matches3X3Case(matchedCharacters, swapCharacterPosition),
                        4 => Matches4Case(matchedCharacters, "x"),
                        5 => Matches5Case(matchedCharacters, "x", swapCharacterPosition),
                        _ => false
                    },
                    5 => horizontalMatchCount switch
                    {
                        1 => Matches4Case(matchedCharacters, "y"),
                        2 or 3 => Match3Case(matchedCharacters, horizontalMatchCount, verticalMatchCount,
                            swapCharacterPosition),
                        4 => Matches4Case(matchedCharacters, "x"),
                        _ => false
                    },
                    4 => horizontalMatchCount switch
                    {
                        1 or 3 => Match3Case(matchedCharacters, horizontalMatchCount, verticalMatchCount,
                            swapCharacterPosition),
                        _ => false
                    },
                    _ => false
                };
            }

            if (!matched || GameManager.Instance.IsBattle) return matched;
            spawnManager.count++;
            if ( spawnManager.count > 1)
            {
                countManager.IncrementComboCount();
            }
            return matched;
        }
        private static void ReturnObject(GameObject character)
        {
            CharacterPool.ReturnToPool(character);
        }
        private void MergeEffect(GameObject target)
        {
            var mergeEffect = mergeEffectList.FirstOrDefault(effect => !effect.gameObject.activeInHierarchy);
            if (mergeEffect == null) return;
            mergeEffect.gameObject.SetActive(true);
            mergeEffect.transform.position = target.transform.position;
            mergeEffect.MergeAction();
            mergeEffect.MergeActionClose();
        }
        private void ReturnEffect(GameObject target)
        {
            var returnEffect = mergeEffectList.FirstOrDefault(effect => !effect.gameObject.activeInHierarchy);
            if (returnEffect == null) return;
            returnEffect.gameObject.SetActive(true);
            returnEffect.transform.position = target.transform.position;
            returnEffect.ReturnAction();
            returnEffect.ReturnActionClose();
        }
        private bool Match3Case(IEnumerable<GameObject> rawMatchedCharacters, int horizontalCount, int verticalCount, Vector3 swapPosition)
        {
            SoundManager.Instance.MatchSound(3);
            var matchedCharacters = rawMatchedCharacters as GameObject[] ?? rawMatchedCharacters.ToArray();
            var groupedByX = matchedCharacters.GroupBy(mc => (int)mc.transform.position.x).ToList();
            var groupedByY = matchedCharacters.GroupBy(mc => (int)mc.transform.position.y).ToList();
            var validCharacters = new List<GameObject>();
            IEnumerable<IGrouping<int, GameObject>> selectedGroups = horizontalCount > verticalCount ? groupedByY : groupedByX;
            var largestGroup = selectedGroups.OrderByDescending(group => group.Count()).FirstOrDefault();
            if (largestGroup != null)
            {
                validCharacters.AddRange(largestGroup);
            }
            var uniqueList = validCharacters.Distinct().ToList();
            foreach (var validCharacter in uniqueList)
            {
                var characterBase = validCharacter.GetComponent<CharacterBase>();
                var isTreasure = characterBase.Type == CharacterBase.Types.Treasure;
                var isCenter = validCharacter.transform.position == swapPosition;
                if (isCenter)
                {
                    characterBase.LevelUpScale(validCharacter);
                    if (isTreasure)
                    {
                        commonRewardManager.PendingTreasure.Enqueue(validCharacter);
                    }
                    IsMatched(validCharacter); 
                    MergeEffect(validCharacter);
                }
                else
                {
                    ReturnObject(validCharacter);
                    ReturnEffect(validCharacter);
                }
            }
            return true;
        }
        private bool Matches4Case(IReadOnlyList<GameObject> matchedCharacters, string dir)
        {
            SoundManager.Instance.MatchSound(4);
            var grouped = matchedCharacters.GroupBy(mc => 
                dir == "x" ? mc.transform.position.x : mc.transform.position.y
            );
            var uniqueList = grouped.Select(g => g.First()).ToList();

            var sortedList = uniqueList.OrderBy(mc => 
                dir == "x" ? mc.transform.position.x : mc.transform.position.y
            ).ToList();
            var hasLeveledUpNonTreasure = false;
            foreach (var matchedCharacter in sortedList)
            {
                var characterBase = matchedCharacter.GetComponent<CharacterBase>();
                var isTreasure = characterBase.Type == CharacterBase.Types.Treasure;
                var isCenter = matchedCharacter.transform.position == matchedCharacters[0].transform.position;
                if (isTreasure)
                {
                    if (isCenter)
                    {
                        if (hasLeveledUpNonTreasure) continue;
                        characterBase.LevelUpScale(matchedCharacter);
                        characterBase.LevelUpScale(matchedCharacter);
                        commonRewardManager.PendingTreasure.Enqueue(matchedCharacter);
                        hasLeveledUpNonTreasure = true;
                        IsMatched(matchedCharacter);
                        MergeEffect(matchedCharacter);
                    }
                    else
                    {
                        ReturnObject(matchedCharacter);
                        ReturnEffect(matchedCharacter);
                    }
                }
                else
                {
                    if (hasLeveledUpNonTreasure) continue;
                    sortedList[1].GetComponent<CharacterBase>().LevelUpScale(sortedList[1]);
                    MergeEffect(sortedList[1]);
                    sortedList[2].GetComponent<CharacterBase>().LevelUpScale(sortedList[2]);
                    MergeEffect(sortedList[2]);
                    ReturnObject(sortedList[0]);
                    ReturnEffect(sortedList[0]);
                    ReturnObject(sortedList[3]);
                    ReturnEffect(sortedList[3]);
                    hasLeveledUpNonTreasure = true;
                    IsMatched(sortedList[1]);
                    IsMatched(sortedList[2]);
                }
            }
            return true;
        }
        private bool Matches5Case(IEnumerable<GameObject> matchedCharacters, string dir, Vector3 swapPosition)
        {
            SoundManager.Instance.MatchSound(5);
            var grouped = matchedCharacters.GroupBy(mc => 
                dir == "x" ? mc.transform.position.x : mc.transform.position.y);
            var uniqueList = grouped.Select(g => g.First()).ToList();
            var sortedList = uniqueList.OrderBy(mc => 
                dir == "x" ? mc.transform.position.x : mc.transform.position.y).ToList();
            var hasLeveledUpNonTreasure = false;
            if (EnforceManager.Instance.addGold)
            {
                EnforceManager.Instance.addGoldCount++;
            }
            foreach (var sort in sortedList)
            {
                var characterBase = sortedList[2].GetComponent<CharacterBase>();
                var isTreasure = characterBase.Type == CharacterBase.Types.Treasure;
                var isCenter = sort.transform.position == swapPosition;
                if (isTreasure)
                {
                    if (isCenter)
                    {
                        if (hasLeveledUpNonTreasure) continue;
                        characterBase.LevelUpScale(sortedList[2]);
                        characterBase.LevelUpScale(sortedList[2]);
                        characterBase.LevelUpScale(sortedList[2]);
                        commonRewardManager.PendingTreasure.Enqueue(sortedList[2]);
                        hasLeveledUpNonTreasure = true;
                        MergeEffect(sortedList[2]);
                    }
                    else
                    {
                        ReturnObject(sort);
                        ReturnEffect(sort);
                    }
                }
                else
                {
                    if (hasLeveledUpNonTreasure) continue;
                    sortedList[2].GetComponent<CharacterBase>().LevelUpScale(sortedList[2]);
                    MergeEffect(sortedList[2]);
                    sortedList[1].GetComponent<CharacterBase>().LevelUpScale(sortedList[1]);
                    MergeEffect(sortedList[1]);
                    sortedList[3].GetComponent<CharacterBase>().LevelUpScale(sortedList[3]);
                    MergeEffect(sortedList[3]);
                    if (enforceManager.match5Upgrade)
                    {
                        sortedList[2].GetComponent<CharacterBase>().LevelUpScale(sortedList[2]);
                    }
                    ReturnObject(sortedList[0]);
                    ReturnEffect(sortedList[0]);
                    ReturnObject(sortedList[4]);
                    ReturnEffect(sortedList[4]);
                    hasLeveledUpNonTreasure = true;
                   
                }
            }
            StartCoroutine(Match3(sortedList[2]));
            return true;
        }
        private IEnumerator Match3(GameObject center)
        {
            yield return new WaitForSecondsRealtime(1f); 
            IsMatched(center);
            StartCoroutine(spawnManager.PositionUpCharacterObject());
        }
        private bool Matches3X3Case(IEnumerable<GameObject> rawMatchedCharacters, Vector3 swapPosition)
        {
            SoundManager.Instance.MatchSound(3);
            var grouped = rawMatchedCharacters.GroupBy(mc => mc.transform.position);
            var matchedCharacters = grouped.Select(g => g.First()).ToList();
            var horizontalMatches = matchedCharacters.Where(mc => (int)mc.transform.position.y == (int)swapPosition.y)
                .OrderBy(mc => mc.transform.position.x).ToList();
            var verticalMatches = matchedCharacters.Where(mc => (int)mc.transform.position.x == (int)swapPosition.x)
                .OrderBy(mc => mc.transform.position.y).ToList();
            var intersection = horizontalMatches.FirstOrDefault(h => verticalMatches.Contains(h));
            GameObject nextInVertical = null;
            if (intersection != null)
            {
                intersection.GetComponent<CharacterBase>().LevelUpScale(intersection);
                if (intersection.GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    commonRewardManager.PendingTreasure.Enqueue(intersection);
                }
                MergeEffect(intersection);
                var indexInVertical = verticalMatches.IndexOf(intersection);
                nextInVertical = indexInVertical < verticalMatches.Count - 1 ? verticalMatches[indexInVertical + 1] : verticalMatches[indexInVertical - 1];
                nextInVertical.GetComponent<CharacterBase>().LevelUpScale(nextInVertical);
                if (nextInVertical.GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    commonRewardManager.PendingTreasure.Enqueue(nextInVertical);
                }
                MergeEffect(nextInVertical);
               
            }
            foreach (var obj in matchedCharacters.Where(obj => obj != intersection && obj != nextInVertical))
            {
                ReturnObject(obj);
                ReturnEffect(obj);
            }
            IsMatched(intersection);
            IsMatched(nextInVertical);
            return true;
        }
        private bool Matches3X4Case(IEnumerable<GameObject> rawMatchedCharacters, Vector3 swapPosition)
        {
            SoundManager.Instance.MatchSound(4);
            var grouped = rawMatchedCharacters.GroupBy(mc => mc.transform.position);
            var matchedCharacters = grouped.Select(g => g.First()).ToList();
            var horizontalMatches = matchedCharacters.Where(mc => (int)mc.transform.position.y == (int)swapPosition.y)
                .OrderBy(mc => mc.transform.position.x).ToList();
            var verticalMatches = matchedCharacters.Where(mc => (int)mc.transform.position.x == (int)swapPosition.x)
                .OrderBy(mc => mc.transform.position.y).ToList();
            var longestMatch = horizontalMatches.Count > verticalMatches.Count ? horizontalMatches : verticalMatches;
            if (longestMatch.Count < 3) return true; // Ensure we have at least 3 to consider it a match
            var levelUp1 = longestMatch[1];
            var levelUp2 = longestMatch[2];

            levelUp1.GetComponent<CharacterBase>().LevelUpScale(levelUp1);
            MergeEffect(levelUp1);
            levelUp2.GetComponent<CharacterBase>().LevelUpScale(levelUp2);
            MergeEffect(levelUp2);
            if(levelUp1.GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
            {
                commonRewardManager.PendingTreasure.Enqueue(levelUp1);
                commonRewardManager.PendingTreasure.Enqueue(levelUp2);
            }
            foreach(var obj in matchedCharacters.Where(obj => obj != levelUp1 && obj != levelUp2))
            {
                ReturnObject(obj);
                ReturnEffect(obj);
            }
            IsMatched(levelUp1);
            IsMatched(levelUp2);
            return true;
        }
        private bool Matches3X5Case(IEnumerable<GameObject> rawMatchedCharacters, Vector3 swapPosition)
        {
            SoundManager.Instance.MatchSound(5);
            var grouped = rawMatchedCharacters.GroupBy(mc => mc.transform.position);
            var matchedCharacters = grouped.Select(g => g.First()).ToList();
            var match5Horizontal = matchedCharacters.Where(mc => (int)mc.transform.position.y == (int)swapPosition.y)
                .OrderBy(mc => mc.transform.position.x).ToList();
            var match5Vertical = matchedCharacters.Where(mc => (int)mc.transform.position.x == (int)swapPosition.x)
                .OrderBy(mc => mc.transform.position.y).ToList();
            var match3Horizontal = match5Horizontal.Count == 5 ? new List<GameObject>() : match5Horizontal;
            var match3Vertical = match5Vertical.Count == 5 ? new List<GameObject>() : match5Vertical;
            if (match5Horizontal.Count >= 5)
            {
                Matches5Case(match5Horizontal, "x", swapPosition);
            }
            else
            {
                Matches5Case(match5Vertical, "y", swapPosition);
            }
            if (match3Horizontal.Count >= 3)
            {
                var levelUpObjectHorizontal = match3Horizontal[1];
                var returnObjectsHorizontal = match3Horizontal.Where((obj, index) => index != 1 && obj.transform.position != swapPosition);
                levelUpObjectHorizontal.GetComponent<CharacterBase>().LevelUpScale(levelUpObjectHorizontal);
                if (levelUpObjectHorizontal.GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    commonRewardManager.PendingTreasure.Enqueue(levelUpObjectHorizontal);
                }
                MergeEffect(levelUpObjectHorizontal);
                foreach (var obj in returnObjectsHorizontal)
                {
                    ReturnObject(obj);
                    ReturnEffect(obj);
                }
                IsMatched(levelUpObjectHorizontal);
            }
            else
            {
                var levelUpObjectVertical = match3Vertical[1];
                var returnObjectsVertical = match3Vertical.Where((obj, index) => index != 1 && obj.transform.position != swapPosition);
                levelUpObjectVertical.GetComponent<CharacterBase>().LevelUpScale(levelUpObjectVertical);
                if (levelUpObjectVertical.GetComponent<CharacterBase>().Type == CharacterBase.Types.Treasure)
                {
                    commonRewardManager.PendingTreasure.Enqueue(levelUpObjectVertical);
                }
                MergeEffect(levelUpObjectVertical);
                foreach (var obj in returnObjectsVertical)
                {
                    ReturnObject(obj);
                    ReturnEffect(obj);
                }
                IsMatched(levelUpObjectVertical);
            }
            return true;
        }
    }
}