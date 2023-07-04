using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitB : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        [SerializeField] private Sprite level5Sprite; // Sprite for level 5

        private SpriteRenderer _spriteRenderer; // Reference to the SpriteRenderer component
        private float DetectionWidth = 3f;
        private float DetectinHeight = 3f;


        public override void Initialize()
        {
            unitGroup = UnitGroups.B;
            UnitProperty = UnitProperties.Darkness;
            UnitGrade = UnitGrades.Green;
            UnLock = true;
            Selected = false;
            base.Initialize();
        }
        public void Awake()
        {
            unitGroup = UnitGroups.B;
            _spriteRenderer = GetComponent<SpriteRenderer>(); // Get the reference to the SpriteRenderer component attached to this object
            Level1(); // Set initial level to level 1
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

        public override List<GameObject> DetectEnemies()
        {
            var detectionCenter = transform.position;
            var detectionSize = new Vector2(DetectionWidth, DetectinHeight);
            var colliders = Physics2D.OverlapBoxAll(detectionCenter, detectionSize,0f);
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
            var detectionSize = new Vector2(DetectionWidth, DetectinHeight);
            Gizmos.color = new Color(0.184f, 0.286f, 0.482f);
            Gizmos.DrawWireCube(detectionCenter,detectionSize);
        }

        // Sets the properties for level 1 of the character
        private void Level1()
        {
            CharacterName = "Unit_B_00";
            UnitLevel = 1;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            DefaultDamage = 0;
            defaultAtkRate = 0;
            defaultAtkDistance = 0;
            _spriteRenderer.sprite = level1Sprite;
            UnitProperty = UnitProperties.Darkness;
        }

        // Sets the properties for level 2 of the character
        private void Level2()
        {
            CharacterName = "Unit_B_01";
            UnitLevel = 2;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            DefaultDamage = 100f;
            defaultAtkRate = 0.8f + (1f * 0.17f * EnforceManager.Instance.darkIncreaseAtkSpeed);
            defaultAtkDistance = 12f;
            projectileSpeed = 1f;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.GuideProjectile;
            UnitProperty = UnitProperties.Darkness;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 3 of the character
        private void Level3()
        {
            CharacterName = "Unit_B_02";
            UnitLevel = 3;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            DefaultDamage *= 1.7f;
            defaultAtkRate = 0.8f + (1f * 0.17f * EnforceManager.Instance.darkIncreaseAtkSpeed);
            defaultAtkDistance = 12f;
            projectileSpeed = 1f;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.GuideProjectile;
            UnitProperty = UnitProperties.Darkness;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 4 of the character
        private void Level4()
        {
            CharacterName = "Unit_B_03";
            UnitLevel = 4;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            DefaultDamage *= 2f;
            defaultAtkRate = 0.8f + (1f * 0.17f * EnforceManager.Instance.darkIncreaseAtkSpeed);
            defaultAtkDistance = 12f;
            projectileSpeed = 1f;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.GuideProjectile;
            UnitProperty = UnitProperties.Darkness;
            UnitEffect = UnitEffects.None;
        }

        // Sets the properties for level 5 of the character
        private void Level5()
        {
            CharacterName = "Unit_B_04";
            UnitLevel = 5;
            Type = Types.Character;
            unitGroup = UnitGroups.B;
            DefaultDamage *= 2.3f;
            defaultAtkRate = 0.8f + (1f * 0.17f * EnforceManager.Instance.darkIncreaseAtkSpeed);
            defaultAtkDistance = 12f;
            projectileSpeed = 1f;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.GuideProjectile;
            UnitProperty = UnitProperties.Darkness;
            UnitEffect = UnitEffects.None;
        }
    }
}
