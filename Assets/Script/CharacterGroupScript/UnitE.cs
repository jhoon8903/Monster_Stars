using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitE : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite; 
        [SerializeField] private Sprite level4Sprite; 
        [SerializeField] private Sprite level5Sprite;
        private const float DetectionWidth = 0.8f;
        private float _detectionHeight;

        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.E;
            UnitProperty = UnitProperties.Water;
            UnitGrade = UnitGrades.Green;
            UnitDesc = "Fishman Unit E / Green Grade";
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
            size = new Vector2(DetectionWidth, _detectionHeight * 2f);
            center = transform.position;
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

        // private IEnumerable<GameObject> DetectAllies()
        // {
        //     var center = transform.position;
        //     const float detectWidth = 3f;
        //     const float detectHeight = 3f;
        //     var size = new Vector2(detectWidth, detectHeight);
        //     var hitColliders = Physics2D.OverlapBoxAll(center, size, 0f);
        //     var allies = hitColliders
        //         .Where(x =>
        //         {
        //             GameObject ally;
        //             return (ally = x.gameObject).CompareTag("Character") && ally.activeInHierarchy;
        //         })
        //         .Select(x => x.gameObject).ToList();
        //     return allies;
        // }

        // public void ApplyAttackSpeedBuffToAllies()
        // {
        //     if (!EnforceManager.Instance.water2AttackSpeedBuffToAdjacentAllies) return;
        //     var allies = DetectAllies();
        //     foreach (var characterBase in allies
        //                  .Select(ally => ally.GetComponent<CharacterBase>())
        //                  .Where(characterBase => characterBase != null))
        //     {
        //         characterBase.defaultAtkRate *= 0.9f;
        //         characterBase.HasAttackSpeedBuff = true;
        //     }
        // }

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
            UnitLevelDamage = unitPeaceLevel > 1 ? unitPeaceLevel * 2f + 1f : 0f;
            Type = Types.Character;
            unitGroup = UnitGroups.E;
            var increaseDamage = 1f + EnforceManager.Instance.water2DamageBoost;
            DefaultDamage = UnitLevelDamage + 17f * increaseDamage * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            slowTime = 1.5f + EnforceManager.Instance.water2SlowTimeBoost;
            freezeTime = EnforceManager.Instance.water2FreezeTimeBoost ? 1.5f : 1f;
            slowPower = EnforceManager.Instance.water2SlowPowerBoost ? 0.6f : 0.7f;
            effectChance = EnforceManager.Instance.water2FreezeChanceBoost ? 20 : 10;
            defaultAtkRate = 1f;
            defaultAtkDistance = 9f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Water;
            UnitEffect = UnitEffects.Slow;
            // ApplyAttackSpeedBuffToAllies();
        }
    }
}
