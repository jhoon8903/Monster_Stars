using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.PowerUpScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Script
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
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private Common common;
        [SerializeField] private Exp exp;
        // [SerializeField] private StagePowerUp stagePowerUp;
        public readonly Queue<GameObject> PendingTreasure = new Queue<GameObject>();

        public static int IncreaseRange(IReadOnlyList<int> rangeValues)
        {
            var random = new System.Random();
            var randomIndex = random.Next(rangeValues.Count);
            return rangeValues[randomIndex];
        }
        private void TreasureCheck(GameObject matchedCharacters)
        {
            Time.timeScale = 0;
            var treasureChestName = matchedCharacters.GetComponent<CharacterBase>()._characterName;

            switch (treasureChestName)
            {
                case "Unit_Treasure00":
                    break;
                case "Unit_Treasure01":
                    Treasure1Reward();
                    break;
                case "Unit_Treasure02":
                    Treasure2Reward();
                    break;
                case "Unit_Treasure03":
                    Treasure3Reward();
                    break;
            }
        }
        private void Treasure1Reward()
        {
            var powerUps = CommonSelectPower(70, 25, 5);
            CommonDisplayPowerUps(powerUps);
        }
        private void Treasure2Reward()
        {
            var powerUps = CommonSelectPower(30, 55, 15);
            CommonDisplayPowerUps(powerUps);
        }
        private void Treasure3Reward()
        {
            var powerUps = CommonSelectPower(0, 50, 50);
            CommonDisplayPowerUps(powerUps);
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
        public void ProcessNextTreasure()
        {
            if (PendingTreasure.Count > 0)
            {
                TreasureCheck(PendingTreasure.Dequeue());
            }
        }
        public void PowerUpSelected()
        { 
            treasurePanel.SetActive(false); 
            Time.timeScale = 1;
            StartCoroutine(spawnManager.PositionUpCharacterObject());
        }


        // From here onwards, the Exp PowerUp section //




    }
}
