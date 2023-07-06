using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Script.CharacterGroupScript
{
    public class UnitG : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        [SerializeField] private Sprite level5Sprite; // Sprite for level 5
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

        private void GetDetectionProperties(out float size, out Vector2 center)
        {
            center = transform.position;
            size = EnforceManager.Instance.fireIncreaseAtkRange ? 2.5f : 1.5f;
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
            detectedEnemies = currentlyDetectedEnemies;
            return detectedEnemies;
        }

        public void OnDrawGizmos()
        {
            GetDetectionProperties(out var size, out var center);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, size);
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
            CharacterName = $"Unit_G_0{level - 1}";
            UnitInGameLevel = level;
            Type = Types.Character;
            unitGroup = UnitGroups.G;
            DefaultDamage = 180f * level switch
            {
                <= 2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            defaultAtkRate = 1.5f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Fire;
            UnitEffect = UnitEffects.Burn;
            spriteRenderer.sprite = GetSprite(level);
        }
    }
}