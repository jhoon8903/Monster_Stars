using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Orc : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;
        public float groupDAtkRate;
        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.Orc;
            UnitProperty = UnitProperties.Physics;
            UnitGrade = UnitGrades.G;
            UnitDesc = "Orc Unit D / G Grade";
            SetLevel(1);
        }
        public void Awake()
        {
            Initialize();
        }

        public override Sprite GetSpriteForLevel(int characterObjectLevel)
        {
            return characterObjectLevel switch
            {
                <= 3 => level1Sprite,
                <= 6 => level2Sprite,
                <= 9 => level3Sprite,
                <= 12 => level4Sprite,
                _ => level5Sprite
            };
        }
        protected override void LevelUp()
        {
            base.LevelUp();
            SetLevel(unitPuzzleLevel);
        }
        protected internal override void CharacterReset()
        {
            base.CharacterReset();
            SetLevel(unitPuzzleLevel);
        }

        private void GetDetectionProperties(out float size, out Vector2 center)
        {
            center = transform.position;
            size = EnforceManager.Instance.orcSwordScaleIncrease ? 2.5f : 1.5f;
        }

        public override List<GameObject> DetectEnemies()
        {
            GetDetectionProperties(out var size, out var center);
            var colliders = Physics2D.OverlapCircleAll(center, size);
            var currentlyDetectedEnemies = (
                from enemyObject in colliders 
                where enemyObject.gameObject.CompareTag("Enemy") && enemyObject.gameObject.activeInHierarchy 
                select enemyObject.GetComponent<EnemyBase>() 
                into enemyBase 
                select enemyBase.gameObject).ToList();
            DetectedEnemies = currentlyDetectedEnemies;
            return DetectedEnemies;
        }


        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            UnitLevelDamage = unitPieceLevel > 1 ? unitPieceLevel * 3 : 0f;
            Type = Types.Character;
            unitGroup = UnitGroups.Orc;
            var increaseDamage = 1f - EnforceManager.Instance.physicalDamageBoost;
            DefaultDamage = UnitLevelDamage + 23f * increaseDamage * level switch
            {
                <= 2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            dotDamage = DefaultDamage * 0.2f;
            bleedTime = EnforceManager.Instance.orcBleedDuration ? 5 : 3;
            defaultAtkRate = 1f * (1f - EnforceManager.Instance.physicalAttackSpeedBoost) * (1f - groupDAtkRate);
            swingSpeed = swingSpeed = 1f / (defaultAtkRate * (1f - EnforceManager.Instance.physicalAttackSpeedBoost) * (1f - groupDAtkRate));
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.Bleed;
        }

        protected internal override Sprite GetSprite(int level)
        {
            return level switch
            {
                1 => level1Sprite,
                2 => level2Sprite,
                3 => level3Sprite,
                4 => level4Sprite,
                _ => level5Sprite
            };
        }
    }
}