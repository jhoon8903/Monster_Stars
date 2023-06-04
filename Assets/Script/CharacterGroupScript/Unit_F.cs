using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Unit_F : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;
        private SpriteRenderer _spriteRenderer;
        private const float DetectionSize = 1.5f;

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

        public override List<GameObject> DetectEnemies()
        {
            var detectionCenter = (Vector2)transform.position;
            var colliders = Physics2D.OverlapCircleAll(detectionCenter, DetectionSize);
            var detectedEnemies = (from collider in colliders
                where collider.gameObject.CompareTag("Enemy")
                select collider.gameObject).ToList();
            return detectedEnemies;
        }

        public void OnDrawGizmos()
        {
            var detectionCenter = transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectionCenter, DetectionSize);
        }

        private void Level1()
        {
            CharacterName = "Unit_F_00";
            UnitLevel = 1;
            Type = Types.Character;
            UnitGroup = UnitGroups.F;
            defaultDamage = 0f;
            defaultAtkRate = 0;
            defaultAtkDistance = 0;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level1Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }

        private void Level2()
        {
            CharacterName = "Unit_F_01";
            UnitLevel = 2;
            Type = Types.Character;
            UnitGroup = UnitGroups.F;
            defaultDamage = 90;
            defaultAtkRate = 1f;
            defaultAtkDistance = 1.5f;
            projectileSpeed = 1.3f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }

        private void Level3()
        {
            CharacterName = "Unit_F_02";
            UnitLevel = 3;
            Type = Types.Character;
            UnitGroup = UnitGroups.F;
            defaultDamage *= 1.7f;
            defaultAtkRate = 1f;
            defaultAtkDistance = 1.5f;
            projectileSpeed = 1.3f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }

        private void Level4()
        {
            CharacterName = "Unit_F_03";
            UnitLevel = 4;
            Type = Types.Character;
            UnitGroup = UnitGroups.F;
            defaultDamage *= 2.0f;
            defaultAtkRate = 1f;
            defaultAtkDistance = 1.5f;
            projectileSpeed = 1.3f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }

        private void Level5()
        {
            CharacterName = "Unit_F_04";
            UnitLevel = 5;
            Type = Types.Character;
            UnitGroup = UnitGroups.F;
            defaultDamage *= 2.3f;
            defaultAtkRate = 1f;
            defaultAtkDistance = 1.5f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }
    }
}