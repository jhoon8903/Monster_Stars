using System.Collections.Generic;
using Script.RobbyScript.CharacterSelectMenuGroup;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class CharacterManager : MonoBehaviour
    {

        [SerializeField] private CharacterBase treasureBox;
        public List<CharacterBase> characterList = new List<CharacterBase>();
        private List<CharacterBase> _instanceUnit = new List<CharacterBase>();

        public void Awake()
        {
            _instanceUnit = SelectedUnitHolder.Instance.selectedUnit;
            foreach (var unit in _instanceUnit)
            {
                characterList.Add(unit);
            }
            characterList.Add(treasureBox);
        }
    }
}
