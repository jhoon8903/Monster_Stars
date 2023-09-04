using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.RewardScript;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class DpsConsole : MonoBehaviour
    {
        // dps ShortPanel;
        [SerializeField] private GameObject dpsBtn;
        [SerializeField] private GameObject dpsPanel;
        [SerializeField] private  Sprite greenBack;
        [SerializeField] private  Sprite blueBack;
        [SerializeField] private  Sprite purpleBack;

        // unit1
        [SerializeField] private Image unit1BackSprite;
        [SerializeField] private Image unit1DpsSprite;
        [SerializeField] private TextMeshProUGUI unit1DpsText;
        // unit2
        [SerializeField] private Image unit2BackSprite;
        [SerializeField] private Image unit2DpsSprite;
        [SerializeField] private TextMeshProUGUI unit2DpsText;
        // unit3
        [SerializeField] private Image unit3BackSprite;
        [SerializeField] private Image unit3DpsSprite;
        [SerializeField] private TextMeshProUGUI unit3DpsText;
        // unit4
        [SerializeField] private Image unit4BackSprite;
        [SerializeField] private Image unit4DpsSprite;
        [SerializeField] private TextMeshProUGUI unit4DpsText;

        private void Awake()
        {
            DOTween.Init();
            dpsBtn.GetComponent<Button>().onClick.AddListener(ToggleDps);
        }
        private void Start()
        {
            DpsConsoleUpdate();
        }

        private void Update()
        {
            DpsConsoleUpdate();
            if (!Input.GetMouseButtonDown(0)) return;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                var clickedObject = EventSystem.current.currentSelectedGameObject;
                if (clickedObject == dpsBtn) return;
                if (dpsPanel.activeInHierarchy && clickedObject != dpsPanel)
                {
                    CloseDps();
                }
            }
            else if (dpsPanel.activeInHierarchy)
            {
                CloseDps();
            }
        }
        private void ToggleDps()
        {
            if (dpsPanel.activeInHierarchy)
            {
                CloseDps();
            }
            else
            {
                OpenDps();
            }
        }
        private void SetForDps(IEnumerable<CharacterBase> unitGroups, Image back, Image unitSprites, TMP_Text dpsText)
        {
            foreach (var unitBase in unitGroups)
            {
                back.sprite = unitBase.UnitGrade switch
                {
                    CharacterBase.UnitGrades.G => greenBack,
                    CharacterBase.UnitGrades.B => blueBack,
                    CharacterBase.UnitGrades.P => purpleBack
                };
                unitSprites.sprite = unitBase.GetSpriteForLevel(3);
                dpsText.text = UnitDpsTranslate(PlayerPrefs.GetInt($"{unitBase.unitGroup}DPS", 0));
            }
        }
        private void DpsConsoleUpdate()
        {
            var unitList = EnforceManager.Instance.characterList.GroupBy(u => u.unitGroup).ToList();
            for (var i = 0; i < unitList.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        SetForDps(unitList[i], unit1BackSprite, unit1DpsSprite, unit1DpsText);
                        break;
                    case 1:
                        SetForDps(unitList[i], unit2BackSprite, unit2DpsSprite, unit2DpsText);
                        break;
                    case 2:
                        SetForDps(unitList[i], unit3BackSprite, unit3DpsSprite, unit3DpsText);
                        break;
                    case 3:
                        SetForDps(unitList[i], unit4BackSprite, unit4DpsSprite, unit4DpsText);
                        break;
                }
            }
        }
        private void OpenDps()
        {
            DpsConsoleUpdate();
            dpsPanel.transform.localScale = new Vector3(1, 0, 1);
            dpsPanel.SetActive(true);
            dpsPanel.transform.DOScaleY(1, 0.3f);
        }
        private void CloseDps()
        {
            dpsPanel.transform.DOScaleY(0, 0.3f)
                .OnComplete(() => dpsPanel.SetActive(false));
        }
        private static string UnitDpsTranslate(int damage)
        {
            var cumulativeDamage = damage switch
            {
                >= 1000000 => $"{damage / 1000000f:F2}M",
                >= 1000 => $"{damage / 1000f:F2}K",
                _ => $"{damage}"
            };
            return cumulativeDamage;
        }
    }
}