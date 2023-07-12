using System.Collections;
using System.Collections.Generic;
using Script;
using Script.RobbyScript.TopMenuGroup;
using Script.UIManager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour
{
 
    [SerializeField] private GameObject CoinBtn;
    [SerializeField] private GameObject StaminaBtn;
    [SerializeField] private GameObject GemBtn;


    private void Awake()
    {
        CoinBtn.GetComponent<Button>().onClick.AddListener(Coin);

        StaminaBtn.GetComponent<Button>().onClick.AddListener(Stamina);

        GemBtn.GetComponent<Button>().onClick.AddListener(Gem);
    }

    private void Coin()
    {
        AppLovinScript.Instance.ShowRewardedAd();
        RewardManager.Instance.RewardButtonClicked("Coin");
    }

    private void Stamina()
    {
        AppLovinScript.Instance.ShowRewardedAd();
        RewardManager.Instance.RewardButtonClicked("Stamina");
    }

    private void Gem()
    {
        AppLovinScript.Instance.ShowRewardedAd();
        RewardManager.Instance.RewardButtonClicked("Gem");
    }
}