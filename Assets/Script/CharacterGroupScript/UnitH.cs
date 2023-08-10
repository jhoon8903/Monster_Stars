using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitH : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite; 
        [SerializeField] private Sprite level4Sprite; 
        [SerializeField] private Sprite level5Sprite;
        [SerializeField] private Sprite level6Sprite;

        public void Awake()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.H;
            UnitProperty = UnitProperties.Fire;
            UnitGrade = UnitGrades.Blue;
            UnitDesc = "유닛H 입니다.";
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
            center = transform.position;
            size = new Vector2(3, 3);
        }
        public override List<GameObject> DetectEnemies()
        {
            GetDetectionProperties(out var size, out var center);
            var colliders = Physics2D.OverlapBoxAll(center, size, 0f);
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
            };
        }

        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            UnitLevelDamage = unitPeaceLevel > 1 ? unitPeaceLevel * 2 + 3f : 0f;
            Type = Types.Character;
            unitGroup = UnitGroups.H;
            DefaultDamage = UnitLevelDamage + 27f * level switch
            {
                <= 2 => 1f,
                3 => 1.7f,
                4 => 2f,
                5 => 2.3f,
                6 => 2.6f
            };
            effectStack = EnforceManager.Instance.fireStackOverlap ? 4 : 1;
            dotDamage = DefaultDamage * 0.1f;
            burnTime = 5f;
            var increaseRateBoost = 1f - EnforceManager.Instance.fireAttackSpeedBoost;
            defaultAtkRate = 1f * increaseRateBoost;
            defaultAtkDistance = 9f; 
            var increaseProjectileBoost = EnforceManager.Instance.fireProjectileSpeedIncrease ? 1f : 0f;
            projectileSpeed = 1f + increaseProjectileBoost;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Fire;
            UnitEffect = UnitEffects.Burn;
        }
    }
}
