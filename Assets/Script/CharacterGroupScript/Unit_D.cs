using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Unit_D : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        [SerializeField] private Sprite level5Sprite; // Sprite for level 5
        private SpriteRenderer _spriteRenderer; // Reference to the SpriteRenderer component
        private float _detectionSize; // Size of the detection circle
        private float _currentDamage;
        private float _increaseDamage;
        public void Awake()
        {
            unitGroup = UnitGroups.D;
            _increaseDamage = 1;
            defaultDamage = 200f;
            _spriteRenderer = GetComponent<SpriteRenderer>(); // Get the reference to the SpriteRenderer component attached to this object
            Level1(); // Set initial level to level 1
        }

        protected override void LevelUp()
        {
            base.LevelUp(); // Increment the level

            // Update the character's properties based on the current level
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

        // Detects enemies within a detection circle and returns a list of their GameObjects
        public override List<GameObject> DetectEnemies()
        {

            var detectionCenter = (Vector2)transform.position;
            if (PhysicIncreaseWeaponScale)
            {
                _detectionSize = 2.5f;
            }
            else
            {
                _detectionSize = 1.5f;
            }
            var colliders = Physics2D.OverlapCircleAll(detectionCenter, _detectionSize);
            var currentlyDetectedEnemies = new List<GameObject>();
            foreach (var enemyObject in colliders)
            {
                if (!enemyObject.gameObject.CompareTag("Enemy")) continue;
                var enemyBase = enemyObject.GetComponent<EnemyBase>();
                enemyBase.EnemyKilled += OnEnemyKilled;
                currentlyDetectedEnemies.Add(enemyObject.gameObject);
            }

            foreach (var detectedEnemy in detectedEnemies)
            {
                if (!currentlyDetectedEnemies.Contains(detectedEnemy))
                {
                    // Unsubscribe from EnemyKilled event.
                    var enemyBase = detectedEnemy.GetComponent<EnemyBase>();
                    enemyBase.EnemyKilled -= OnEnemyKilled;
                }
            }
            detectedEnemies = currentlyDetectedEnemies;

            return detectedEnemies;
        }

        // Draws a wire sphere in the Scene view to visualize the detection circle
        public void OnDrawGizmos()
        {
            var detectionCenter = transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(detectionCenter, _detectionSize);
        }

        public void IncreasedPhysicsDamage()
        {
            if (PhysicIncreaseDamage)
            {
                var atkUnitList = FindObjectOfType<CharacterPool>().UsePoolCharacterList();
                foreach (var units in atkUnitList
                             .Select(unit => unit.GetComponent<CharacterBase>())
                             .Where(units => units.unitGroup == UnitGroups.D && units.UnitLevel >= 2))
                {
                    _increaseDamage = 0.05f;
                }
            }
            else
            {
                _increaseDamage = 0f;
            }
        }

        public void ResetDamage()
        {
            _increaseDamage = 1;
        }

        // Sets the properties for level 1 of the character
        private void Level1()
        {
            CharacterName = "Unit_D_00";
            UnitLevel = 1;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            defaultAtkRate = 0;
            defaultAtkDistance = 0;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level1Sprite;
        }

        // Sets the properties for level 2 of the character
        private void Level2()
        {
            CharacterName = "Unit_D_01";
            UnitLevel = 2;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            defaultDamage += (defaultDamage * _increaseDamage);
            if (PhysicAtkSpeed)
            {
                defaultAtkRate = 0.5f * 1.5f ;
                swingSpeed = 2f * 1.5f;
            }
            else
            {
                defaultAtkRate = 0.5f;
                swingSpeed = 2f;
            }
            defaultAtkDistance = 1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 3 of the character
        private void Level3()
        {
            CharacterName = "Unit_D_02";
            UnitLevel = 3;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            defaultDamage *= 1.7f;
            if (PhysicAtkSpeed)
            {
                defaultAtkRate = 0.5f * 1.5f ;
                swingSpeed = 2f * 1.5f;
            }
            else
            {
                defaultAtkRate = 0.5f;
                swingSpeed = 2f;
            }
            defaultAtkDistance = 1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 4 of the character
        private void Level4()
        {
            CharacterName = "Unit_D_03";
            UnitLevel = 4;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            defaultDamage *= 2.0f;
            if (PhysicAtkSpeed)
            {
                defaultAtkRate = 0.5f * 1.5f ;
                swingSpeed = 2f * 1.5f;
            }
            else
            {
                defaultAtkRate = 0.5f;
                swingSpeed = 2f;
            }
            defaultAtkDistance = 1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 5 of the character
        private void Level5()
        {
            CharacterName = "Unit_D_04";
            UnitLevel = 5;
            Type = Types.Character;
            unitGroup = UnitGroups.D;
            defaultDamage *= 2.3f;
            if (PhysicAtkSpeed)
            {
                defaultAtkRate = 0.5f * 1.5f ;
                swingSpeed = 2f * 1.5f;
            }
            else
            {
                defaultAtkRate = 0.5f;
                swingSpeed = 2f;
            }
            defaultAtkDistance = 1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.Circle;
            UnitProperty = UnitProperties.Physics;
            UnitEffect = UnitEffects.None;
        }
    }
}
