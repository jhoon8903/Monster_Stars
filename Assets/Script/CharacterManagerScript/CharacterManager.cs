using System.Collections.Generic;
using System.Linq;
using Script.RobbyScript.CharacterSelectMenuGroup;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private CharacterBase treasureBox;
        public List<CharacterBase> characterList = new List<CharacterBase>();
        public static CharacterManager Instance;
        public void Awake()
        {
            Instance = this;

            foreach (var unit in SelectedUnitHolder.Instance.selectedUnit.Where(unit => unit.selected))
            {
                characterList.Add(unit);
            }
            characterList.Add(treasureBox);
        }
    }
}
