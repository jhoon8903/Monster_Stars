using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitF : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        [SerializeField] private Sprite level5Sprite; // Sprite for level 5
        private SpriteRenderer _spriteRenderer; // Reference to the SpriteRenderer component
        private float _detectionSize = 1.5f; // Size of the detection circle
        public void Awake()
        {
            unitGroup = UnitGroups.F;
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
            if (EnforceManager.Instance.poisonIncreaseAtkRange)
            {
                _detectionSize = 2.5f;
            }
            var colliders = Physics2D.OverlapCircleAll(detectionCenter, _detectionSize);
            foreach (var enemyObject in colliders)
            {
                if (!enemyObject.gameObject.CompareTag("Enemy") || !enemyObject.gameObject.activeInHierarchy) continue;
                var enemyBase = enemyObject.GetComponent<EnemyBase>();
                _currentlyDetectedEnemies.Add(enemyBase.gameObject);
            }
            return _currentlyDetectedEnemies;
        }

        private readonly List<GameObject> _currentlyDetectedEnemies = new List<GameObject>();
        protected internal override void DeleteList(EnemyBase enemyObject)
        {
            _currentlyDetectedEnemies.Remove(enemyObject.gameObject);
        }

        // Draws a wire sphere in the Scene view to visualize the detection circle
        public void OnDrawGizmos()
        {
            var detectionCenter = transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectionCenter, _detectionSize);
        }

        // Sets the properties for level 1 of the character
        private void Level1()
        {
            CharacterName = "Unit_F_00";
            UnitLevel = 1;
            Type = Types.Character;
            unitGroup = UnitGroups.F;
            defaultDamage = 0f;
            defaultAtkRate = 0f;
            defaultAtkDistance = 0f;
            _spriteRenderer.sprite = level1Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }

        // Sets the properties for level 2 of the character
        private void Level2()
        {
            CharacterName = "Unit_F_01";
            UnitLevel = 2;
            Type = Types.Character;
            unitGroup = UnitGroups.F;
            defaultDamage = 120f;
            defaultAtkRate = 1.2f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1.0f;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }

        // Sets the properties for level 3 of the character
        private void Level3()
        {
            CharacterName = "Unit_F_02";
            UnitLevel = 3;
            Type = Types.Character;
            unitGroup = UnitGroups.F;
            defaultDamage *= 1.7f;
            defaultAtkRate = 1.2f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1.0f;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }

        // Sets the properties for level 4 of the character
        private void Level4()
        {
            CharacterName = "Unit_F_03";
            UnitLevel = 4;
            Type = Types.Character;
            unitGroup = UnitGroups.F;
            defaultDamage *= 2.0f;
            defaultAtkRate = 1.2f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1.0f;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }

        // Sets the properties for level 5 of the character
        private void Level5()
        {
            CharacterName = "Unit_F_04";
            UnitLevel = 5;
            Type = Types.Character;
            unitGroup = UnitGroups.F;
            defaultDamage *= 2.3f;
            defaultAtkRate = 1.2f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1.0f;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Poison;
            UnitEffect = UnitEffects.Poison;
        }
    }
}