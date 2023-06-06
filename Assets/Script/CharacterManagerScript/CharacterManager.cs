using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] internal List<CharacterBase> characterList = new List<CharacterBase>(); // List of character bases
        [SerializeField] private CharacterPool characterPool; // Reference to the character pool

        // Increase the damage of all characters in the group by a given amount
        public void IncreaseGroupDamage(int increaseAmount)
        {
            foreach (var character in characterList)
            {
                character.IncreaseDamage(increaseAmount);
            }
        }

        // Increase the attack rate of all characters in the group by a given amount
        public void IncreaseGroupAtkRate(int increaseAmount)
        {
            foreach (var character in characterList)
            {
                character.IncreaseAtkRate(increaseAmount);
            }
        }

        // Level up a random selection of characters a specified number of times
        public void RandomCharacterLevelUp(int characterCount)
        {
            var activeCharacters = characterPool.UsePoolCharacterList();
            if (activeCharacters.Count == 0) return;
            var rnd = new System.Random();
            var levelUpCount = 0;
    
            var eligibleCharacters = activeCharacters.Where(character => 
                character.GetComponent<CharacterBase>()?.UnitLevel < 5).ToList();
            if (eligibleCharacters.Count == 0) return;
    
            while (levelUpCount < characterCount && eligibleCharacters.Count > 0)
            {
                var randomIndex = rnd.Next(eligibleCharacters.Count);
                var characterBase = eligibleCharacters[randomIndex].GetComponent<CharacterBase>();
                characterBase.LevelUpScale(eligibleCharacters[randomIndex]);
                levelUpCount++;
        
                eligibleCharacters = eligibleCharacters.Where(character => 
                    character.GetComponent<CharacterBase>()?.UnitLevel < 5).ToList();
            }
        }


        // Level up all characters in a specific group
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

        // Set the permanent level up flag for all characters in a specific group
        public void PermanentIncreaseCharacter(int characterListIndex)
        {
            var activeCharacterGroup = characterList[characterListIndex];
            var activeCharacters = characterPool.UsePoolCharacterList();
            foreach (var character in activeCharacters
                         .Select(characterObject => characterObject.GetComponent<CharacterBase>())
                         .Where(character => character.UnitGroup == activeCharacterGroup.UnitGroup &&
                                             character.Type == CharacterBase.Types.Character &&
                                             character.UnitLevel == 1))
            {
                character.PermanentLevelUp = true;
            }
        }
    }
}
