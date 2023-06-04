using Script.CharacterManagerScript;
using UnityEngine;


namespace Script.CharacterGroupScript
{
    public class Unit_C : CharacterBase
    {


        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Level1();
        }

        protected override void LevelUp()
        {
            base.LevelUp(); // increment the level
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
                case 5:
                    Level5();
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
            CharacterName = "Unit_C_00";
            UnitLevel = 1;
            Type = Types.Character;
            defaultDamage = 0;
            defaultAtkRate = 0;
            defaultAtkDistance = 0;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level1Sprite;

        }

        private void Level2()
        {
            CharacterName = "Unit_C_01";
            UnitLevel = 2;
            Type = Types.Character;
            UnitGroup = UnitGroups.C;
            defaultDamage = 90;
            defaultAtkRate = 0.1f;
            defaultAtkDistance = 0.1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level2Sprite;
        }

        private void Level3()
        {
            CharacterName = "Unit_C_02";
            UnitLevel = 3;
            Type = Types.Character;
            defaultDamage = 3;
            defaultAtkRate = 0.3f;
            defaultAtkDistance = 0.2f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level3Sprite;
        }

        private void Level4()
        {
            CharacterName = "Unit_C_03";
            UnitLevel = 4;
            Type = Types.Character;
            defaultDamage = 9;
            defaultAtkRate = 0.9f;
            defaultAtkDistance = 0.3f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level4Sprite;
        }

        private void Level5()
        {
            CharacterName = "Unit_C_04";
            UnitLevel = 5;
            Type = Types.Character;
            defaultDamage = 27;
            defaultAtkRate = 2.7f;
            defaultAtkDistance = 0.4f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level5Sprite;
        }
    }
}