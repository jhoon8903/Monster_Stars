using System;
using System.Collections.Generic;
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
        [SerializeField] private GameObject nextStageBtn;
        [SerializeField] private GameObject previousStageBtn;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private GameObject stageImage;
        [SerializeField] private Slider stageProgress;
        [SerializeField] private TextMeshProUGUI stageProgressText;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject continuePanel;
        [SerializeField] private GameObject confirmBtn;
        [SerializeField] private GameObject cancelBtn;
        public int Stage { get; private set; }
        public static MainPanel Instance { get; private set; }
        public List<int> clearStageList = new List<int>();

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            holdCharacterList.InstanceUnit();
            if (PlayerPrefs.HasKey("unitState"))
            {
                continuePanel.SetActive(true);
            }

            var stage = PlayerPrefs.GetInt("ClearStage", 1);
            UpdateProgress(stage);
            startBtn.GetComponent<Button>().onClick.AddListener(StartGame); 
            nextStageBtn.GetComponent<Button>().onClick.AddListener(NextStage);
            previousStageBtn.GetComponent<Button>().onClick.AddListener(PreviousStage);
            confirmBtn.GetComponent<Button>().onClick.AddListener(ContinueGame);
            cancelBtn.GetComponent<Button>().onClick.AddListener(CancelContinue);
        }

        private void Update()
        {
           if (Input.GetKeyDown(KeyCode.R))
           {
                PlayerPrefs.DeleteAll();
           }
        }

        private static void ContinueGame()
        {
            SceneManager.LoadScene("StageScene");
        }

        private void CancelContinue()
        {
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("EnforceData");
            PlayerPrefs.SetInt("CurrentWave",1);
            continuePanel.SetActive(false);
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
            var waveMaxValue = Stage switch
            {
                >= 1 and < 5 => 10,
                >= 5 and < 10 => 20,
                _ => 30
            };
            var listString = PlayerPrefs.GetString("ClearStageList", "");
            if (!string.IsNullOrEmpty(listString))
            {
                clearStageList = new List<int>(Array.ConvertAll(listString.Split(','), int.Parse));
            }
            var value = new int();
            value = clearStageList.Contains(value) ? waveMaxValue : PlayerPrefs.GetInt("ClearWave", 1);
            Stage = stage;
            stageText.text = $"스테이지 {Stage}";
            stageProgress.maxValue = waveMaxValue;
            stageProgress.value = value;
            stageProgressText.text = $"{stageProgress.value} / {stageProgress.maxValue}";
        }


        private void NextStage()
        {
            var clearedStage = PlayerPrefs.GetInt("ClearStage", 1);
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
