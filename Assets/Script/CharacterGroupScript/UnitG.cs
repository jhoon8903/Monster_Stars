using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitG : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite; // Sprite for level 1
        [SerializeField] private Sprite level2Sprite; // Sprite for level 2
        [SerializeField] private Sprite level3Sprite; // Sprite for level 3
        [SerializeField] private Sprite level4Sprite; // Sprite for level 4
        [SerializeField] private Sprite level5Sprite; // Sprite for level 5
        private SpriteRenderer _spriteRenderer ;
        private float _detectionSize = 1.5f;
        private float _currentDamage;


        public override void Initialize()
        {
            unitGroup = UnitGroups.G;
            UnitProperty = UnitProperties.Fire;
            UnitGrade = UnitGrades.Green;
            UnLock = true;
            Selected = false;
            base.Initialize();
        }
        public void Awake()
        {
            unitGroup = UnitGroups.G;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Level1();
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
            if (EnforceManager.Instance.fireIncreaseAtkRange)
            {
                _detectionSize = 2.5f;
            }
            var colliders = Physics2D.OverlapCircleAll(detectionCenter,_detectionSize);
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectionCenter, _detectionSize);
        }
        private void Level1()
        {
            CharacterName = "Unit_G_00";
            UnitLevel = 1;
            Type = Types.Character;
            unitGroup = UnitGroups.G;
            defaultAtkRate = 0;
            defaultAtkDistance = 0;
            _spriteRenderer.sprite = level1Sprite;
            UnitProperty = UnitProperties.Fire;
        }
        private void Level2()
        {
            CharacterName = "Unit_G_01";
            UnitLevel = 2;
            Type = Types.Character;
            unitGroup = UnitGroups.G;
            DefaultDamage += 180f ;
            defaultAtkRate = 1.5f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1f;
            _spriteRenderer.sprite = level2Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Fire;
            UnitEffect = UnitEffects.Burn;
        }
        private void Level3()
        {
            CharacterName = "Unit_G_02";
            UnitLevel = 3;
            Type = Types.Character;
            unitGroup = UnitGroups.G;
            DefaultDamage *= 1.7f;
            defaultAtkRate = 1.5f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1f;
            _spriteRenderer.sprite = level3Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Fire;
            UnitEffect = UnitEffects.Burn;
        }
        private void Level4()
        {
            CharacterName = "Unit_G_03";
            UnitLevel = 4;
            Type = Types.Character;
            unitGroup = UnitGroups.G;
            DefaultDamage *= 2.0f;
            defaultAtkRate = 1.5f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1f;
            _spriteRenderer.sprite = level4Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Fire;
            UnitEffect = UnitEffects.Burn;
        }
        private void Level5()
        {
            CharacterName = "Unit_G_04";
            UnitLevel = 5;
            Type = Types.Character;
            unitGroup = UnitGroups.G;
            DefaultDamage *= 2.3f;
            defaultAtkRate = 1.5f;
            defaultAtkDistance = 1f;
            projectileSpeed = 1f;
            _spriteRenderer.sprite = level5Sprite;
            UnitAtkType = UnitAtkTypes.Gas;
            UnitProperty = UnitProperties.Fire;
            UnitEffect = UnitEffects.Burn;
        }
    }
}
