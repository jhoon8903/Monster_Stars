using System;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
using Script.RewardScript;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private SpawnManager spawnManager;
        //Pause Panel
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button pauseBtn;
        // SoundController
        [SerializeField] private GameObject volumeBtn;
        [SerializeField] private Sprite volumeOnSprite;
        [SerializeField] private Sprite volumeOffSprite;
        private bool _volume = true;
        [SerializeField] private GameObject soundBtn;
        [SerializeField] private Sprite soundOnSprite;
        [SerializeField] private Sprite soundOffSprite;
        private bool _sound = true;
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

        private void Start()
        {
            UnitSkillView();
            volumeBtn.GetComponent<Button>().onClick.AddListener(VolumeController);
            soundBtn.GetComponent<Button>().onClick.AddListener(SoundController);
            homeBtn.onClick.AddListener(Home);
            continueBtn.onClick.AddListener(Continue);
   
            pauseBtn.onClick.AddListener(OnPausePanel);
        }
        private void OnPausePanel()
        {
            if (spawnManager.isTutorial) return;
            pausePanel.SetActive(true);
            UnitSkillView();
            Time.timeScale = 0;
        }
        private void UpdateUnitSkillView(CharacterBase unit, Image unitBack, Image unitImage, Component unitSkillGrid)
        {
            unitImage.sprite = unit.GetSpriteForLevel(unit.unitPeaceLevel);
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
                if (characterBase.unitPeaceLevel < (int)skill["Level"])
                {
                    continue; // 해당 스킬이 조건을 만족하지 않으면 다음 스킬로 이동
                }

                var skillLevel = (int)skill["Level"];
                var isActive = activeSkills.TryGetValue(skillLevel, out var active) && active;

                var instance = Instantiate(unitSkillPrefabs, unitSkillGrid.transform);
                if (instance == null) continue;
                instance.GetComponent<Image>().sprite = isActive
                    ? gradeBack[(int)Enum.Parse(typeof(CharacterBase.UnitGrades), (string)skill["Grade"], true)]
                    : gradeBack[3];
                if (characterBase.UnitSkillDict.TryGetValue(skillLevel, out var skillSprite))
                {
                    instance.skillIcon.sprite = skillSprite;
                }
                instance.skillDesc.text = (string)skill["PopupDesc"];

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
        private void VolumeController()
        {
            if (_volume)
            {
                _volume = false;
                volumeBtn.GetComponent<Image>().sprite = volumeOffSprite;
            }
            else
            {
                _volume = true;
                volumeBtn.GetComponent<Image>().sprite = volumeOnSprite;
            }
        }
        private void SoundController()
        {
            if (_sound)
            {
                _sound = false;
                soundBtn.GetComponent<Image>().sprite = soundOffSprite;
            }
            else
            {
                _sound = true;
                soundBtn.GetComponent<Image>().sprite = soundOnSprite;
            }
        }
        private static void Home()
        {
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.DeleteKey("moveCount");
            PlayerPrefs.DeleteKey("GridHeight");
            PlayerPrefs.SetInt($"{StageManager.Instance.latestStage}Stage_ProgressWave",1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SelectScene");
        }
        private void Continue()
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
