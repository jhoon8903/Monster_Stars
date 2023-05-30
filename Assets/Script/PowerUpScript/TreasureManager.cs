using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Script.PowerUpScript
{
    public class TreasureManager : MonoBehaviour
    {
        [SerializeField] public GameObject treasurePanel;
        [SerializeField] private TextMeshProUGUI power1Text;
        [SerializeField] private TextMeshProUGUI power2Text;
        [SerializeField] private TextMeshProUGUI power3Text;
        [SerializeField] private Image powerOption1Button;
        [SerializeField] private Image powerOption2Button;
        [SerializeField] private Image powerOption3Button;
        [SerializeField] private TextMeshProUGUI power1Code;
        [SerializeField] private TextMeshProUGUI power2Code;
        [SerializeField] private TextMeshProUGUI power3Code;
        [SerializeField] internal Sprite greenBtn;
        [SerializeField] internal Sprite blueBtn;
        [SerializeField] internal Sprite purpleBtn;
        [SerializeField] private Common common;
        [SerializeField] private Exp exp;
        // [SerializeField] private StagePowerUp stagePowerUp;
        private readonly Queue<GameObject> _pendingTreasure = new Queue<GameObject>();
        private GameObject _currentTreasure = null;

        public void EnqueueAndCheckTreasure(GameObject treasure)
        {
            var shake = treasure.transform.DOShakeScale(1.0f, 0.5f, 8);
            shake.OnComplete(() =>
                {
                    if (_currentTreasure == null)
                    {
                        _currentTreasure = treasure;
                        StartCoroutine(HandleTreasure(_currentTreasure));
                    }
                    else
                    {
                        _pendingTreasure.Enqueue(treasure);
                    }
                }
            );
        }
        private IEnumerator HandleTreasure(GameObject treasure)
        {
            Time.timeScale = 0;
            treasurePanel.SetActive(true);
            var treasureChestName = treasure.GetComponent<CharacterBase>()._characterName;
            switch (treasureChestName)
            {
                case "Unit_Treasure00":
                    break;
                case "Unit_Treasure01":
                    yield return StartCoroutine(Treasure1Reward());
                    break;
                case "Unit_Treasure02":
                    yield return StartCoroutine(Treasure2Reward());
                    break;
                case "Unit_Treasure03":
                    yield return StartCoroutine(Treasure3Reward());
                    break;
            }
            yield return new WaitUntil(() => treasurePanel.activeSelf == false);
        }
        public void PowerUpSelected()
        {
            treasurePanel.SetActive(false);
            CharacterPool.ReturnToPool(_currentTreasure);
            if (_pendingTreasure.Count > 0)
            {
                _currentTreasure = _pendingTreasure.Dequeue(); ;
                Time.timeScale = 0;
                StartCoroutine(HandleTreasure(_currentTreasure));
                treasurePanel.SetActive(true);
            }
            else
            {
                _currentTreasure = null;
                Time.timeScale = 1;
                
            }
        }
        private IEnumerator Treasure1Reward()
        {
            var powerUps = CommonSelectPower(70, 25, 5);
            CommonDisplayPowerUps(powerUps);
            yield return null;
        }
        private IEnumerator Treasure2Reward()
        {
            var powerUps = CommonSelectPower(30, 55, 15);
            CommonDisplayPowerUps(powerUps);
            yield return null;
        }
        private IEnumerator Treasure3Reward()
        {
            var powerUps = CommonSelectPower(0, 50, 50);
            CommonDisplayPowerUps(powerUps);
            yield return null;
        }
        public static int IncreaseRange(IReadOnlyList<int> rangeValues)
        {
            var random = new System.Random();
            var randomIndex = random.Next(rangeValues.Count);
            return rangeValues[randomIndex];
        }
        private List<CommonData> CommonSelectPower(int greenChance, int blueChance, int purpleChance)
        {
            var commonPowerUps = new List<CommonData>();
            var selectedCodes = new HashSet<int>();

            for (var i = 0; i < 3; i++)
            {
                var total = greenChance + blueChance + purpleChance;
                var randomValue = Random.Range(0, total);

                CommonData selectedPowerUp = null;

                if (randomValue < greenChance)
                {
                    selectedPowerUp = SelectUniquePowerUp(common.CommonGreenList, selectedCodes);
                }
                else if (randomValue < greenChance + blueChance)
                {
                    selectedPowerUp = SelectUniquePowerUp(common.CommonBlueList, selectedCodes);
                }
                else
                {
                    selectedPowerUp = SelectUniquePowerUp(common.CommonPurpleList, selectedCodes);
                }

                if (selectedPowerUp == null) continue;
                commonPowerUps.Add(selectedPowerUp);
                selectedCodes.Add(selectedPowerUp.Code);
            }

            return commonPowerUps;
        }
        private static CommonData SelectUniquePowerUp(IEnumerable<CommonData> powerUps, ICollection<int> selectedCodes)
        {
            var validOptions = powerUps.Where(p => !selectedCodes.Contains(p.Code)).ToList();

            if (validOptions.Count == 0)
            {
                return null;
            }

            var randomIndex = Random.Range(0, validOptions.Count);
            return validOptions[randomIndex];
        }
        private void CommonDisplayPowerUps(IReadOnlyList<CommonData> powerUps)
        {
            treasurePanel.SetActive(true);

            power1Text.text = $"{powerUps[0].Type} {powerUps[0].Property[0]}% PowerUp_Property";
            powerOption1Button.sprite = powerUps[0].Image;
            power1Code.text = $"{powerUps[0].Code}";

            power2Text.text = $"{powerUps[1].Type} {powerUps[1].Property[0]}% PowerUp_Property";
            powerOption2Button.sprite = powerUps[1].Image;
            power2Code.text = $"{powerUps[1].Code}";

            power3Text.text = $"{powerUps[2].Type} {powerUps[2].Property[0]}% PowerUp_Property";
            powerOption3Button.sprite = powerUps[2].Image;
            power3Code.text = $"{powerUps[2].Code}";
        }
    }
}
