using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Unit_B : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        [SerializeField] private Sprite level5Sprite; // Sprite for level 5

        private SpriteRenderer _spriteRenderer; // Reference to the SpriteRenderer component

        private void Awake()
        {
            unitGroup = UnitGroups.B;
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
                case 5:
                    Level5(); // Set properties for level 5
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

        // Sets the properties for level 1 of the character
        private void Level1()
        {
            CharacterName = "Unit_B_00";
            UnitLevel = 1;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            defaultDamage = 0;
            defaultAtkRate = 0;
            defaultAtkDistance = 0;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level1Sprite;
        }

        // Sets the properties for level 2 of the character
        private void Level2()
        {
            CharacterName = "Unit_B_01";
            UnitLevel = 2;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            defaultDamage = 1;
            defaultAtkRate = 0.1f;
            defaultAtkDistance = 0.1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 3 of the character
        private void Level3()
        {
            CharacterName = "Unit_B_02";
            UnitLevel = 3;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            defaultDamage = 3;
            defaultAtkRate = 0.3f;
            defaultAtkDistance = 0.2f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 4 of the character
        private void Level4()
        {
            CharacterName = "Unit_B_03";
            UnitLevel = 4;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            defaultDamage = 9;
            defaultAtkRate = 0.9f;
            defaultAtkDistance = 0.3f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 5 of the character
        private void Level5()
        {
            CharacterName = "Unit_B_04";
            UnitLevel = 5;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            defaultDamage = 27;
            defaultAtkRate = 2.7f;
            defaultAtkDistance = 0.4f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }
    }
}
