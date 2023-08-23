using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Berserker : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;
        [SerializeField] private Sprite level6Sprite;
        private Vector2 _detectionSize;
        public int atkCount;
        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.Berserker;
            UnitProperty = UnitProperties.Physics;
            UnitGrade = UnitGrades.B;
            UnitDesc = "Berserker Unit J / B Grade";
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

        private void GetDetectionProperties(out Vector2 size, out Vector2 center)
        {
            var rangeBoost = EnforceManager.Instance.berserkerRangeBoost ? 1 : 0;
            _detectionSize = EnforceManager.Instance.berserkerCastleCrushStatBoost ? new Vector2(5+rangeBoost,5+rangeBoost) : new Vector2(3+rangeBoost, 3+rangeBoost);
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


        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            UnitLevelDamage = unitPieceLevel > 1 ? unitPieceLevel * 6 : 0f;
            Type = Types.Character;
            unitGroup = UnitGroups.Berserker;
            var crushDamageBoost = EnforceManager.Instance.berserkerCastleCrushStatBoost ? 1.3f : 1f; 
            DefaultDamage = UnitLevelDamage + 49f * crushDamageBoost * level switch
            {
                <= 2 => 1f,
                3 => 1.7f,
                4 => 2f,
                5 => 2.3f,
                6 => 2.6f
            };
            dotDamage = DefaultDamage * 0.2f;
            bleedTime = EnforceManager.Instance.berserkerBleedTimeBoost ? 5f : 3f;
            var rateBoost = 1f - EnforceManager.Instance.physical2RateBoost;
            defaultAtkRate = 1f * rateBoost;
            projectileSpeed = 1f;
            defaultAtkDistance = 9f;
            UnitAtkType = UnitAtkTypes.Projectile;
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
                5 => level5Sprite,
                6 => level6Sprite
            };
        }
    }
}
