using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.RobbyScript.MainMenuGroup
{
    public class MainPanel : MonoBehaviour
    {
        [SerializeField] private HoldCharacterList holdCharacterList;
        [SerializeField] private StaminaScript staminaScript;
        [SerializeField] private GameObject startBtn;
        [SerializeField] private GameObject warningPanel;
        [SerializeField] private TextMeshProUGUI messageText;

        private void Start()
        {
            holdCharacterList.InstanceUnit();
            startBtn.GetComponent<Button>().onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            if (SelectedUnitHolder.Instance.selectedUnit.Count == 4)
            {
                if (staminaScript.currentStamina >= 6)
                {
                    staminaScript.currentStamina -= 6;
                    staminaScript.StaminaUpdate();
                    staminaScript.SaveStaminaState();
                    SceneManager.LoadScene("StageScene");
                }
                else
                {
                    warningPanel.SetActive(true);
                    messageText.text = "스테미나가 부족합니다";
                }
            }
            else
            {
                warningPanel.SetActive(true);
                messageText.text = "유닛배치를 확인해주세요";
            }
        }
    }
}
