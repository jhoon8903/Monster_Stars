using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.RewardScript;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        protected internal int UnitGameLevel { get; private set; } = 1;
        protected internal int CharacterPieceCount { get; set; }
        protected internal int CharacterMaxPiece => UnitGameLevel * 5;
        public enum UnitGrades { Green, Blue, Purple }
        protected internal UnitGrades UnitGrade;
        protected internal bool UnLock { get; private set; }
        protected internal bool Selected { get; set; }
        protected internal int UnitInGameLevel { get; protected set; } 
        protected internal string CharacterName;
        public enum Types { Character, Treasure }
        protected internal Types Type; 
        public enum UnitGroups { A,B,C,D,E,F,G,H,None }
        public enum UnitAtkTypes {None,  Projectile, GuideProjectile, Gas, Circle, Vibrate, Boomerang }
        public enum UnitProperties { Divine, Darkness, Physics, Water, Poison, Fire, None }
        public enum UnitEffects { Slow, Bleed, Poison, Burn, Stun, Strike, Restraint, Darkness, None } 
        public UnitGroups unitGroup = UnitGroups.None; 
        protected internal UnitAtkTypes UnitAtkType = UnitAtkTypes.None;
        protected internal UnitProperties UnitProperty = UnitProperties.None;
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
        private readonly Vector3 _levelUpScale = new Vector3(1.3f, 1.3f, 0); 
        public GameObject CurrentWeapon { get; set; }
        protected internal bool IsClicked { get; set; }
        protected static List<GameObject> detectedEnemies = new List<GameObject>();
        protected SpriteRenderer spriteRenderer;

        public void OnEnable()
        {
            if (PermanentLevelUp && UnitInGameLevel == 1)
            {
                UnitInGameLevel = 2;
            }
        }
        public virtual void Initialize()
        {
            UnLock = true;
            Selected = false;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void LevelUpScale(GameObject levelUpObject)
        {
            var sequence = DOTween.Sequence(); 
            Tween scaleUp = sequence.Append(levelUpObject.transform.DOScale(_levelUpScale, 0.3f)); 
            scaleUp.WaitForCompletion();
            LevelUp(); // Level up the character
            Tween scaleDown = sequence.Append(levelUpObject.transform.DOScale(_initialScale, 0.3f)); 
            scaleDown.WaitForCompletion();
        }

        protected virtual void LevelUp()
        {
            UnitInGameLevel++;
        }
        protected internal virtual void CharacterReset()
        {
            UnitInGameLevel = 1;
        }
        public virtual List<GameObject> DetectEnemies()
        {
            return new List<GameObject>();
        }
        public virtual Sprite GetSpriteForLevel(int characterObjectLevel)
        {
            return null;
        }
        public IEnumerator UnitLevelUp()
        {
            CharacterPieceCount -= CharacterMaxPiece;
            UnitGameLevel++;
            yield return null;
        }

        protected virtual Sprite GetSprite(int level)
        {
            return null;
        }
    }
}
