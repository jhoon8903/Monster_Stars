using System;
using System.Collections.Generic;
using Script.AdsScript;
using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.CharacterSelectMenuGroup;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using TMPro;
using Unity.VisualScripting;
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
        [SerializeField] private GameObject startAdsBtn;
        [SerializeField] private GameObject warningPanel;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject nextStageBtn;
        [SerializeField] private GameObject previousStageBtn;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private GameObject stageImage;
        [SerializeField] private Slider stageProgress;
        [SerializeField] private TextMeshProUGUI stageProgressText;
        [SerializeField] private GameObject timeRewardBtn;
        [SerializeField] private GameObject continuePanel;
        [SerializeField] private GameObject confirmBtn;
        [SerializeField] private GameObject cancelBtn;
        private int LatestStage { get; set; }
        public int SelectStage { get; private set; }
        public int recordWave;
        public static MainPanel Instance { get; private set; }
        public void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
        }
        public void Start()
        {
            holdCharacterList.InstanceUnit();
            if (PlayerPrefs.HasKey("unitState"))
            {
                continuePanel.SetActive(true);
            }
            LatestStage = PlayerPrefs.GetInt("LatestStage", 1);
            SelectStage = LatestStage;
            var (maxWave, clearWave) = GetStageWave(LatestStage);
            UpdateProgress(LatestStage, maxWave, clearWave);
            startBtn.GetComponent<Button>().onClick.AddListener(StartGame); 
            nextStageBtn.GetComponent<Button>().onClick.AddListener(NextStage);
            previousStageBtn.GetComponent<Button>().onClick.AddListener(PreviousStage);
            confirmBtn.GetComponent<Button>().onClick.AddListener(ContinueGame);
            cancelBtn.GetComponent<Button>().onClick.AddListener(CancelContinue);
            timeRewardBtn.GetComponent<Button>().onClick.AddListener(OpenTimeReward);
            startAdsBtn.GetComponent<Button>().onClick.AddListener(AdsStart);
            if (LatestStage != 1) return;
            previousStageBtn.SetActive(false);
            nextStageBtn.SetActive(false);
        }
        private void Update()
        {
            try
            {
                if (!Input.GetKeyDown(KeyCode.R)) return;
                Debug.Log("All Data Delete Complete");
                PlayerPrefs.DeleteAll();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }
        private static void ContinueGame()
        {
            SceneManager.LoadScene("StageScene");
        }
        private void CancelContinue()
        {
            ReturnRobby();
            continuePanel.SetActive(false);
        }
        public void StartGame()
        {
            if (SelectedUnitHolder.Instance.selectedUnit.Count == 4)
            {
                if (staminaScript.CurrentStamina >= 5)
                {
                    if (PlayerPrefs.HasKey("Retry"))
                    {
                        PlayerPrefs.SetInt("Retry", 1);
                    }

                    if (!LoadingManager.Instance.isFirstContact)
                    {
                        staminaScript.CurrentStamina -= 5;
                    }
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
        private void UpdateProgress(int stage, int maxWave, int clearWave)
        {
            stageText.text = $"스테이지 {stage}";
            stageProgress.maxValue = maxWave;
            stageProgress.value = clearWave ;
            stageProgressText.text = $"{clearWave} / {maxWave}";
        }
        private (int maxWave, int clearWave) GetStageWave(int selectStage)
        {
            var maxWaveKey = $"{selectStage}Stage_MaxWave";
            var clearWaveKey = $"{selectStage}Stage_ClearWave";
            var maxWave = PlayerPrefs.GetInt(maxWaveKey, 10);
            var clearWave = PlayerPrefs.GetInt(clearWaveKey, 0);
            recordWave = PlayerPrefs.GetInt($"{SelectStage}Stage_RecordWave", 0);
            if (clearWave < recordWave)
            {
                clearWave = recordWave;
            }
            else
            {
                recordWave = clearWave;
                PlayerPrefs.SetInt($"{SelectStage}Stage_RecordWave", recordWave);
            }
            return (maxWave, clearWave);
        }
        private void NextStage()
        {
            if (SelectStage < LatestStage)
            {
                SelectStage++;
                var (maxWave, clearWave) = GetStageWave(SelectStage);
                UpdateProgress(SelectStage, maxWave, clearWave);
            }
            else
            {
                nextStageBtn.SetActive(false);
            }
        }
        private void PreviousStage()
        {
            if (SelectStage <= 1)
            {
                previousStageBtn.SetActive(false);
            }
            SelectStage--;
            var (maxWave, clearWave) = GetStageWave(SelectStage);
            UpdateProgress(SelectStage, maxWave, clearWave);
            nextStageBtn.SetActive(true);
        }
        private void ReturnRobby()
        {
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("EnforceData");
            PlayerPrefs.SetInt($"{LatestStage}Stage_ProgressWave",1);
            PlayerPrefs.SetInt("GridHeight", 6);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SelectScene");
        }
        private static void OpenTimeReward()
        {
            TimeRewardManager.Instance.OpenPanel();
        }
        private static void AdsStart()
        {
            AdsManager.Instance.ShowRewardedAd();
            AdsManager.Instance.ButtonTypes = AdsManager.ButtonType.AdsStart;
        }
    }
}
