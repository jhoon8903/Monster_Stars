using System;
using UnityEngine;
using DG.Tweening;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        protected int Level { get; private set; } = 1;
        protected internal string _characterName;

        public enum Type {  Boss, character, treasure }
        protected internal Type _type;

        public enum Unit_AtkType { Arrow, GuideArrow, Gas, Circle, Vibrate, Boomerang }
        protected internal Unit_AtkType _unitAtkType;

        public enum Unit_Property { Divine, Darkness, Physics, Water, Poison, Fire }
        protected internal Unit_Property _unitProperty;

        public enum Unit_Effect { Slow, Bleed, Poison, Burn, Stun, Strike, Restraint }
        protected internal Unit_Effect _unitEffect;

        public float _defaultDamage;
        public float _increaseDamage;
        public float _defaultAtkSpeed;
        public float _increaseAtkSpeed;
        public float _projectileSpeed;
        public float _swingSpeed;
        public Vector3 _defaultAtkDistance;
        public Vector3 _increaseAtkDistance;
        public Vector3 _defaultAtkRange;
        public Vector3 _increaseAtkRange;
        public int _penetrate;









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
    }
}