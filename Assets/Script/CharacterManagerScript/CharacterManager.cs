using System.Collections.Generic;
using System.Linq;
using Script.RobbyScript.CharacterSelectMenuGroup;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private CharacterBase treasureBox;
        public List<CharacterBase> characterList = new List<CharacterBase>();
        public readonly HashSet<int> CharacterGroupLevelUpIndexes = new HashSet<int>();
        public bool goldGetMore;
        private List<CharacterBase> _instanceUnit = new List<CharacterBase>();

        public void Awake()
        {
            _instanceUnit = SelectedUnitHolder.Instance.selectedUnit;
            foreach (var unit in _instanceUnit)
            {
                characterList.Add(unit);
            }
            characterList.Add(treasureBox);
        }
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
