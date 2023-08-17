using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class OptionManager : MonoBehaviour
    {
        [SerializeField] private GameObject settingPanel;
        [SerializeField] private Button openBtn;
        [SerializeField] private Button closeBtn;
        [SerializeField] private Button privacyPolicyBtn;
        [SerializeField] private Button termsOfServiceBtn;
        [SerializeField] private string privacyPolicy;
        [SerializeField] private string termsOfService;
        [SerializeField] private GameObject musicOn;
        [SerializeField] private GameObject musicOff;
        [SerializeField] private GameObject soundOn;
        [SerializeField] private GameObject soundOff;
        [SerializeField] private Button supportBtn;

        private bool _music;
        private bool _sound;

        private const string MusicKey = "Music";
        private const string SoundKey = "Sound";

        private void Awake()
        {
           openBtn.GetComponent<Button>().onClick.AddListener(() => settingPanel.SetActive(true)); 
           closeBtn.onClick.AddListener(() => settingPanel.SetActive(false));
        }

        private void Start()
        {
            privacyPolicyBtn.onClick.AddListener(PrivacyPolicyURL);
            termsOfServiceBtn.onClick.AddListener(TermsOfServiceURL);
            musicOn.GetComponent<Button>().onClick.AddListener(MusicController);
            musicOff.GetComponent<Button>().onClick.AddListener(MusicController);
            soundOn.GetComponent<Button>().onClick.AddListener(SoundController);
            soundOff.GetComponent<Button>().onClick.AddListener(SoundController);
            MusicController();
            SoundController();
        }

        private void PrivacyPolicyURL()
        {
            Application.OpenURL(privacyPolicy);
        }

        private void TermsOfServiceURL()
        {
            Application.OpenURL(termsOfService);
        }

        private void MusicController()
        {
            _music = PlayerPrefs.GetInt(MusicKey) == 1;
            if (_music)
            {
                musicOn.SetActive(false);
                musicOff.SetActive(true);
                PlayerPrefs.SetInt(MusicKey, 0);
               
            }
            else
            {
                musicOn.SetActive(true);
                musicOff.SetActive(false);
                PlayerPrefs.SetInt(MusicKey, 1);
            }
            PlayerPrefs.Save();
        }

        private void SoundController()
        {
            _sound = PlayerPrefs.GetInt(SoundKey) == 1;
            if (_sound)
            {
                soundOn.SetActive(false);
                soundOff.SetActive(true);
                PlayerPrefs.SetInt(SoundKey, 0);
            }
            else
            {
                soundOn.SetActive(true);
                soundOff.SetActive(false);
                PlayerPrefs.SetInt(SoundKey, 1);
            }
            PlayerPrefs.Save();
        }
    }
}
