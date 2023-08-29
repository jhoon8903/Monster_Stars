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
        [SerializeField] private Image stageImage;
        [SerializeField] private List<Sprite> stageSprite;
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
        public const string IsLoadingKey = "IsLoading";

        public void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
            stageImage.sprite = stageSprite[PlayerPrefs.GetInt("LatestStage", 1) - 1];
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
            UpdateButtonStates();
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
            PlayerPrefs.SetString(IsLoadingKey, "true");
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
            PlayerPrefs.SetString(IsLoadingKey, "false");
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

                    if (PlayerPrefs.GetInt("TutorialKey", 1)!=1)
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
                    messageText.text = "Stamina is low";
                }
            }
            else
            {
                warningPanel.SetActive(true);
                messageText.text = "You need to Deploy 4 Units";
            }
        }
        private void UpdateProgress(int stage, int maxWave, int clearWave)
        {
            stageText.text = $"Stage {stage}";
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
                stageImage.sprite = PreviewStage(SelectStage);
                var (maxWave, clearWave) = GetStageWave(SelectStage);
                UpdateProgress(SelectStage, maxWave, clearWave);
            }
            UpdateButtonStates(); 
        }

        private void PreviousStage()
        {
            if (SelectStage > 1)
            {
                SelectStage--;
                stageImage.sprite = PreviewStage(SelectStage);
                var (maxWave, clearWave) = GetStageWave(SelectStage);
                UpdateProgress(SelectStage, maxWave, clearWave);
            }
            UpdateButtonStates();
        }

        private Sprite PreviewStage(int stage)
        {
            return stage switch
            {
                1 or 8 or 14 or 19 => stageSprite[0],
                2 or 6 or 12 or 17 => stageSprite[1],
                3 or 10 or 13 or 16 => stageSprite[2],
                4 or 7 or 11 or 18 => stageSprite[3],
                5 or 9 or 15 or 20 => stageSprite[4],
            };
        }

        private void UpdateButtonStates()
        {
            previousStageBtn.SetActive(SelectStage > 1);
            nextStageBtn.SetActive(SelectStage < LatestStage);
            if (SelectStage == LatestStage)
            {
                nextStageBtn.SetActive(false);
            }
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
