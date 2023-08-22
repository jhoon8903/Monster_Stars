using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class DeathChiller : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite; 
        [SerializeField] private Sprite level4Sprite; 
        [SerializeField] private Sprite level5Sprite;
        [SerializeField] private Sprite level6Sprite;
        [SerializeField] private Sprite level7Sprite;
        private float _detectionWidth;
        private float _detectionHeight;

        public void Awake()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.DeathChiller;
            UnitProperty = UnitProperties.Water;
            UnitGrade = UnitGrades.P;
            UnitDesc = "Death Chiller Unit C / P Grade";
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
            _detectionHeight = defaultAtkDistance;
            size = new Vector2(_detectionWidth, _detectionHeight * 2f);
            center = transform.position;
        }

        public override List<GameObject> DetectEnemies()
        {
            GetDetectionProperties(out var detectionSize, out var detectionCenter);
            var colliders = Physics2D.OverlapBoxAll(detectionCenter, detectionSize, 0f);
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
                6 => level6Sprite,
                7 => level7Sprite
            };
        }

        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            UnitLevelDamage = unitPeaceLevel > 1 ? unitPeaceLevel * 2 + 3f : 0;
            Type = Types.Character;
            unitGroup = UnitGroups.DeathChiller;
            DefaultDamage = UnitLevelDamage + 11f * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                5 => 2.3f,
                6 => 2.6f,
                7 => 2.9f
            };
            slowTime = 1f + EnforceManager.Instance.waterSlowDurationBoost;
            slowPower = EnforceManager.Instance.deathChillerSlowCPowerBoost ? 0.55f : 0.7f;
            freezeTime = 1f;
            effectChance = EnforceManager.Instance.deathChillerFreezeChance ? 15 : 25;
            defaultAtkRate = 0.8f * (1f - EnforceManager.Instance.waterAttackRateBoost);
            defaultAtkDistance = 9f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Water;
            UnitEffect = UnitEffects.Slow;
        }
    }
}
