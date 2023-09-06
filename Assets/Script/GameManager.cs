using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.AdsScript;
using Script.CharacterGroupScript;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.PuzzleManagerGroup;
using Script.QuestGroup;
using Script.RewardScript;
using Script.UIManager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
// ReSharper disable All

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TutorialManager tutorialManager;
        [SerializeField] private CountManager countManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private int moveCount;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private BackGroundManager backgroundManager;
        [SerializeField] private GameObject commonRewardPanel;
        [SerializeField] private GameObject expRewardPanel;
        [SerializeField] private CastleManager castleManager;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private LevelUpRewardManager levelUpRewardManager;
        [SerializeField] private ExpManager expManager;
        [SerializeField] private CommonRewardManager commonRewardManager;
        [SerializeField] private Image speedUpImage;
        [SerializeField] private Sprite normalSpeedImage;
        [SerializeField] private Sprite doubleSpeedImage;
        [SerializeField] private GameObject bossArea;
        public List<GameObject> characterList = new List<GameObject>(); 
        public static GameManager Instance { get; private set; }
        public bool speedUp;
        public Vector3Int bossSpawnArea;
        public bool IsBattle { get; private set; }
        private bool isPanelClosed = false;
        public bool isInitWave;
        
        private void Awake()
        {
            Instance = this;
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
            PlayerPrefs.SetString("IsLoading", "true");
        }
        private void Start()
        {
            StartCoroutine(LoadGame());
        }
        private IEnumerator LoadGame()
        {
            swipeManager.isBusy = true;
            
            gridManager.GenerateInitialGrid(PlayerPrefs.GetInt("GridHeight", 6));
            var isTutorial = bool.Parse(PlayerPrefs.GetString("TutorialKey", "true"));
            if (isTutorial)
            {
                // 사용가능 무브 5회 [3match, 4match, 5match, nullSwap, commonReward match] 
                 countManager.Initialize(5);
                 // 튜토리얼용 매치 오브젝트 생성
                 StartCoroutine(tutorialManager.TutorialState());
                 swipeManager.isBusy = false;
            }
            else
            {
                if (StageManager.Instance != null)
                {                                                 
                    var stage = StageManager.Instance.SelectedStages();
                    StageManager.Instance.UpdateWaveText();
                    BackGroundManager.Instance.ChangedBackGround(stage);
                }

                if (PlayerPrefs.HasKey("unitState"))
                {
                    StartCoroutine(SpawnManager.LoadGameState());
                    EnforceManager.Instance.LoadEnforceData();
                    countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
                    expManager.LoadExp();
                    castleManager.LoadCastleHp();
                    if (StageManager.Instance != null && StageManager.Instance.currentWave % 10 == 0)
                    {
                       
                        bossSpawnArea = new Vector3Int(Random.Range(1, 5), 9, 0);
                        bossArea.SetActive(true);
                        bossArea.transform.position = new Vector3(bossSpawnArea.x, 3.5f, 0);
                        StartCoroutine(SoundManager.Instance.BossWave(SoundManager.Instance.bossWaveClip));
                    }
                }
                else
                {
                    countManager.Initialize(moveCount + AdsManager.Instance.adsMoveCount);
                    castleManager.castleCrushBoost = false;
                    AdsManager.Instance.adsMoveCount = 0;
                }
                StartCoroutine(spawnManager.PositionUpCharacterObject());
            }
            castleManager.castleCrushBoost = false;
            speedUp = true;
            GameSpeedSelect();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("stage_play", "play", PlayerPrefs.GetInt("LatestStage", 1));
            yield return null;
        }
        public IEnumerator Count0Call()
        {
            Debug.Log("ddddd");
            if (IsBattle) yield break;
            IsBattle = true;
            if (bool.Parse(PlayerPrefs.GetString("TutorialKey", "true")))
            {
                yield return StartCoroutine(tutorialManager.EndTutorial());
            }
            CoverUnit(true);
            cameraManager.CameraBattleSizeChange();
            backgroundManager.ChangeBattleSize();
            yield return new WaitForSecondsRealtime(0.5f);
            if (StageManager.Instance == null) yield break;
            StartCoroutine(StageManager.Instance.WaveController());
            AtkManager.Instance.CheckForAttack();
            GameSpeed();
            // var allUnits = FindObjectsOfType<CharacterBase>();
            // foreach (var unit in allUnits)
            // {
            //     if (unit is Fishman unitE)
            //     {
            //         unitE.ApplyAttackSpeedBuffToAllies();
            //     }
            // }
        }
        private void CoverUnit(bool value)
        {
            characterList = CharacterPool.Instance.UsePoolCharacterList();
            foreach (var unit in characterList)
            {
                unit.GetComponent<CharacterBase>().cover.SetActive(value);
                unit.GetComponent<CharacterBase>().cover.GetComponent<SpriteRenderer>().sortingOrder = 2;
                unit.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        }
        public IEnumerator ContinueOrLose()
        {
            if (!IsBattle) yield break;
            IsBattle = false;
            Time.timeScale = 1;
            AtkManager.Instance.ClearWeapons();
            yield return StartCoroutine(WaitForPanelToClose());
            if (isPanelClosed)
            {
                if (castleManager.HpPoint > 0)
                {
                    StageManager.Instance.SaveClearWave();
                    if (StageManager.Instance.isBossClear)
                    {
                        StageManager.Instance.alreadyBoss = false;
                        moveCount = 10;
                        PlayerPrefs.SetInt("moveCount", moveCount);
                        countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
                        bossArea.SetActive(false);
                        commonRewardManager.WaveRewardChance();
                        yield return StartCoroutine(WaitForPanelToClose());
                        spawnManager.BossStageClearRule();
                    }
                    else
                    {
                        moveCount = 6;
                        PlayerPrefs.SetInt("moveCount", moveCount);
                        countManager.Initialize(PlayerPrefs.GetInt("moveCount"));
                    }
                  
                    yield return StartCoroutine(InitializeWave());
                }
                else
                {
                    LoseGame();
                }
                AtkManager.Instance.weaponsList.Clear();
            }
            // var allUnits = FindObjectsOfType<CharacterBase>();
            // foreach (var unit in allUnits)
            // {
            //     if (unit is not Fishman { HasAttackSpeedBuff: true } || !EnforceManager.Instance.water2AttackSpeedBuffToAdjacentAllies) continue;
            //     unit.defaultAtkRate /= 0.9f;
            //     unit.HasAttackSpeedBuff = false;
            // }
        }
        private IEnumerator InitializeWave()
        {
            if (isInitWave) yield break;
            isInitWave = true;
            yield return StartCoroutine(WaitForPanelToClose());
            Time.timeScale = 1;
            expManager.SaveExp();
            castleManager.SaveCastleHp();
            EnforceManager.Instance.SaveEnforceData();
            if (EnforceManager.Instance.recoveryCastle)
            {
                castleManager.RecoverCastleHp();
            }
            castleManager.TookDamageLastWave = false;
            StartCoroutine(backgroundManager.ChangePuzzleSize());
            yield return StartCoroutine(cameraManager.CameraPuzzleSizeChange());
            enemyPool.ClearList();
           
            castleManager.castleCrushBoost = false;
            if (levelUpRewardManager.HasUnitInGroup(CharacterBase.UnitGroups.Orc))
            {
                AtkManager.Instance.groupDAtkCount = 0;
                FindObjectOfType<Orc>().groupDAtkRate = 0f;
            } else if (levelUpRewardManager.HasUnitInGroup(CharacterBase.UnitGroups.Skeleton))
            {
                AtkManager.Instance.groupFCount = 0;
                FindObjectOfType<Skeleton>().groupFDamage = 0f;
            }
            foreach (var unit in CharacterPool.Instance.pooledCharacters)
            {
                unit.GetComponent<CharacterBase>().cover.SetActive(false);
            }

            if (StageManager.Instance != null)
            {                                                 
                var stage = StageManager.Instance.SelectedStages();
                StageManager.Instance.UpdateWaveText();
                BackGroundManager.Instance.ChangedBackGround(stage);
            }

            if (StageManager.Instance != null && StageManager.Instance.currentWave % 10 == 0)
            {
                bossSpawnArea = new Vector3Int(Random.Range(1, 5), 9, 0);
                bossArea.SetActive(true);
                bossArea.transform.position = new Vector3(bossSpawnArea.x, 3.5f, 0);
                StartCoroutine(SoundManager.Instance.BossWave(SoundManager.Instance.bossWaveClip));
            }
            Quest.Instance.VictoryQuest();
            if (!StageManager.Instance.isBossClear)
            {
                spawnManager.AddToQueue(spawnManager.PositionUpCharacterObject());
            }
            if (StageManager.Instance != null) StageManager.Instance.isBossClear = false;
        }
        private void LoseGame()
        {
            if (StageManager.Instance != null)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("stage_fail", "fail", StageManager.Instance.latestStage);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("stage_fail", "wave", StageManager.Instance.currentWave);
            }

            Time.timeScale = 0;
            StartCoroutine(KillMotion());
            ClearRewardManager.Instance.ClearReward(false);
        }
        private static IEnumerator KillMotion()
        {
           yield return DOTween.KillAll(true);
        }
        public IEnumerator WaitForPanelToClose()
        {
            isPanelClosed = false;
            if (commonRewardPanel.activeSelf)
            {
                while (commonRewardPanel.activeSelf)
                {
                    yield return null;
                }
            }

            if (!expRewardPanel.activeSelf)
            {
                while (expRewardPanel.activeSelf)
                {
                    yield return null;
                }
            }
            isPanelClosed = true;
            yield return null;
        }
        public void GameSpeedSelect()
        {
            if (spawnManager.isTutorial) return;
            if (speedUp == false)
            {
                speedUp = true;
                speedUpImage.sprite = doubleSpeedImage;
                GameSpeed();
            }
            else
            {
                speedUp = false;
                speedUpImage.sprite = normalSpeedImage;
                GameSpeed();
            }
        }
        public void GameSpeed()
        {
            if (speedUp)
            {
                Time.timeScale = IsBattle ? 2 : 1;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        public static void ReturnRobby()
        {
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("EnforceData");
            PlayerPrefs.SetInt("GridHeight", 6);
            PlayerPrefs.SetInt($"{StageManager.Instance.latestStage}Stage_ProgressWave",1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SelectScene");
        }
    }
}