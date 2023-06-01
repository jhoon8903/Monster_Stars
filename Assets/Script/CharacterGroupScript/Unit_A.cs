using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Unit_A : CharacterBase
    {
        [SerializeField]
        private Sprite level1Sprite;
        [SerializeField]
        private Sprite level2Sprite;
        [SerializeField]
        private Sprite level3Sprite;
        [SerializeField]
        private Sprite level4Sprite;
        [SerializeField]
        private Sprite level5Sprite;

        private SpriteRenderer _spriteRenderer;
        private const float DetectionWidth = 1f;
        private const float DetectionHeight = 9f;

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
                case 5:
                    Level5();
                    break;
                default:
                    return;
            }
        }
        protected internal override void LevelReset()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Level1();
        }

        public override List<GameObject> DetectEnemies()
        {
            Debug.Log("DetectEnemies");
            var detectionSize = new Vector2(DetectionWidth, DetectionHeight);
            var detectionCenter = (Vector2)transform.position + Vector2.up * DetectionHeight / 2f;
            var colliders = Physics2D.OverlapBoxAll(detectionCenter, detectionSize, 0f);
            return (from collider in colliders
                    where collider.gameObject
                        .CompareTag("Enemy")
                    select collider.gameObject)
                .ToList();
        }

        public void OnDrawGizmosSelected()
        {
            Debug.Log("ONDrawGizmos");
            var detectionSize = new Vector3(DetectionWidth, DetectionHeight, 0);
            var detectionCenter = (Vector3)transform.position * DetectionHeight / 2f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(detectionCenter, detectionSize);
        }



        private void Level1()
        {
            CharacterName = "Unit_A_00";
            Type = Types.Character;
            defaultDamage = 0;
            defaultAtkRate = 0;
            defaultAtkDistance = 0;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level1Sprite;
        }
        private void Level2()
        {
            CharacterName = "Unit_A_01";
            Type = Types.Character;
            UnitGroup = UnitGroups.A;
            defaultDamage = 125f;
            defaultAtkRate = 1;
            defaultAtkDistance = 12;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
        }
        private void Level3()
        {
            CharacterName = "Unit_A_02";
            Type = Types.Character;
            UnitGroup = UnitGroups.A;
            defaultDamage *= 1.7f;
            defaultAtkRate = 0.3f;
            defaultAtkDistance = 12f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
        }
        private void Level4()
        {
            CharacterName = "Unit_A_03";
            Type = Types.Character;
            UnitGroup = UnitGroups.A;
            defaultDamage *= 2;
            defaultAtkRate = 0.9f;
            defaultAtkDistance = 12;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
        }
        private void Level5()
        {
            CharacterName = "Unit_A_04";
            Type = Types.Character;
            UnitGroup = UnitGroups.A;
            defaultDamage *= 2.3f;
            defaultAtkRate = 2.7f;
            defaultAtkDistance = 12;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
        }

    }
}
