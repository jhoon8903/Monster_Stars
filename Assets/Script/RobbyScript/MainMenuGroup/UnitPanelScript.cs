using Script.RobbyScript.CharacterSelectMenuGroup;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.MainMenuGroup
{
    public class UnitPanelScript : MonoBehaviour
    {
        [SerializeField] private SelectedCharacter selectedCharacter;

        private GameObject _unit1BackGround;
        private GameObject _unit1;
        private Slider _unit1LevelSlider;
        private Slider _unit1PieceSlider;

         private GameObject _unit2BackGround;
         private GameObject _unit2;
         private Slider _unit2LevelSlider;
         private Slider _unit2PieceSlider;

         private GameObject _unit3BackGround;
         private GameObject _unit3;
         private Slider _unit3LevelSlider;
         private Slider _unit3PieceSlider;

         private GameObject _unit4BackGround;
         private GameObject _unit4;
         private Slider _unit4LevelSlider;
         private Slider _unit4PieceSlider;
    }
}
