using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
using Script.RewardScript;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private SpawnManager spawnManager;
        //Pause Panel
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button pauseBtn;
        // SoundController
        [SerializeField] private GameObject soundBtn;
        [SerializeField] private Sprite soundOnSprite;
        [SerializeField] private Sprite soundOffSprite;
        [SerializeField] private GameObject musicBtn;
        [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;

        // Unit1 Skill Grid
        [SerializeField] private List<Sprite> gradeBack;
        [SerializeField] private UnitSkillObject unitSkillPrefabs;
        [SerializeField] private GameObject unit1SkillGrid;
        [SerializeField] private Image unit1Back;
        [SerializeField] private Image unit1Image;
        // Unit2 Skill Grid
        [SerializeField] private GameObject unit2SkillGrid;
        [SerializeField] private Image unit2Back;
        [SerializeField] private Image unit2Image;
        // Unit3 Skill Grid
        [SerializeField] private GameObject unit3SkillGrid;
        [SerializeField] private Image unit3Back;
        [SerializeField] private Image unit3Image;
        // Unit4 Skill Grid
        [SerializeField] private GameObject unit4SkillGrid;
        [SerializeField] private Image unit4Back;
        [SerializeField] private Image unit4Image;
        // Bottom Button
        [SerializeField] private Button homeBtn;
        [SerializeField] private Button continueBtn;

        public UnitSkillObject unitSkillObject;

        public static PauseManager Instance;
        private void Awake()
        {
            Instance = this;
            musicBtn.GetComponent<Button>().onClick.AddListener(MusicController);
            soundBtn.GetComponent<Button>().onClick.AddListener(SoundController);
            SoundManager.Instance.music = bool.Parse(PlayerPrefs.GetString(SoundManager.MusicKey, "true"));
            SoundManager.Instance.sound = bool.Parse(PlayerPrefs.GetString(SoundManager.SoundKey, "true"));
            UpdateMusicState();
            UpdateSoundState();
            pausePanel.SetActive(false);
        }
        private void Start()
        {
            UnitSkillView();
            homeBtn.onClick.AddListener(Home);
            continueBtn.onClick.AddListener(Continue);
            pauseBtn.onClick.AddListener(OnPausePanel);
        }
        private void OnPausePanel()
        {
            if (spawnManager.isTutorial) return;
            pausePanel.SetActive(true);
            pausePanel.transform.localScale = Vector3.zero; 
            pausePanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                UnitSkillView();
                Time.timeScale = 0;
            });
    
        }
        private void UpdateUnitSkillView(CharacterBase unit, Image unitBack, Image unitImage, Component unitSkillGrid)
        {
            unitImage.sprite = unit.GetSpriteForLevel(unit.unitPieceLevel);
            unitBack.sprite = unit.UnitGrade switch
            {
                CharacterBase.UnitGrades.G => gradeBack[0],
                CharacterBase.UnitGrades.B => gradeBack[1],
                CharacterBase.UnitGrades.P => gradeBack[2],
                _ => gradeBack[3]
            };
            var unitSkillList = UnitSkills(unit);
            PopulateUnitSkillObject(unitSkillList, unit, unitSkillGrid);
        }
        private void PopulateUnitSkillObject(List<Dictionary<string, object>> skills, CharacterBase characterBase, Component unitSkillGrid)
        {
            var activeSkills = EnforceManager.Instance.GetActivatedSkills(characterBase.unitGroup);
            foreach (Transform child in unitSkillGrid.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var skill in skills)
            {
                if (characterBase.unitPieceLevel < (int)skill["Level"])
                {
                    continue; // 해당 스킬이 조건을 만족하지 않으면 다음 스킬로 이동
                }
                unitSkillObject = Instantiate(unitSkillPrefabs, unitSkillGrid.transform);
                var skillLevel = (int)skill["Level"];
                unitSkillObject.skillLevel = skillLevel;
                var isActive = activeSkills.TryGetValue(skillLevel, out var active) && active;
                if (unitSkillObject == null) continue;
                unitSkillObject.GetComponent<Image>().sprite = isActive
                    ? gradeBack[(int)Enum.Parse(typeof(CharacterBase.UnitGrades), (string)skill["Grade"], true)]
                    : gradeBack[3];
                if (characterBase.UnitSkillDict.TryGetValue(skillLevel, out var skillSpriteDict))
                {
                    unitSkillObject.skillIcon.sprite =
                        isActive ? skillSpriteDict.Keys.First() : skillSpriteDict.Values.First();
                }
                switch (unitSkillObject.skillLevel)
                {
                    case <= 3:
                        unitSkillObject.rightDesc.text =  (string)skill["PopupDesc"];
                        break;
                    case 5:
                        unitSkillObject.centerDesc.text =  (string)skill["PopupDesc"];
                        break;
                    default:
                        unitSkillObject.leftDesc.text = (string)skill["PopupDesc"];
                        break;
                }
                // instance.skillType.text = $"{skill["Type"]} / {skill["Level"]}";
            }
        }
        private static List<Dictionary<string, object>> UnitSkills(CharacterBase characterBase)
        {
            var unitSkillFile = Resources.Load<TextAsset>("SkillData");
            var unitSkillData = unitSkillFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var skillList = new List<Dictionary<string, object>>();
            for (var i = 1; i < unitSkillData.Length; i++)
            {
                var data = unitSkillData[i].Split(",");
                var unitGroup = (CharacterBase.UnitGroups)Enum.Parse(typeof(CharacterBase.UnitGroups), data[0], true);
                if (unitGroup != characterBase.unitGroup) continue;
                var unitSkillDict = new Dictionary<string, object>
                {
                    { "Grade", data[4] },
                    { "Level", int.Parse(data[1]) },
                    { "Type", data[3] },
                    { "EngDesc", data[6] },
                    { "PopupDesc", data[7]}
                };
                skillList.Add(unitSkillDict);
            }
            skillList.Sort((a, b) => {
                var levelComparison = ((int)a["Level"]).CompareTo((int)b["Level"]);
                return levelComparison == 0 ? string.Compare(((string)a["Grade"]), (string)b["Grade"], StringComparison.Ordinal) : levelComparison;
            });
            return skillList;
        }
        private void UnitSkillView()
        {
            var units = EnforceManager.Instance.characterList;
            UpdateUnitSkillView(units[0], unit1Back, unit1Image, unit1SkillGrid.transform);
            UpdateUnitSkillView(units[1], unit2Back, unit2Image, unit2SkillGrid.transform);
            UpdateUnitSkillView(units[2], unit3Back, unit3Image, unit3SkillGrid.transform);
            UpdateUnitSkillView(units[3], unit4Back, unit4Image, unit4SkillGrid.transform);
        }
        private void SoundController()
        {
            SoundManager.Instance.sound = !SoundManager.Instance.sound;
            PlayerPrefs.SetString(SoundManager.SoundKey, $"{SoundManager.Instance.sound}");
            PlayerPrefs.Save();
            UpdateSoundState();
        }
        private void UpdateSoundState()
        {
            SoundManager.Instance.IsSoundEnabled = SoundManager.Instance.sound;
            soundBtn.GetComponent<Image>().sprite = SoundManager.Instance.IsSoundEnabled ? soundOnSprite : soundOffSprite;
        }
        private void MusicController()
        {
            SoundManager.Instance.music = !SoundManager.Instance.music;
            PlayerPrefs.SetString(SoundManager.MusicKey, $"{SoundManager.Instance.music}");
            PlayerPrefs.Save();
            UpdateMusicState();
        }
        private void UpdateMusicState()
        {
            SoundManager.Instance.IsMusicEnabled = SoundManager.Instance.music;
            if (StageManager.Instance != null) SoundManager.Instance.StageSound(StageManager.Instance.selectStage, true);
            musicBtn.GetComponent<Image>().sprite = SoundManager.Instance.IsMusicEnabled ? musicOnSprite : musicOffSprite;
        }
        private static void Home()
        {
            PlayerPrefs.SetString("IsLoading", "true");
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("moveCount");
            PlayerPrefs.DeleteKey("GridHeight");
            foreach (var unit in EnforceManager.Instance.characterList)
            {
                PlayerPrefs.DeleteKey($"{unit.unitGroup}DPS");
            }
            if (StageManager.Instance != null) PlayerPrefs.SetInt($"{StageManager.Instance.latestStage}Stage_ProgressWave", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SelectScene");
        }
        private void Continue()
        {
            pausePanel.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                pausePanel.SetActive(false);
            });
            GameManager.Instance.GameSpeed();

        }
    }
}
