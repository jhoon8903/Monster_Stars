using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.TopMenuGroup
{
    public class StaminaScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI staminaText;
        [SerializeField] private TextMeshProUGUI staminaRecoveryTime;
        // MarketingVersion
        private const int MaxStamina = 30;
        // private const int MaxStamina = 1000;
        private const float RecoveryCooldown = 1200.0f;
        private float _currentCooldown;
        private const string LastTimeKey = "LastTimeKey";
        private const string StaminaKey = "Stamina";

        public int CurrentStamina
        {
            get => PlayerPrefs.GetInt(StaminaKey, MaxStamina);
            set
            {
                PlayerPrefs.SetInt(StaminaKey, value);
                StaminaUpdate();
            }
        }

        public static StaminaScript Instance;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            LoadStaminaState();
            StaminaUpdate();
            StartCoroutine(RecoveryStamina());
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.S)) return;
            // MarketingVersion
            CurrentStamina = 30;
            // CurrentStamina = 1000;
            StaminaUpdate();
        }
        private void OnApplicationQuit()
        {
            SaveStaminaState();
        }
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveStaminaState();
            }
            else
            {
                LoadStaminaState();
            }
        }
        public void SaveStaminaState()
        {
            PlayerPrefs.SetString(LastTimeKey, DateTime.UtcNow.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetInt("Stamina", CurrentStamina);
            PlayerPrefs.SetFloat("Cooldown", _currentCooldown);
        }

        private void LoadStaminaState()
        {
            var lastTime = PlayerPrefs.GetString(LastTimeKey, DateTime.UtcNow.ToString(CultureInfo.CurrentCulture));
            var lastDateTime = DateTime.Parse(lastTime);
            var elapsed = DateTime.UtcNow - lastDateTime;
            var recoveryAmount = Mathf.FloorToInt((float)elapsed.TotalSeconds / RecoveryCooldown); // 시작 스테미나 설정 기본 MaxStamina
            CurrentStamina = Mathf.Min(MaxStamina, CurrentStamina + recoveryAmount);
            _currentCooldown = PlayerPrefs.GetFloat("Cooldown", RecoveryCooldown);
            _currentCooldown -= (float)(elapsed.TotalSeconds % RecoveryCooldown);
            if (!(_currentCooldown < 0)) return;
            _currentCooldown += RecoveryCooldown;
            CurrentStamina = Mathf.Min(MaxStamina, CurrentStamina + 1);
        }
        public void StaminaUpdate()
        {
            staminaText.text = $"{CurrentStamina}/{MaxStamina}";
        }


        public void StaminaReward(int stamina)
        {
            CurrentStamina += stamina;
        }
        private void StaminaTimeUpdate()
        {
            var minutes = Mathf.FloorToInt(_currentCooldown / 60);
            var seconds = Mathf.FloorToInt(_currentCooldown % 60);
            staminaRecoveryTime.text = $"{minutes:00}:{seconds:00}";
        }
        private IEnumerator RecoveryStamina()
        {
            while (true)
            {
                if (CurrentStamina < MaxStamina)
                {
                    staminaRecoveryTime.gameObject.SetActive(true);
                    _currentCooldown -= Time.deltaTime;
                    if (_currentCooldown <= 0)
                    {
                        CurrentStamina++;
                        StaminaUpdate();
                        _currentCooldown = RecoveryCooldown;
                    }
                }
                else
                {
                    staminaRecoveryTime.gameObject.SetActive(false);
                }
                StaminaTimeUpdate();
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}