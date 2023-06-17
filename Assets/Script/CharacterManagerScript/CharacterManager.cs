using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] internal List<CharacterBase> characterList = new List<CharacterBase>(); // List of character bases
        [SerializeField] private CharacterPool characterPool; // Reference to the character pool
        public readonly HashSet<int> CharacterGroupLevelUpIndexes = new HashSet<int>();
        public bool goldGetMore = false;

        // Level up a random selection of characters a specified number of times
        public void RandomCharacterLevelUp(int characterCount)
        {
            var activeCharacters = characterPool.UsePoolCharacterList();
            if (activeCharacters.Count == 0) return;
            var rnd = new System.Random();
            var levelUpCount = 0;
    
            var eligibleCharacters = activeCharacters.Where(character => 
                character.GetComponent<CharacterBase>()?.UnitLevel < 5 && character.GetComponent<CharacterBase>().Type != CharacterBase.Types.Treasure).ToList();
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
            var group = characterList[characterListIndex].unitGroup;
            var activeCharacterGroup = characterPool.UsePoolCharacterList();

            foreach (var character in  activeCharacterGroup)
            {
                var characterObj = character.GetComponent<CharacterBase>();
                if (group == characterObj.unitGroup && characterObj.UnitLevel == 1)
                {
                    characterObj.LevelUpScale(character);
                }
            }
        }

        // Set the permanent level up flag for all characters in a specific group
        public void PermanentIncreaseCharacter(int characterListIndex)
        {
            var levelUpGroup = characterList[characterListIndex].unitGroup;
            var pooledCharacters = characterPool.pooledCharacters;
            foreach (var character in pooledCharacters
                         .Select(characterObject => characterObject.GetComponent<CharacterBase>())
                         .Where(character => character.unitGroup == levelUpGroup && character.UnitLevel == 1))
            {
                character.PermanentLevelUp = true;
            }
        }
    }
}
