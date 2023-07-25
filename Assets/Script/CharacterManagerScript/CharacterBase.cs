using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.RewardScript;
using Script.RobbyScript.TopMenuGroup;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        public int unitPieceLevel = 1;
        protected internal int CharacterPieceCount { get; set; }
        protected internal int CharacterMaxPiece => unitPieceLevel * 5;
        public enum UnitGrades { Green, Blue, Purple }
        protected internal UnitGrades UnitGrade;
        protected internal bool UnLock { get; private set; }
        protected internal bool Selected { get; set; }
        public int unitPuzzleLevel;
        public enum Types { Character, Treasure }
        protected internal Types Type; 
        public enum UnitGroups { A,B,C,D,E,F,G,H,None }
        public enum UnitAtkTypes {None, Projectile, GuideProjectile, Circle}
        public enum UnitProperties { Divine, Darkness, Physics, Water, Poison, Fire, None }
        public enum UnitEffects { Slow, Bleed, Poison, Burn, Bind, None } 
        public UnitGroups unitGroup = UnitGroups.None; 
        protected internal UnitAtkTypes UnitAtkType = UnitAtkTypes.None;
        protected internal UnitProperties UnitProperty = UnitProperties.None;
        protected internal UnitEffects UnitEffect = UnitEffects.None;
        public int baseDamage;
        public float DefaultDamage
        {
            get
            {
                var increaseDamageAmount = EnforceManager.Instance.increaseAtkDamage;
                return baseDamage * (1.0f + (increaseDamageAmount / 100f));
            }
            protected set => baseDamage = (int)value;
        }
        public float defaultAtkRate;
        public float projectileSpeed; 
        public float swingSpeed;
        public float defaultAtkDistance;
        private readonly Vector3 _initialScale = Vector3.one; 
        private readonly Vector3 _levelUpScale = new Vector3(1.3f, 1.3f, 0); 
        public GameObject CurrentWeapon { get; set; }
        protected internal bool IsClicked { get; set; }
        protected static List<GameObject> DetectedEnemies = new List<GameObject>();
        private SpriteRenderer _spriteRenderer;
        protected internal bool HasAttackSpeedBuff { get; set; }
        protected internal string UnitDesc { get; protected set; }

        public virtual void Initialize()
        {
            UnLock = true;
            Selected = false;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void LevelUpScale(GameObject levelUpObject)
        {
            var sequence = DOTween.Sequence(); 
            Tween scaleUp = sequence.Append(levelUpObject.transform.DOScale(_levelUpScale, 0.3f)); 
            scaleUp.WaitForCompletion();
            LevelUp();
            Tween scaleDown = sequence.Append(levelUpObject.transform.DOScale(_initialScale, 0.3f)); 
            scaleDown.WaitForCompletion();
        }

        protected virtual void LevelUp()
        {
            unitPuzzleLevel++;
        }
        protected internal virtual void CharacterReset()
        {
            unitPuzzleLevel = 1;
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
            CoinsScript.Instance.Coin -= unitPieceLevel * 500;
            unitPieceLevel++;
            yield return null;
        }

        protected internal virtual Sprite GetSprite(int level)
        {
            return null;
        }

        protected internal virtual void SetLevel(int level)
        {
            unitPuzzleLevel = level;
            _spriteRenderer.sprite = GetSprite(level);
        }
    }
}
