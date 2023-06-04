using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Unit_Treasure : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Level1();
        }

        protected override void LevelUp()
        {
            base.LevelUp();
            switch (Level)
            {
                case 2:
                    Level2();
                    break;
                case 3:
                    Level3();
                    break;
                case 4:
                    Level4();
                    break;
                default:
                    return;
            }
        }

        protected internal override void CharacterReset()
        {
            ResetLevel();
            Level1();
        }

        private void Level1()
        {
            CharacterName = "Unit_Treasure00";
            UnitLevel = 1;
            Type = Types.Treasure;
            _spriteRenderer.sprite = level1Sprite;
        }

        private void Level2()
        {
            CharacterName = "Unit_Treasure01";
            UnitLevel = 2;
            Type = Types.Treasure;
            _spriteRenderer.sprite = level2Sprite;
            UnitGroup = UnitGroups.None;
            UnitAtkType = UnitAtkTypes.None;
            UnitProperty = UnitProperties.None;
            UnitEffect = UnitEffects.None;

        }

        private void Level3()
        {
            CharacterName = "Unit_Treasure02";
            UnitLevel = 3;
            Type = Types.Treasure;
            _spriteRenderer.sprite = level3Sprite;
            UnitGroup = UnitGroups.None;
            UnitAtkType = UnitAtkTypes.None;
            UnitProperty = UnitProperties.None;
            UnitEffect = UnitEffects.None;
        }

        private void Level4()
        {
            CharacterName = "Unit_Treasure03";
            UnitLevel = 4;
            Type = Types.Treasure;
            _spriteRenderer.sprite = level4Sprite;
            UnitGroup = UnitGroups.None;
            UnitAtkType = UnitAtkTypes.None;
            UnitProperty = UnitProperties.None;
            UnitEffect = UnitEffects.None;
        }
    }
}