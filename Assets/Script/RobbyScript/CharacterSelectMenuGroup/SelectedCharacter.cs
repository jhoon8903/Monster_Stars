using System.Collections.Generic;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.CharacterSelectMenuGroup
{
    public class SelectedCharacter : MonoBehaviour
    {
        [Header("\nUnit1")] 
        [SerializeField] private GameObject unit1BackGround;
        [SerializeField] private GameObject unit1;
        [SerializeField] private GameObject unit1Shadow;
        [SerializeField] private Slider unit1LevelSlider;
        [SerializeField] private TextMeshProUGUI unit1LevelText;
        [SerializeField] private Slider unit1PieceSlider;
        [SerializeField] private TextMeshProUGUI unit1PieceText;

        [Header("\nUnit2")] 
        [SerializeField] private GameObject unit2BackGround;
        [SerializeField] private GameObject unit2;
        [SerializeField] private GameObject unit2Shadow;
        [SerializeField] private Slider unit2LevelSlider;
        [SerializeField] private TextMeshProUGUI unit2LevelText;
        [SerializeField] private Slider unit2PieceSlider;
        [SerializeField] private TextMeshProUGUI unit2PieceText;
                
        [Header("\nUnit3")] 
        [SerializeField] private GameObject unit3BackGround;
        [SerializeField] private GameObject unit3;
        [SerializeField] private GameObject unit3Shadow;
        [SerializeField] private Slider unit3LevelSlider;
        [SerializeField] private TextMeshProUGUI unit3LevelText;
        [SerializeField] private Slider unit3PieceSlider;
        [SerializeField] private TextMeshProUGUI unit3PieceText;
        
        [Header("\nUnit4")] 
        [SerializeField] private GameObject unit4BackGround;
        [SerializeField] private GameObject unit4;
        [SerializeField] private GameObject unit4Shadow;
        [SerializeField] private Slider unit4LevelSlider;
        [SerializeField] private TextMeshProUGUI unit4LevelText;
        [SerializeField] private Slider unit4PieceSlider;
        [SerializeField] private TextMeshProUGUI unit4PieceText;

        [Header("\nScripts")]
        [SerializeField] private HoldCharacterList holdCharacterList;
        public List<CharacterBase> selectedCharacters = new List<CharacterBase>();

        public void AddCharacter()
        {
            // Ensure there are no more than 4 characters selected.
            if (selectedCharacters.Count < 4)
            {
                // Get the character information.
                var unitInfo = holdCharacterList.CharacterInformation();
                if (unitInfo == null) return;  // The character index was out of range.
        
                // Add the character to the list.
                selectedCharacters.Add(unitInfo.unit.GetComponent<CharacterBase>());

                // Update the UI elements.
                // This will depend on which slot is currently empty.
                if (unit1.GetComponent<Image>().sprite == null)
                {
                    UpdateUI(unitInfo, unit1, unit1LevelSlider, unit1LevelText, unit1PieceSlider, unit1PieceText);
                }
                else if (unit2.GetComponent<Image>().sprite == null)
                {
                    UpdateUI(unitInfo, unit2, unit2LevelSlider, unit2LevelText, unit2PieceSlider, unit2PieceText);
                }
                // Repeat for unit3 and unit4...
            }
        }
    }
}
