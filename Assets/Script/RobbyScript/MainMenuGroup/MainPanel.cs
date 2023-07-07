using System;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
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
        [SerializeField] private GameObject nextStageBtn;
        [SerializeField] private GameObject previousStageBtn;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private GameObject stageImage;
        [SerializeField] private Slider stageProgress;
        [SerializeField] private TextMeshProUGUI stageProgressText;
        [SerializeField] private GameObject optionsPanel;
        private int Stage { get; set; }
        private const string ClearedStageKey = "ClearedStage";


        private void Start()
        {
            var stage = StageManager.Instance.currentStage;
            UpdateProgress(stage);
            holdCharacterList.InstanceUnit();
            startBtn.GetComponent<Button>().onClick.AddListener(StartGame); 
            nextStageBtn.GetComponent<Button>().onClick.AddListener(NextStage);
            previousStageBtn.GetComponent<Button>().onClick.AddListener(PreviousStage);
        }

        private void Update()
        {
           if (Input.GetKeyDown(KeyCode.R))
           {
                PlayerPrefs.DeleteAll();
           }
        }


        private void StartGame()
        {
            if ( SelectedUnitHolder.Instance.selectedUnit.Count == 4)
            {
                if (staminaScript.currentStamina >= 5)
                {
                    staminaScript.currentStamina -= 5;
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

        private void UpdateProgress(int stage)
        {
            var value = StageManager.Instance.clearWave;
            Stage = stage;
            stageText.text = $"스테이지 {Stage}";
            stageProgress.maxValue = StageManager.Instance.maxWaveCount;
            stageProgress.value = value;
            stageProgressText.text = $"{stageProgress.value} / {stageProgress.maxValue}";
        }

        private void NextStage()
        {
            var clearedStage = PlayerPrefs.GetInt(ClearedStageKey, 1);
            if (Stage < clearedStage)
            {
                Stage++;
                UpdateProgress(Stage);
            }
            else
            {
                warningPanel.SetActive(true);
                messageText.text = $"먼저 스테이지 {Stage}을/를 클리어 하셔야 합니다.";
            }
        }

        private void PreviousStage()
        {
            if (Stage <= 1) return;
            Stage--;
            UpdateProgress(Stage);
        }
    }
}
