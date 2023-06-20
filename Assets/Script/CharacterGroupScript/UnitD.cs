using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitD : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        [SerializeField] private Sprite level5Sprite; // Sprite for level 5
        private SpriteRenderer _spriteRenderer ; 
        private float _detectionSize; 
        private float _currentDamage;
        
        public void Awake()
        {
            unitGroup = UnitGroups.D;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Level1();
        }
        protected override void LevelUp()
        {
            base.LevelUp();
            switch (Level)
            {
                case 2:
                    Level2(); // Set properties for level 2
                    break;
                case 3:
                    Level3(); // Set properties for level 3
                    break;
                case 4:
                    Level4(); // Set properties for level 4
                    break;
                case 5:
                    Level5(); // Set properties for level 5
                    break;
                default:
                    return;
            }
        }
        protected internal override void CharacterReset()
        {
            ResetLevel(); // Reset the character's level
            Level1(); // Set level back to 1
        }

        public override List<GameObject> DetectEnemies()
        {
            var detectionCenter = (Vector2)transform.position;
            _detectionSize = EnforceManager.Instance.physicIncreaseWeaponScale ? 2.5f : 1.5f;
            var colliders = Physics2D.OverlapCircleAll(detectionCenter, _detectionSize);
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
            var detectionCenter = transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(detectionCenter, _detectionSize);
        }
        public void ResetDamage()
        {
            EnforceManager.Instance.increasePhysicsDamage = 1f;
        }
        private void Level1()
        {
            CharacterName = "Unit_D_00";
            UnitLevel = 1;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            DefaultAtkRate = 0;
            defaultAtkDistance = 0;
            _spriteRenderer.sprite = level1Sprite;
        }
        private void Level2()
        {
            CharacterName = "Unit_D_01";
            UnitLevel = 2;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            DefaultDamage += (250f * EnforceManager.Instance.increasePhysicsDamage);
            DefaultAtkRate = 1f * EnforceManager.Instance.increasePhysicAtkSpeed ;
            swingSpeed = 2f * EnforceManager.Instance.increasePhysicAtkSpeed;
            defaultAtkDistance = 1f;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }
        private void Level3()
        {
            CharacterName = "Unit_D_02";
            UnitLevel = 3;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            DefaultDamage *= 1.7f;
            DefaultAtkRate = 1f * EnforceManager.Instance.increasePhysicAtkSpeed ;
            swingSpeed = 2f * EnforceManager.Instance.increasePhysicAtkSpeed;
            defaultAtkDistance = 1f;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }
        private void Level4()
        {
            CharacterName = "Unit_D_03";
            UnitLevel = 4;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            DefaultDamage *= 2.0f;
            DefaultAtkRate = 1f * EnforceManager.Instance.increasePhysicAtkSpeed ;
            swingSpeed = 2f * EnforceManager.Instance.increasePhysicAtkSpeed;
            defaultAtkDistance = 1f;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }
        private void Level5()
        {
            CharacterName = "Unit_D_04";
            UnitLevel = 5;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            DefaultDamage *= 2.3f;
            DefaultAtkRate = 1f * EnforceManager.Instance.increasePhysicAtkSpeed ;
            swingSpeed = 2f * EnforceManager.Instance.increasePhysicAtkSpeed;
            defaultAtkDistance = 1f;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }
    }
}
