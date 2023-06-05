using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.RewardScript;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        protected internal int Level { get; private set; } = 1;
        protected internal string CharacterName;

        protected internal int UnitLevel;
        public enum Types { Character, Treasure }
        protected internal Types Type;

        public enum UnitGroups { A,B,C,D,E,F,None }
        protected internal UnitGroups UnitGroup = UnitGroups.None;

        public enum UnitAtkTypes {None,  Projectile, GuideProjectile, Gas, Circle, Vibrate, Boomerang }
        protected internal UnitAtkTypes UnitAtkType = UnitAtkTypes.None;

        public enum UnitProperties { Divine, Darkness, Physics, Water, Poison, Fire, None }
        protected internal UnitProperties UnitProperty = UnitProperties.None;

        public enum UnitEffects { Slow, Bleed, Poison, Burn, Stun, Strike, Restraint, None }
        protected internal UnitEffects UnitEffect = UnitEffects.None;

        public float defaultDamage;
        // public float increaseDamage;
        public float defaultAtkRate;
        // public float increaseAtkRate;
        public float projectileSpeed;
        public float swingSpeed;
        public float defaultAtkDistance;
        // public float increaseAtkDistance;
        public Vector3 defaultAtkRange;
        // public Vector3 increaseAtkRange;
        // public int penetrate;

        private readonly Vector3 _initialScale = Vector3.one;
        private readonly Vector3 _levelUpScale = new Vector3(1.2f, 1.2f, 0);
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
            Level++;
        }
        protected internal virtual void CharacterReset()
        {
        }
        protected internal void ResetLevel()
        {
            Level = 1;
        }
        public virtual List<GameObject> DetectEnemies()
        {
            return new List<GameObject>();
        }
        
        // 전체 데미지 증가 버프
        public void IncreaseDamage(int increaseAmount)
        {
            var percentageIncrease = (float)increaseAmount / 100;
            defaultDamage *= increaseAmount * percentageIncrease;
        }

        public void IncreaseAtkRate(int increaseAmount)
        {
            var percentageIncrease = (float)increaseAmount / 100;
            defaultAtkRate *= increaseAmount * percentageIncrease;
        }

    }
}