using System.Collections;
using System.Globalization;
using Script.AdsScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class CountdownScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private GameObject countdown;
        [SerializeField] private GameObject adsContinueBtn;
        [SerializeField] private GameObject robbyBtn;

        private const float TotalTime = 10f; // 총 카운트다운 시간
        private float _currentCountdown; // 현재 카운트다운 시간
        public bool retry;
        private const string RetryKey = "Retry";
        public static bool ClickRetry;

        private void Start()
        {
            LoadRetryStatus(); // retry 상태 로드

            if (IsFirstLaunch()) // 최초 실행 여부 확인
            {
                retry = true;
                SaveRetryStatus(true); // 최초 실행 시 retry 상태를 true로 설정
            }
            adsContinueBtn.GetComponent<Button>().onClick.AddListener(YesRetry);
            robbyBtn.GetComponent<Button>().onClick.AddListener(NoRetry);
            StartCountdown();
        }

        private void Update()
        {
            if (retry)
            {
                if (_currentCountdown > 0)
                {
                    _currentCountdown -= Time.unscaledDeltaTime;
                    countdownText.text = Mathf.Ceil(_currentCountdown).ToString(CultureInfo.CurrentCulture); // 올림으로 반올림
                }

                if (!(_currentCountdown <= 0)) return;
                // 카운트다운 종료 처리
                countdownText.text = "0";
                CountdownOver();
            }
            else
            {
                CountdownOver();
            }
        }

        private void StartCountdown()
        {
            _currentCountdown = TotalTime;
        }

        private void CountdownOver()
        {
            countdown.SetActive(false);
            adsContinueBtn.SetActive(false); // retry 상태 저장
        }

        private static void YesRetry()
        {
            AdsManager.Instance.ShowRewardedAd();
            RewardManager.Instance.RewardButtonClicked("Retry");
            ClickRetry = true;
        }

        private void NoRetry()
        {
            retry = true;
            SaveRetryStatus(true); // retry 상태 저장
            GameManager.ReturnRobby();
        }

        private static void SaveRetryStatus(bool value)
        {
            PlayerPrefs.SetInt(RetryKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadRetryStatus()
        {
            retry = PlayerPrefs.GetInt(RetryKey, 1) == 1;
        }

        private static bool IsFirstLaunch()
        {
            return !PlayerPrefs.HasKey(RetryKey);
        }

        public static IEnumerator Retry()
        {
            // retry = false; 
            // SaveRetryStatus(retry); // retry 상태 저장
            // SceneManager.LoadScene("StageScene");
            yield return null;
        }
    }
}
