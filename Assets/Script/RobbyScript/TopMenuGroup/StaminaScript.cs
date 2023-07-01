using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Script.RobbyScript.TopMenuGroup
{
    public class StaminaScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI staminaText;
        [SerializeField] private TextMeshProUGUI staminaRecoveryTime;
        public int currentStamina;
        private const int MaxStamina = 30;
        private const float RecoveryCooldown = 180.0f;
        private float _currentCooldown;
        private const string LastTimeKey = "LastTimeKey";

        private void Start()
        {
            LoadStaminaState();
            StaminaUpdate();
            StartCoroutine(RecoveryStamina());
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
            PlayerPrefs.SetString(LastTimeKey, DateTime.UtcNow.ToString());
            PlayerPrefs.SetInt("Stamina", currentStamina);
            PlayerPrefs.SetFloat("Cooldown", _currentCooldown);
        }
        private void LoadStaminaState()
        {
            var lastTime = PlayerPrefs.GetString(LastTimeKey, DateTime.UtcNow.ToString());
            var lastDateTime = DateTime.Parse(lastTime);
            var elapsed = DateTime.UtcNow - lastDateTime;
            var recoveryAmount = Mathf.FloorToInt((float)elapsed.TotalSeconds / RecoveryCooldown);
            currentStamina = PlayerPrefs.GetInt("Stamina", MaxStamina); // 시작 스테미나 설정 기본 MaxStamina
            currentStamina = Mathf.Min(MaxStamina, currentStamina + recoveryAmount);
            _currentCooldown = PlayerPrefs.GetFloat("Cooldown", RecoveryCooldown);
            _currentCooldown -= (float)(elapsed.TotalSeconds % RecoveryCooldown);
            if (!(_currentCooldown < 0)) return;
            _currentCooldown += RecoveryCooldown;
            currentStamina = Mathf.Min(MaxStamina, currentStamina + 1);
        }
        public void StaminaUpdate()
        {
            staminaText.text = $"{currentStamina}/{MaxStamina}";
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
                if (currentStamina < MaxStamina)
                {
                    staminaRecoveryTime.gameObject.SetActive(true);
                    _currentCooldown -= Time.deltaTime;
                    if (_currentCooldown <= 0)
                    {
                        currentStamina++;
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