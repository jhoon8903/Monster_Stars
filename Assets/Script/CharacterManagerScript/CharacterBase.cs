using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        public int Level { get; private set; } = 1;
        protected string _characterName;
        protected int _damage;
        protected float _atkSpeed;
        protected float _range;
        protected int _spearForce;
        protected float _splashRange;
        private AtkElementProperty _atkElementProperty;
        private SpecialAtkProperty _specialAtkProperty;

        public void Setup(CharacterBase characterToCopy)
        {
            _characterName = characterToCopy._characterName;
            _damage = characterToCopy._damage;
            _atkSpeed = characterToCopy._atkSpeed;
            _range = characterToCopy._range;
            _spearForce = characterToCopy._spearForce;
            _splashRange = characterToCopy._splashRange;
            _atkElementProperty = characterToCopy._atkElementProperty;
            _specialAtkProperty = characterToCopy._specialAtkProperty;
        }

        public void LevelUp()
        {
            Debug.Log("Character Level UP ! ! ");
            // switch (_characterName)
            // {
            //     case "bear1":
            //         Level2();
            //         break;
            //     case "bear2":
            //         Level3();
            //         break;
            //     case "bear3":
            //         Level4();
            //         break;
            //     case "bear4":
            //         Level5();
            //         break;
            //     default:
            //         Debug.Log("Character is already at maximum level.");
            //         break;
            // }
        }
    }
}

