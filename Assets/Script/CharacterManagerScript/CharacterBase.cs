using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.RewardScript;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        // Robby Source
        protected internal int CharacterObjectLevel { get; private set; } = 1;
        protected internal int CharacterPieceCount { get; set; }
        protected internal int CharacterMaxPiece { get; private set; }

        public enum UnitGrades { Green, Blue, Purple }
        protected internal UnitGrades UnitGrade;
        protected internal bool UnLock { get; protected set; }
        protected internal bool Selected { get; set; }
        
        
        protected internal int Level { get; private set; } = 1; 
        protected internal string CharacterName;
        protected internal int UnitLevel; 
        public enum Types { Character, Treasure }
        protected internal Types Type; 
        public enum UnitGroups { A,B,C,D,E,F,G,H,None }
        public UnitGroups unitGroup = UnitGroups.None; 
        public enum UnitAtkTypes {None,  Projectile, GuideProjectile, Gas, Circle, Vibrate, Boomerang } 
        protected internal UnitAtkTypes UnitAtkType = UnitAtkTypes.None;
        public enum UnitProperties { Divine, Darkness, Physics, Water, Poison, Fire, None } 
        protected internal UnitProperties UnitProperty = UnitProperties.None;
        public enum UnitEffects { Slow, Bleed, Poison, Burn, Stun, Strike, Restraint, Darkness, None } 
        protected internal UnitEffects UnitEffect = UnitEffects.None;
        public float baseDamage;
        public float DefaultDamage
        {
            get
            {
                var increaseDamageAmount = EnforceManager.Instance.increaseAtkDamage;
                return baseDamage * (1.0f + (increaseDamageAmount / 100f));
            }
            protected set => baseDamage = value;
        }
        public float defaultAtkRate;
        public float projectileSpeed; 
        public float swingSpeed;
        public float defaultAtkDistance;
        public bool PermanentLevelUp { get; set; } 
        private readonly Vector3 _initialScale = Vector3.one; 
        private readonly Vector3 _levelUpScale = new Vector3(1.2f, 1.2f, 0); 
        public GameObject CurrentWeapon { get; set; }
        protected internal bool IsClicked { get; set; }
        protected static List<GameObject> detectedEnemies = new List<GameObject>();

        public void OnEnable()
        {
            if (PermanentLevelUp && UnitLevel == 1)
            {
                Level = 2;
            }
        }
        public virtual void Initialize()
        {
            CharacterMaxPiece = CharacterObjectLevel * 5;
        }
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
        protected virtual void LevelUp()
        {
            Level++;
        }
        protected internal virtual void CharacterReset()
        {
            ResetLevel();
        }
        protected internal void ResetLevel()
        {
            Level = 1;
        }
        public virtual List<GameObject> DetectEnemies()
        {
            return new List<GameObject>();
        }
        public virtual Sprite GetSpriteForLevel(int characterObjectLevel)
        {
            return null;
        }
        public void UnitLevelUp()
        {
            CharacterObjectLevel++;
            CharacterMaxPiece = CharacterObjectLevel * 5;
            CharacterPieceCount -= CharacterMaxPiece;
        }
    }
}
