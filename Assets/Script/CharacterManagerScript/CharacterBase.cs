using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.EnemyManagerScript;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        protected internal int Level { get; private set; } = 1; // Current level of the character
        protected internal string CharacterName; // Name of the character
        protected internal int UnitLevel; // Level of the unit
        public enum Types { Character, Treasure } // Types of characters
        protected internal Types Type; // Type of the character
        public enum UnitGroups { A,B,C,D,E,F,None } // Groups of units
        public UnitGroups unitGroup; // Group of the unit
        public enum UnitAtkTypes {None,  Projectile, GuideProjectile, Gas, Circle, Vibrate, Boomerang } // Attack types of units
        protected internal UnitAtkTypes UnitAtkType = UnitAtkTypes.None; // Attack type of the unit
        public enum UnitProperties { Divine, Darkness, Physics, Water, Poison, Fire, None } // Properties of units
        protected internal UnitProperties UnitProperty = UnitProperties.None; // Property of the unit
        public enum UnitEffects { Slow, Bleed, Poison, Burn, Stun, Strike, Restraint, None } // Effects of units
        protected internal UnitEffects UnitEffect = UnitEffects.None; // Effect of the unit
        public float defaultDamage; // Default damage of the unit
        public float defaultAtkRate; // Default attack rate of the unit
        public float projectileSpeed; // Projectile speed of the unit
        public float swingSpeed; // Swing speed of the unit
        public float defaultAtkDistance; // Default attack distance of the unit
        public Vector3 defaultAtkRange; // Default attack range of the unit
        public bool PermanentLevelUp { get; set; } = false; // Indicates if the unit has permanent level up
        private readonly Vector3 _initialScale = Vector3.one; // Initial scale of the character
        private readonly Vector3 _levelUpScale = new Vector3(1.2f, 1.2f, 0); // Scale to use when leveling up
        public List<GameObject> detectedEnemies = new List<GameObject>();
        public GameObject CurrentWeapon { get; set; }
        protected internal bool DivineAtkRange { get; set; } = false;
        protected internal bool PhysicIncreaseWeaponScale { get; set; } = false;
        protected internal bool PhysicAtkSpeed { get; set; } = false;
        protected internal bool PhysicIncreaseDamage { get; set; } = false;
        protected internal bool PoisonIncreaseAtkRange { get; set; } = false;


        public void OnEnable()
        {
            if (PermanentLevelUp && UnitLevel == 1)
            {
                Level = 2;
            }
        }

        // Perform scale animation when leveling up
        public void LevelUpScale(GameObject levelUpObject)
        {
            var sequence = DOTween.Sequence(); // Create a sequence for animations
            Tween scaleUp = sequence
                .Append(levelUpObject.transform
                    .DOScale(_levelUpScale, 0.3f)); // Scale up the object
            scaleUp.WaitForCompletion();
            LevelUp(); // Level up the character
            Tween scaleDown = sequence
                .Append(levelUpObject.transform
                    .DOScale(_initialScale, 0.3f)); // Scale down the object
            scaleDown.WaitForCompletion();
        }

        // Level up the character
        protected virtual void LevelUp()
        {
            Level++;
        }

        // Reset the character's level
        protected internal virtual void CharacterReset()
        {
            ResetLevel();
        }

        // Reset the level of the character
        protected internal void ResetLevel()
        {
            Level = 1;
        }

        // Detect enemies and return a list of detected enemy game objects
        public virtual List<GameObject> DetectEnemies()
        {
            return new List<GameObject>();
        }

        // Increase the default damage of the character by a given amount
        public void IncreaseDamage(int increaseAmount)
        {
            var percentageIncrease = (float)increaseAmount / 100;
            defaultDamage *= increaseAmount * percentageIncrease;
        }

        // Increase the default attack rate of the character by a given amount
        public void IncreaseAtkRate(int increaseAmount)
        {
            var percentageIncrease = (float)increaseAmount / 100;
            defaultAtkRate *= increaseAmount * percentageIncrease;
        }

        protected void OnEnemyKilled(object source, EventArgs args)
        {
            var enemyBase = (EnemyBase)source;
            enemyBase.EnemyKilled -= OnEnemyKilled;
            detectedEnemies.Remove(enemyBase.gameObject);
        }
    }
}
