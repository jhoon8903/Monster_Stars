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
            UnitGrade = UnitGrades.Green;
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
            if (EnforceManager.Instance.water2BackAttack)
            {
                _detectionWidth = 0.8f;
                _detectionHeight = 18f;
                size = new Vector2(_detectionWidth, _detectionHeight);
                center = transform.position;
            }
            else
            {
                _detectionWidth = 0.8f;
                _detectionHeight = 9f;
                size = new Vector2(_detectionWidth, _detectionHeight);
                center = (Vector2)transform.position + Vector2.up * _detectionHeight / 2f;
            }

            if (EnforceManager.Instance.water2AdditionalProjectile)
            {
                _detectionWidth = 2.8f;
            }
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
            detectedEnemies = currentlyDetectedEnemies;
            return detectedEnemies;
        }

        public void OnDrawGizmos()
        {
            GetDetectionProperties(out var detectionSize, out var detectionCenter);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(detectionCenter, detectionSize);
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
            Type = Types.Character;
            unitGroup = UnitGroups.C;
            DefaultDamage = 60f * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            defaultAtkRate = 1.2f;
            defaultAtkDistance = 9f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Water;
            UnitEffect = UnitEffects.Slow;
        }
    }
}
