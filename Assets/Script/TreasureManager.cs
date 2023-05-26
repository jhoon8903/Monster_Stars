using System;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Script
{
    public class TreasureManager : MonoBehaviour
    {
        public class PowerUp
        {
            public enum PowerUpType
            { Damage, Speed, Slow, AtkRange, Freeze, MaxHp, Recovery, AddRow, MultiShot, LevelUp }
            // Damage Code = 1
            // Speed Code = 2
            // Slow Code = 3
            // AtkRange Code = 4
            // Freeze Code = 5
            // MaxHp Code = 6
            // Recovery Code = 7
            // AddRow Code = 8
            // MultiShot Code = 9
            // LevelUp Code = 10
            public PowerUpType PowerType { get; set; }
            public int Increase { get; set; }
            public int PowerCode { get; set; }
            public Sprite Image { get; set; }
        }
        [SerializeField] private GameObject treasurePanel;
        [SerializeField] private TextMeshProUGUI power1Text;
        [SerializeField] private TextMeshProUGUI power2Text;
        [SerializeField] private TextMeshProUGUI power3Text;
        [SerializeField] private Image powerOption1Button;
        [SerializeField] private Image powerOption2Button;
        [SerializeField] private Image powerOption3Button;
        [SerializeField] private TextMeshProUGUI power1Code;
        [SerializeField] private TextMeshProUGUI power2Code;
        [SerializeField] private TextMeshProUGUI power3Code;
        [SerializeField] private Sprite greenBtn;
        [SerializeField] private Sprite blueBtn;
        [SerializeField] private Sprite purpleBtn;

        private bool _selected = false;
        private List<PowerUp> _greenList;
        private List<PowerUp> _blueList;
        private List<PowerUp> _purpleList;

        private void Start()
        {
            var green = greenBtn;
            var blue = blueBtn;
            var purple = purpleBtn;
            _greenList = new List<PowerUp>
            {
                new PowerUp { PowerType = PowerUp.PowerUpType.Damage, Increase = Random.Range(1, 10), PowerCode = 1, Image = green },
                new PowerUp { PowerType = PowerUp.PowerUpType.Speed, Increase = Random.Range(1, 10), PowerCode = 2, Image = green },
                new PowerUp { PowerType = PowerUp.PowerUpType.Slow, Increase = Random.Range(5, 9), PowerCode = 3, Image = green },
                new PowerUp { PowerType = PowerUp.PowerUpType.AtkRange, Increase = Random.Range(5, 10), PowerCode = 4, Image = green },
                new PowerUp { PowerType = PowerUp.PowerUpType.Freeze, Increase = Random.Range(1, 5), PowerCode = 5, Image = green },
                new PowerUp { PowerType = PowerUp.PowerUpType.MaxHp, Increase = 100, PowerCode = 6, Image = green },
                new PowerUp { PowerType = PowerUp.PowerUpType.Recovery, Increase = 100, PowerCode = 7, Image = green},
                new PowerUp { PowerType = PowerUp.PowerUpType.LevelUp, Increase = 1, PowerCode = 10, Image = green }
            };

            _blueList = new List<PowerUp>
            {
                new PowerUp { PowerType = PowerUp.PowerUpType.Damage, Increase = Random.Range(11, 20), PowerCode = 1, Image = blue },
                new PowerUp { PowerType = PowerUp.PowerUpType.Speed, Increase = Random.Range(11, 20), PowerCode = 2, Image = blue },
                new PowerUp { PowerType = PowerUp.PowerUpType.Slow, Increase = Random.Range(10, 15), PowerCode = 3, Image = blue },
                new PowerUp { PowerType = PowerUp.PowerUpType.AtkRange, Increase = Random.Range(11, 20), PowerCode = 4, Image = blue },
                new PowerUp { PowerType = PowerUp.PowerUpType.Freeze, Increase = Random.Range(6, 10), PowerCode = 5, Image = blue },
                new PowerUp { PowerType = PowerUp.PowerUpType.MaxHp, Increase = 200, PowerCode = 6, Image = blue },
                new PowerUp { PowerType = PowerUp.PowerUpType.Recovery, Increase = 200, PowerCode = 7, Image = blue},
                new PowerUp { PowerType = PowerUp.PowerUpType.LevelUp, Increase = 2, PowerCode = 10, Image = blue },
                new PowerUp { PowerType = PowerUp.PowerUpType.MultiShot, Increase = 3, PowerCode = 9, Image = blue },
            };

            _purpleList = new List<PowerUp>
            {
                new PowerUp { PowerType = PowerUp.PowerUpType.Damage, Increase = Random.Range(21, 30), PowerCode = 1, Image = purple },
                new PowerUp { PowerType = PowerUp.PowerUpType.Speed, Increase = Random.Range(21, 30), PowerCode = 2, Image = purple },
                new PowerUp { PowerType = PowerUp.PowerUpType.Freeze, Increase = 20, PowerCode = 5, Image = purple },
                new PowerUp { PowerType = PowerUp.PowerUpType.MaxHp, Increase = 400, PowerCode = 6, Image = purple },
                new PowerUp { PowerType = PowerUp.PowerUpType.Recovery, Increase = 400, PowerCode = 7, Image = purple},
                new PowerUp { PowerType = PowerUp.PowerUpType.LevelUp, Increase = 3, PowerCode = 10, Image = purple },
                new PowerUp { PowerType = PowerUp.PowerUpType.AddRow, Increase = 1, PowerCode = 8, Image = purple },
            }; 
        }

        private List<PowerUp> SelectPower(int greenChance, int blueChance, int purpleChance)
        {
            var powerUps = new List<PowerUp>();
            for (var i = 0; i < 3; i++)
            {
                var total = greenChance + blueChance + purpleChance;
                var randomValue = Random.Range(0, total);
                var powerUp = new PowerUp();
                if (randomValue < greenChance)
                {
                    powerUp = _greenList[Random.Range(0, _greenList.Count)];
                }
                else if (randomValue < greenChance + blueChance)
                {
                    powerUp = _blueList[Random.Range(0, _blueList.Count)];
                }
                else
                {
                    powerUp = _purpleList[Random.Range(0, _purpleList.Count)];
                }
                powerUps.Add(powerUp);
            }
            return powerUps;
        }

        private void DisplayPowerUps(IReadOnlyList<PowerUp> powerUps)
        {
            treasurePanel.SetActive(true);

            power1Text.text = $"{powerUps[0].PowerType} {powerUps[0].Increase}% Increase";
            powerOption1Button.sprite = powerUps[0].Image;
            power1Code.text = $"{powerUps[0].PowerCode}";
            
            power2Text.text = $"{powerUps[1].PowerType} {powerUps[1].Increase}% Increase";
            powerOption2Button.sprite = powerUps[1].Image;
            power2Code.text = $"{powerUps[1].PowerCode}";
            
            power3Text.text = $"{powerUps[2].PowerType} {powerUps[2].Increase}% Increase";
            powerOption3Button.sprite = powerUps[2].Image;
            power3Code.text = $"{powerUps[2].PowerCode}";

        }

        public void TreasureCheck(GameObject matchedCharacters)
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
            var powerUps = SelectPower(70, 25, 5);
            DisplayPowerUps(powerUps);
        }

        private void Treasure2Reward()
        {
            var powerUps = SelectPower(40, 50, 10);
            DisplayPowerUps(powerUps);
        }

        private void Treasure3Reward()
        {
            var powerUps = SelectPower(30, 45, 25);
            DisplayPowerUps(powerUps);
        }

        public void PowerUpSelected()
        {
            treasurePanel.SetActive(false);
            Time.timeScale = 1;

        }
    }
}
