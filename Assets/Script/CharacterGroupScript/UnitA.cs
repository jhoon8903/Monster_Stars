using System.Collections.Generic;
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
        private const float DetectionWidth = 0.5f;
        private float _detectionHeight;
        public int atkCount;


        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.A;
            UnitProperty = UnitProperties.Divine;
            UnitGrade = UnitGrades.Blue;
            UnitDesc = "유닛A 입니다.";
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
            if (EnforceManager.Instance.divineDualAttack)
            {
                size = new Vector2(DetectionWidth, _detectionHeight * 2);
                center = transform.position;
            }
            else
            {
                size = new Vector2(DetectionWidth, _detectionHeight);
                center = (Vector2)transform.position + Vector2.up * _detectionHeight / 2f;
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
            UnitLevelDamage = unitPieceLevel > 1 ? unitPieceLevel * 2f + 1f : 0f;
            Type = Types.Character;
            unitGroup = UnitGroups.A;
            DefaultDamage = UnitLevelDamage + 29f * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            defaultAtkRate = 1.2f * (1f - EnforceManager.Instance.divineRateBoost);
            defaultAtkDistance = 9f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Bind;
        }
    }
}