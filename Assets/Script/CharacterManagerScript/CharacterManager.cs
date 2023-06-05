using System.Collections.Generic;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] internal List<CharacterBase> characterList = new List<CharacterBase>();
        [SerializeField] private CharacterPool characterPool;

        public List<CharacterBase> GetCharacterList()
        {
            return characterList;
        }

        public void IncreaseGroupDamage(int increaseAmount)
        {
            foreach (var character in characterList)
            {
                character.IncreaseDamage(increaseAmount);
            }
        }

        public void IncreaseGroupAtkRate(int increaseAmount)
        {
            foreach (var character in characterList)
            {
                character.IncreaseAtkRate(increaseAmount);
            }
        }

        public void RandomCharacterLevelUp(int characterCount)
        {
            var activeCharacters = characterPool.UsePoolCharacterList();
            if (activeCharacters.Count == 0) return;
            var rnd = new System.Random();
            var levelUpCount = 0;
            while (levelUpCount < characterCount)
            {
                var randomIndex = rnd.Next(activeCharacters.Count);
                var characterBase = activeCharacters[randomIndex].GetComponent<CharacterBase>();
                if (characterBase == null) continue;
                if (characterBase.UnitLevel >= 5) continue;
                characterBase.LevelUpScale(activeCharacters[randomIndex]);
                levelUpCount++;
            }
        }

        public void CharacterGroupLevelUp(int characterListIndex)
        {
            var activeCharacters = characterPool.UsePoolCharacterList();
            var activeCharacterGroup = characterList[characterListIndex];
            foreach (var character in activeCharacters)
            {
                var characterObj = character.GetComponent<CharacterBase>();
                if (activeCharacterGroup.GetComponent<CharacterBase>().UnitGroup ==
                    characterObj.UnitGroup && characterObj.UnitLevel == 1
                    && characterObj.Type == CharacterBase.Types.Character)
                {
                    characterObj.LevelUpScale(character);
                }
            }
        }
    }
}