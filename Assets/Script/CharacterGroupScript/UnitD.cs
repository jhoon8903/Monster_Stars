using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitD : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        [SerializeField] private Sprite level5Sprite;

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.D;
            UnitProperty = UnitProperties.Physics;
            UnitGrade = UnitGrades.Green;
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
            size = EnforceManager.Instance.physicIncreaseWeaponScale ? 2.5f : 1.5f;
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

        // public void OnDrawGizmos()
        // {
        //     GetDetectionProperties(out var size, out var center);
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawWireSphere(center, size);
        // }

        public void ResetDamage()
        {
            EnforceManager.Instance.increasePhysicsDamage = 1f;
        }

        protected internal override void SetLevel(int level)
        {
            base.SetLevel(level);
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            DefaultDamage = 250f  * level switch
            {
                <=  2 => 1f,
                3 => 1.7f,
                4 => 2f,
                _ => 2.3f
            } * EnforceManager.Instance.increasePhysicsDamage;
            defaultAtkRate = 1f * EnforceManager.Instance.increasePhysicAtkSpeed ;
            swingSpeed = 1f * EnforceManager.Instance.increasePhysicAtkSpeed;
            defaultAtkDistance = 1f;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
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
    }
}
