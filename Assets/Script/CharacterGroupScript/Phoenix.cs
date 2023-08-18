using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Phoenix : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite; 
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;
        [SerializeField] private Sprite level6Sprite;
        private float _currentDamage;

        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.Phoenix;
            UnitProperty = UnitProperties.Fire;
            UnitGrade = UnitGrades.Blue;
            UnitDesc = "Phoenix Unit G / Blue Grade";
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

        private void GetDetectionProperties(out float size, out Vector2 center)
        {
            center = transform.position;
            size = EnforceManager.Instance.fire2RangeBoost ? 2.5f : 1.5f;
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
            UnitLevelDamage = unitPeaceLevel > 0 ? unitPeaceLevel * 3f + 2f: 0f;
            Type = Types.Character;
            unitGroup = UnitGroups.Phoenix;
            var damageBoost = 1f + EnforceManager.Instance.fire2DamageBoost;
            var propertyDamage = EnforceManager.Instance.fire2ChangeProperty ? 1.5f : 1f;
            DefaultDamage = UnitLevelDamage + 29f * damageBoost * propertyDamage * level switch
            {
                <= 2 => 1f,
                3 => 1.7f,
                4 => 2f,
                5 => 2.3f,
                6 => 2.6f
            };
            effectStack = 1;
            dotDamage = DefaultDamage * 0.2f;
            burnTime = EnforceManager.Instance.fire2BurnDurationBoost ? 5f:3f;
            var increaseRateBoost = EnforceManager.Instance.fire2RateBoost ? 0.85f : 1f;
            defaultAtkRate = 1f * increaseRateBoost;
            swingSpeed = 1f * increaseRateBoost;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Fire;
            UnitEffect = EnforceManager.Instance.fire2ChangeProperty? UnitEffects.None : UnitEffects.Burn;
        }
    }
}