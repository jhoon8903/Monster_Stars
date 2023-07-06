using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitA : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite; 
        [SerializeField] private Sprite level5Sprite;

        private const float DetectionWidth = 1f;
        private const float DetectionHeight = 8f;

        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.A;
            UnitProperty = UnitProperties.Divine;
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
            SetLevel(UnitInGameLevel);
        }
        protected internal override void CharacterReset()
        {
            base.CharacterReset();
            SetLevel(UnitInGameLevel);
        }

        private void GetDetectionProperties(out Vector2 size, out Vector2 center)
        {
            if (EnforceManager.Instance.divineAtkRange)
            {
                size = new Vector2(DetectionWidth - 0.5f, DetectionHeight * 2);
                center = transform.position;
            }
            else
            {
                size = new Vector2(DetectionWidth - 0.5f, DetectionHeight);
                center = (Vector2)transform.position + Vector2.up * DetectionHeight / 2f;
            }
        }

        public override List<GameObject> DetectEnemies()
        {
            GetDetectionProperties(out var detectionSize, out var detectionCenter);
            
            var colliders = Physics2D.OverlapBoxAll(detectionCenter, detectionSize, 0f);
            var currentlyDetectedEnemies = new List<GameObject>();
            foreach (var enemyObject in colliders)
            {
                if (!enemyObject.gameObject.CompareTag("Enemy") || !enemyObject.gameObject.activeInHierarchy) continue;
                var enemyBase = enemyObject.GetComponent<EnemyBase>();
                currentlyDetectedEnemies.Add(enemyBase.gameObject);
            }

            detectedEnemies = currentlyDetectedEnemies;
            return detectedEnemies;
        }

        public void OnDrawGizmos()
        {
            GetDetectionProperties(out var detectionSize, out var detectionCenter);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(detectionCenter, detectionSize);
        }

        protected override Sprite GetSprite(int level)
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
        private void SetLevel(int level)
        {
            CharacterName = $"Unit_A_0{level - 1}";
            UnitInGameLevel = level;
            Type = Types.Character;
            unitGroup = UnitGroups.A;
            DefaultDamage = 150f * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            defaultAtkRate = 1f;
            defaultAtkDistance = 9f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
            spriteRenderer.sprite = GetSprite(level);
        }
    }
}