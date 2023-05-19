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
        private AtkElementProperty _atkElementProperty;
        private SpecialAtkProperty _specialAtkProperty;
        private readonly Vector3 _initialScale = new Vector3(0.6f, 0.6f, 0.6f);
        private readonly Vector3 _levelUpScale = new Vector3(0.8f, 0.8f, 0.8f);
        
        protected internal void LevelUpScale(GameObject levelUpObject)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(levelUpObject.transform.DOScale(_levelUpScale, 0.1f));
            sequence.Append(levelUpObject.transform.DOScale(_initialScale, 0.1f));
            LevelUp();
        }

        protected virtual void LevelUp()
        {
            Level++;
        }
        
    }
    
}

