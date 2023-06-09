using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class Unit_A : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        [SerializeField] private Sprite level5Sprite; // Sprite for level 5
        private SpriteRenderer _spriteRenderer; // Reference to the SpriteRenderer component
        private const float DetectionWidth = 1f; // Width of detection box
        private const float DetectionHeight = 9f; // Height of detection box
        public void Awake()
        {
            unitGroup = UnitGroups.A;
            _spriteRenderer = GetComponent<SpriteRenderer>(); // Get the reference to the SpriteRenderer component attached to this object
            Level1(); // Set initial level to level 1
        }

        // Handles the logic when the character levels up
        protected override void LevelUp()
        {
            base.LevelUp();

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

        // Resets the character's state to its initial values
        protected internal override void CharacterReset()
        {
            ResetLevel(); // Reset the character's level
            Level1(); // Set level back to 1
        }

        // Detects enemies within a detection box and returns a list of their GameObjects
        public override List<GameObject> DetectEnemies()
        {
            // Detect enemies within a detection box
            var detectionSize = new Vector2(DetectionWidth-0.5f, DetectionHeight);
            var detectionCenter = (Vector2)transform.position + Vector2.up * DetectionHeight / 2f;
            var colliders = Physics2D.OverlapBoxAll(detectionCenter, detectionSize, 0f);
            var detectedEnemies = (from collider in colliders
                                   where collider.gameObject.CompareTag("Enemy")
                                   select collider.gameObject).ToList();
            return detectedEnemies;
        }

        // Draws a wire cube in the Scene view to visualize the detection box
        public void OnDrawGizmos()
        {
            // Draw a wire cube to visualize the detection box in the Scene view
            var detectionSize = new Vector3(DetectionWidth-0.5f, DetectionHeight, 0);
            var detectionCenter = transform.position + Vector3.up * DetectionHeight / 2f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(detectionCenter, detectionSize);
        }

        // Sets the properties for level 1 of the character
        private void Level1()
        {
            // Set properties for level 1
            CharacterName = "Unit_A_00";
            UnitLevel = 1;
            unitGroup = UnitGroups.A;
            Type = Types.Character;
            defaultDamage = 0;
            defaultAtkRate = 0;
            defaultAtkDistance = 0;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level1Sprite;
        }

        // Sets the properties for level 2 of the character
        private void Level2()
        {
            // Set properties for level 2
            CharacterName = "Unit_A_01";
            UnitLevel = 2;
            Type = Types.Character;
            unitGroup = UnitGroups.A;
            defaultDamage = 125f;
            defaultAtkRate = 1;
            defaultAtkDistance = 9f;
            projectileSpeed = 1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
        }

        // Sets the properties for level 3 of the character
        private void Level3()
        {
            // Set properties for level 3
            CharacterName = "Unit_A_02";
            UnitLevel = 3;
            Type = Types.Character;
            unitGroup = UnitGroups.A;
            defaultDamage *= 1.7f;
            defaultAtkRate = 0.3f;
            defaultAtkDistance = 12f;
            projectileSpeed = 1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
        }

        // Sets the properties for level 4 of the character
        private void Level4()
        {
            // Set properties for level 4
            CharacterName = "Unit_A_03";
            UnitLevel = 4;
            Type = Types.Character;
            unitGroup = UnitGroups.A;
            defaultDamage *= 2f;
            defaultAtkRate = 0.9f;
            defaultAtkDistance = 12;
            projectileSpeed = 1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
        }

        // Sets the properties for level 5 of the character
        private void Level5()
        {
            // Set properties for level 5
            CharacterName = "Unit_A_04";
            UnitLevel = 5;
            Type = Types.Character;
            unitGroup = UnitGroups.A;
            defaultDamage *= 2.3f;
            defaultAtkRate = 2.7f;
            defaultAtkDistance = 12;
            projectileSpeed = 1f;
            defaultAtkRange = Vector3.zero;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.Projectile;
            UnitProperty = UnitProperties.Divine;
            UnitEffect = UnitEffects.Restraint;
        }
    }
}
