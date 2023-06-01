using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        protected internal int Level { get; private set; } = 1;
        protected internal string CharacterName;

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
        public float increaseDamage;
        public float defaultAtkRate;
        public float increaseAtkRate;
        public float projectileSpeed;
        public float swingSpeed;
        public float defaultAtkDistance;
        public float increaseAtkDistance;
        public Vector3 defaultAtkRange;
        public Vector3 increaseAtkRange;
        public int penetrate;

        private readonly Vector3 _initialScale = new Vector3(0.6f, 0.6f, 0.6f);
        private readonly Vector3 _levelUpScale = new Vector3(0.8f, 0.8f, 0.8f);

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

        protected internal virtual void LevelReset() { }

        public virtual List<GameObject> DetectEnemies()
        {
            return new List<GameObject>();
        }

    }
}