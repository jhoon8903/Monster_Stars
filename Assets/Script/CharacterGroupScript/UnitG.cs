using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitG : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite; 
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite; 
        private float _currentDamage;

        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.G;
            UnitProperty = UnitProperties.Fire;
            UnitGrade = UnitGrades.Blue;
            UnitDesc = "유닛G 입니다.";
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

        private void GetDetectionProperties(out float size, out Vector2 center)
        {
            center = transform.position;
            size = EnforceManager.Instance.fire2SwordSizeIncrease ? 2.5f : 1.5f;
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
                _ => level5Sprite
            };
        }

        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            UnitLevelDamage = (unitPieceLevel-1) * 40f;
            Type = Types.Character;
            unitGroup = UnitGroups.G;
            var damageBoost = 1f + EnforceManager.Instance.fire2AttackPowerIncrease * 12 / 100f;
            DefaultDamage = UnitLevelDamage + 12f * damageBoost * level switch
            {
                <= 2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            var increaseRateBoost = 1f + EnforceManager.Instance.fire2AttackSpeedIncrease * 6 / 100f;
            defaultAtkRate = 1f / increaseRateBoost;
            defaultAtkDistance = 1f;
            swingSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Fire;
            UnitEffect = EnforceManager.Instance.fire2NoBurnDamageIncrease ? UnitEffects.None : UnitEffects.Burn;
        }
    }
}