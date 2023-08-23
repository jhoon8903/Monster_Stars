using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Ogre : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite; 
        [SerializeField] private Sprite level5Sprite;
        private float _detectionWidth;
        private float _detectionHeight;
        public int atkCount;
       
        public void Awake()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.Ogre;
            UnitProperty = UnitProperties.Darkness;
            UnitGrade = UnitGrades.G;
            UnitDesc = "Ogre Unit B / B Grade";
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
            if (EnforceManager.Instance.ogreRangeIncrease)
            {
                _detectionWidth = 5f;
                _detectionHeight = 5f;
            }
            else
            {
                _detectionWidth = 3f;
                _detectionHeight = 3f;
            }
            center = transform.position;
            size = new Vector2(_detectionWidth, _detectionHeight);
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
            UnitLevelDamage = unitPieceLevel > 1 ? unitPieceLevel * 2 + 8f : 0;
            Type = Types.Character;
            unitGroup = UnitGroups.Ogre;
            DefaultDamage = UnitLevelDamage + 38f * (1f + EnforceManager.Instance.darkAttackPowerBoost) * level switch
            {
               <= 2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            };
            knockBackTime = 0.2f;
            knockBackPower = 1f;
            slowTime = 1f;
            slowPower = 0.8f; 
            effectChance = 10;
            defaultAtkRate = 1.2f * (1f - EnforceManager.Instance.darkAttackSpeedBoost);
            projectileSpeed = 1f;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Darkness;
            UnitEffect = UnitEffects.None;
        }
    }
}
