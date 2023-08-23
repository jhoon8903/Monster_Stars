using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Skeleton : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;
        private Vector2 _detectionSize;
        public float groupFDamage;

        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.Skeleton;
            UnitProperty = UnitProperties.Poison;
            UnitGrade = UnitGrades.G;
            UnitDesc = "Individually, the skeleton is insignificant, \nbut together they are stronger.";
            SetLevel(1);
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

        private void GetDetectionProperties(out Vector2 size, out Vector2 center)
        {
            _detectionSize = new Vector2(3, 3);
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
            UnitLevelDamage = unitPieceLevel > 0 ? unitPieceLevel * 5 + 1f : 0f;
            Type = Types.Character;
            unitGroup = UnitGroups.Skeleton;
            var increaseDamage = EnforceManager.Instance.skeletonDamageBoost ? 0.16f : 0f;
            DefaultDamage = UnitLevelDamage + 32f * (1f + groupFDamage) * (1f + increaseDamage) * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            var increaseDotDamage = EnforceManager.Instance.skeletonDotDamageBoost ? 0.3f : 0.2f;
            dotDamage = DefaultDamage * increaseDotDamage;
            var effectTime = EnforceManager.Instance.skeletonPerHitEffect ? 3f : 0f;
            poisonTime = effectTime + (EnforceManager.Instance.skeletonDurationBoost ? 2f : 0f);
            var increaseRateBoost = 1f - EnforceManager.Instance.poisonAttackSpeedIncrease; 
            defaultAtkRate = 1.2f * increaseRateBoost;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.GuideProjectile;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }
    }
}
