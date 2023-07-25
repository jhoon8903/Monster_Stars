using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitC : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite; 
        [SerializeField] private Sprite level4Sprite; 
        [SerializeField] private Sprite level5Sprite;
        private float _detectionWidth;
        private float _detectionHeight;

        public void Awake()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.C;
            UnitProperty = UnitProperties.Water;
            UnitGrade = UnitGrades.Purple;
            UnitDesc = "유닛C 입니다.";
            SetLevel(1);
        }
        public override Sprite GetSpriteForLevel(int characterObjectLevel)
        {
            return characterObjectLevel switch
            {
                <= 5 => level1Sprite,
                <= 10 => level2Sprite,
                <= 15 => level3Sprite,
                <= 20 => level4Sprite,
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
            size = new Vector2(_detectionWidth, _detectionHeight);
            center = (Vector2)transform.position + Vector2.up * _detectionHeight / 2f;
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
                _ => level5Sprite
            };
        }

        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            var unitLevelDamage = unitPieceLevel * 12f;
            Type = Types.Character;
            unitGroup = UnitGroups.C;
            defaultAtkRate = 0.8f / (1f + 6 * EnforceManager.Instance.darkAttackSpeedBoost / 100f);
            defaultAtkDistance = 9f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Water;
            var increaseDamage = 1f + EnforceManager.Instance.waterAttackBoost * 12 / 100f;
            DefaultDamage = unitLevelDamage + 60f * increaseDamage * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            UnitEffect = EnforceManager.Instance.waterAllyDamageBoost ? UnitEffects.None : UnitEffects.Slow;
        }
    }
}
