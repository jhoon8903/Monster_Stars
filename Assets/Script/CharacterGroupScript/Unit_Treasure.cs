using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Unit_Treasure : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        private SpriteRenderer _spriteRenderer; // Reference to the SpriteRenderer component

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>(); // Get the reference to the SpriteRenderer component attached to this object
            Level1(); // Set initial level to level 1
        }

        protected override void LevelUp()
        {
            base.LevelUp(); // Increment the level

            // Update the character's properties based on the current level
            switch (Level)
            {
                case 2:
                    Level2(); // Set properties for level 2
                    break;
                case 3:
                    Level3(); // Set properties for level 3
                    break;
                case 4:
                    Level4(); // Set properties for level 4
                    break;
                default:
                    return;
            }
        }

        protected internal override void CharacterReset()
        {
            ResetLevel(); // Reset the character's level
            Level1(); // Set level back to 1
        }

        // Sets the properties for level 1 of the treasure
        private void Level1()
        {
            CharacterName = "Unit_Treasure00";
            UnitLevel = 1;
            Type = Types.Treasure;
            _spriteRenderer.sprite = level1Sprite;
        }

        // Sets the properties for level 2 of the treasure
        private void Level2()
        {
            CharacterName = "Unit_Treasure01";
            UnitLevel = 2;
            Type = Types.Treasure;
            _spriteRenderer.sprite = level2Sprite;
            unitGroup = UnitGroups.None;
            UnitAtkType = UnitAtkTypes.None;
            UnitProperty = UnitProperties.None;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 3 of the treasure
        private void Level3()
        {
            CharacterName = "Unit_Treasure02";
            UnitLevel = 3;
            Type = Types.Treasure;
            _spriteRenderer.sprite = level3Sprite;
            unitGroup = UnitGroups.None;
            UnitAtkType = UnitAtkTypes.None;
            UnitProperty = UnitProperties.None;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 4 of the treasure
        private void Level4()
        {
            CharacterName = "Unit_Treasure03";
            UnitLevel = 4;
            Type = Types.Treasure;
            _spriteRenderer.sprite = level4Sprite;
            unitGroup = UnitGroups.None;
            UnitAtkType = UnitAtkTypes.None;
            UnitProperty = UnitProperties.None;
            UnitEffect = UnitEffects.None;
        }
    }
}
