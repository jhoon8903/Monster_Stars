using UnityEngine;

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

        protected internal virtual void LevelUp()
        {
            Level++;
        }
    }
}

