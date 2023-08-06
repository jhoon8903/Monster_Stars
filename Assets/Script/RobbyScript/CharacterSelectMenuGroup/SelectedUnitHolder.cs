using System.Collections.Generic;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{
    public class SelectedUnitHolder : MonoBehaviour
    {
        
        public static SelectedUnitHolder Instance { get; private set; }
        public List<CharacterBase> selectedUnit = new List<CharacterBase>();

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
