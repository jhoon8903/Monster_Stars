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
        private float _detectionWidth; 
        private const float DetectionHeight = 9f; 

        public void Awake()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.H;
            UnitProperty = UnitProperties.Physics;
            UnitGrade = UnitGrades.Green;
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
            SetLevel(UnitInGameLevel);
        }

        protected internal override void CharacterReset()
        {
            base.CharacterReset();
            SetLevel(UnitInGameLevel);
        }

        private void GetDetectionProperties(out Vector2 size, out Vector2 center)
        {         
            _detectionWidth = EnforceManager.Instance.water2AdditionalProjectile ? 3f : 0.8f;
            center = (Vector2)transform.position + Vector2.up * DetectionHeight / 2f;
            size = new Vector2(_detectionWidth, DetectionHeight);
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
            detectedEnemies = currentlyDetectedEnemies;
            return detectedEnemies;
        }
        public void OnDrawGizmos()
        {
            GetDetectionProperties(out var size, out var center);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(center, size);
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
            CharacterName = $"Unit_H_0{level - 1}";
            UnitInGameLevel = level;
            Type = Types.Character;
            unitGroup = UnitGroups.H;
            DefaultDamage = 120f * level switch
            {
                <= 2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            defaultAtkRate = 1f * (1f + 0.17f * EnforceManager.Instance.physics2AdditionalAtkSpeed);
            defaultAtkDistance = 9f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.Bleed;
            spriteRenderer.sprite = GetSprite(level);
        }
    }
}
