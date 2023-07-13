using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownBtn;
    [SerializeField] private GameObject adsCountinueBtn;

    private float totalTime = 10f; // 총 카운트다운 시간
    private float currentCountdown; // 현재 카운트다운 시간
    private bool retry;

    private const string RetryKey = "Retry";

    private void Start()
    {
        LoadRetryStatus(); // retry 상태 로드

        if (IsFirstLaunch()) // 최초 실행 여부 확인
        {
            retry = true;
            SaveRetryStatus(true); // 최초 실행 시 retry 상태를 true로 설정
        }
        adsCountinueBtn.GetComponent<Button>().onClick.AddListener(NoRetry);
        StartCountdown();
    }

    void Update()
    {
        if (retry)
        {
            if (currentCountdown > 0)
            {
                currentCountdown -= Time.unscaledDeltaTime;
                countdownText.text = Mathf.Ceil(currentCountdown).ToString(); // 올림으로 반올림
            }

            if (currentCountdown <= 0)
            {
                // 카운트다운 종료 처리
                countdownText.text = "0";
                CountdownOver();
            }
        }
        else
        {
            CountdownOver();
        }
    }

    void StartCountdown()
    {
        currentCountdown = totalTime;
    }

    private void CountdownOver()
    {
        countdownBtn.SetActive(false);
        adsCountinueBtn.SetActive(false);
        SaveRetryStatus(false); // retry 상태 저장
    }

    public void NoRetry()
    {
        retry = false;
        SaveRetryStatus(false); // retry 상태 저장
        AppLovinScript.Instance.ShowRewardedAd();
    }

    public void YesRetry()
    {
        retry = true;
        SaveRetryStatus(true); // retry 상태 저장
    }

    private void SaveRetryStatus(bool value)
    {
        PlayerPrefs.SetInt(RetryKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadRetryStatus()
    {
        retry = PlayerPrefs.GetInt(RetryKey, 0) == 1;
    }

    private bool IsFirstLaunch()
    {
        return !PlayerPrefs.HasKey(RetryKey);
    }
}
