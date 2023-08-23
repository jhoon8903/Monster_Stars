using System;
using System.Linq;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RewardScript
{
    public class Dps : MonoBehaviour
    {
        [SerializeField] private Image unitBack;
        [SerializeField] private  Sprite greenBack;
        [SerializeField] private  Sprite blueBack;
        [SerializeField] private  Sprite purpleBack;
        [SerializeField] private  Image unitSprite;
        [SerializeField] private  TextMeshProUGUI unitDps;
        private Dps _unitDpsInstance;
        public void UnitDps(TextMeshProUGUI totalDps, Transform dpsGrid)
        { 
            Debug.Log("DPS Call!");
            if (_unitDpsInstance != null)
            {
                Destroy(_unitDpsInstance);
            }
            var unitList = EnforceManager.Instance.characterList.GroupBy(u => u.unitGroup).ToList();
            var totalDamage = 0;
            foreach (var group in unitList)
            {
                var groupDamage = 0;
                Sprite unitIcon = null;
                var unitGrades = CharacterBase.UnitGrades.G;
                foreach (var unit in group)
                {
                    var damage = PlayerPrefs.GetInt($"{unit.unitGroup}DPS", 0);
                    groupDamage += damage;
                    unitIcon = unit.GetSpriteForLevel(unit.unitPieceLevel);
                    unitGrades = unit.UnitGrade;
                    PlayerPrefs.DeleteKey($"{unit.unitGroup}DPS");
                }
                totalDamage += groupDamage;
                _unitDpsInstance = Instantiate(this, dpsGrid);
                _unitDpsInstance.unitBack.sprite = UnitBackGrade(unitGrades);
                _unitDpsInstance.unitSprite.sprite = unitIcon;
                _unitDpsInstance.unitDps.text = UnitDpsTranslate(groupDamage);
            }
            totalDps.text = UnitDpsTranslate(totalDamage);
        }

        private Sprite UnitBackGrade(CharacterBase.UnitGrades unitGrade)
        {
            var boxColor = unitGrade switch
            {
                CharacterBase.UnitGrades.G => greenBack,
                CharacterBase.UnitGrades.B => blueBack,
                CharacterBase.UnitGrades.P => purpleBack,
                _ => null
            };
            return boxColor;
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
