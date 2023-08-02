using UnityEngine;
using System;
using UnityEngine.UI;

public class Reward_Ctrl : MonoBehaviour
{
    public GameObject AdsPanel;
    public GameObject RewardPanel;
	public Text Rewardtext;

    public KeyTimer_Ctrl keyTimer_Ctrl;



    // 광고 애드몹버튼 누르기
    public void AdMobButton_Yes()
    {
        ShowRewardAd();
    }
    public void AdMobButton_No()
    {
        AdsPanel.SetActive(false);
    }

    // 보상형 광고 호출
    public void ShowRewardAd()
	{
        AdsPanel.SetActive(false);
        RewardPanel.SetActive(true);
        Rewardtext.text = keyTimer_Ctrl.Maxcount.ToString();
    }

    // 보상 확인
    public void AdMomRewardPanel_Close()
    {
        keyTimer_Ctrl.RewardVideo_Key();
        AdsPanel.SetActive(false);
        RewardPanel.SetActive(false);
    }
}
