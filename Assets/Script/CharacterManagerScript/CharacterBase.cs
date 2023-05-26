using System;
using UnityEngine;
using DG.Tweening;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        protected int Level { get; private set; } = 1;
        protected internal string _characterName;
        protected int _damage;
        protected float _atkSpeed;
        protected float _range;
        protected int _spearForce;
        protected float _splashRange;

        public enum Unit_Property { Holy, Dark, Physics, Water, Venom, Fire, Stone, }
        protected internal Unit_Property _unitProperty;

        public enum Unit_Effect { Slow, Bleed, Venom, Burn, Stun, Strike, Restraint }
        protected internal Unit_Effect _unitEffect;

        public enum Type {  Boss, character, treasure }
        protected internal Type _type;
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