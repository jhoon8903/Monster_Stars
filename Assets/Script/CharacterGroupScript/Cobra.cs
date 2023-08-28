using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Cobra : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;
        [SerializeField] private Sprite level6Sprite;
        private Vector2 _detectionSize;

        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.Cobra;
            UnitProperty = UnitProperties.Poison;
            UnitGrade = UnitGrades.B;
            UnitDesc = "Cobra's venom is very potent \nand leaves a puddle that continues to poison the enemy.";
            SetLevel(1);
        }

        public override Sprite GetSpriteForLevel(int characterObjectLevel)
        {
            return characterObjectLevel switch
            {
                <= 3 => level2Sprite,
                <= 5 => level3Sprite,
                <= 7 => level4Sprite,
                <= 9 => level5Sprite,
                _ => level6Sprite
            };
        }

        protected internal override Sprite GetBasicSprite()
        {
            return level1Sprite;
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

        private void GetDetectionProperties(out Vector2 size, out Vector2 center)
        {
            _detectionSize = EnforceManager.Instance.cobraRangeBoost ? new Vector2(5,5) :new Vector2(3, 3);
            center = transform.position;
            size = _detectionSize;
        }

        public override List<GameObject> DetectEnemies()
        {
            GetDetectionProperties(out var detectionSize, out var detectionCenter);
            var colliders = Physics2D.OverlapBoxAll(detectionCenter, detectionSize,0f);
            var currentlyDetectedEnemies = (
                from enemyObject in colliders 
                where enemyObject.gameObject.CompareTag("Enemy") && enemyObject.gameObject.activeInHierarchy 
                select enemyObject.GetComponent<EnemyBase>()
                into enemyBase 
                select enemyBase.gameObject).ToList();
            DetectedEnemies = currentlyDetectedEnemies;
            return DetectedEnemies;
        }

        protected internal override Sprite GetSprite(int level)
        {
            return level switch
            {
                1 => level1Sprite,
                2 => level2Sprite,
                3 => level3Sprite,
                4 => level4Sprite,
                5 => level5Sprite,
                6 => level6Sprite
            };
        }

        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            UnitLevelDamage = unitPieceLevel > 0 ? unitPieceLevel * 6f - 1f : 0f;
            Type = Types.Character;
            unitGroup = UnitGroups.Cobra;
            DefaultDamage = UnitLevelDamage + 42f * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                5 => 2.3f,
                6 => 2.6f
            };
            effectChance = EnforceManager.Instance.cobraStunChanceBoost ? 100 : 50;
            poisonTime = 5f;
            var dotDamageBoost = EnforceManager.Instance.cobraDotDamageBoost ? 0.2f : 0.1f;
            dotDamage = DefaultDamage * dotDamageBoost;
            var timeBoost = EnforceManager.Instance.poison2StunTimeBoost;
            stunTime = 0.4f + timeBoost;
            defaultAtkRate = EnforceManager.Instance.cobraRateBoost ? 1.2f * 0.8f : 1.2f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.GuideProjectile;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }
    }
}