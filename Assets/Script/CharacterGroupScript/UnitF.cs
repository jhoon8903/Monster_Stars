using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitF : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;
        private Vector2 _detectionSize;

        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.F;
            UnitProperty = UnitProperties.Poison;
            UnitGrade = UnitGrades.Green;
            UnitDesc = "유닛F 입니다.";
            SetLevel(1);
        }
        
        public override Sprite GetSpriteForLevel(int characterObjectLevel)
        {
            return characterObjectLevel switch
            {
                <= 9 => level1Sprite,
                <= 19 => level2Sprite,
                <= 29 => level3Sprite,
                <= 39 => level4Sprite,
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

        private void GetDetectionProperties(out Vector2 size, out Vector2 center)
        {
            _detectionSize = EnforceManager.Instance.poisonRangeIncrease ? new Vector2(5, 5) : new Vector2(3, 3);
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
                _ => level5Sprite
            };
        }

        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            var unitLevelDamage = unitPieceLevel * 20f;
            Type = Types.Character;
            unitGroup = UnitGroups.F;
            DefaultDamage = unitLevelDamage + 100f * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            var increaseRateBoost = 1f + EnforceManager.Instance.poisonAttackSpeedIncrease * 6 / 100f; 
            defaultAtkRate = 1.2f / increaseRateBoost;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.GuideProjectile;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }
    }
}
