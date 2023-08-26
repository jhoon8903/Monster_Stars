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
        private void Awake()
        {
           openBtn.GetComponent<Button>().onClick.AddListener(() =>
           {
               SoundManager.Instance.PlaySound(SoundManager.Instance.popupOpen);
               settingPanel.SetActive(true);
           }); 
           closeBtn.onClick.AddListener(() =>
           {
               SoundManager.Instance.PlaySound(SoundManager.Instance.popupClose);
               settingPanel.SetActive(false);
           });
           musicOn.GetComponent<Button>().onClick.AddListener(MusicController);
           musicOff.GetComponent<Button>().onClick.AddListener(MusicController);
           soundOn.GetComponent<Button>().onClick.AddListener(SoundController);
           soundOff.GetComponent<Button>().onClick.AddListener(SoundController);
           SoundManager.Instance.music = PlayerPrefs.GetInt(SoundManager.MusicKey) == 1;
           SoundManager.Instance.sound = PlayerPrefs.GetInt(SoundManager.SoundKey) == 1;
           UpdateMusicState();
           UpdateSoundState();
           gameObject.SetActive(false);
        }

        private void Start()
        {
            privacyPolicyBtn.onClick.AddListener(PrivacyPolicyURL);
            termsOfServiceBtn.onClick.AddListener(TermsOfServiceURL);
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
            SoundManager.Instance.music = !SoundManager.Instance.music;
            PlayerPrefs.SetInt(SoundManager.MusicKey, SoundManager.Instance.music ? 1 : 0);
            PlayerPrefs.Save();
            UpdateMusicState();
        }

        private void UpdateMusicState()
        {
            SoundManager.Instance.IsMusicEnabled = SoundManager.Instance.music;
            musicOn.SetActive(SoundManager.Instance.music);
            musicOff.SetActive(!SoundManager.Instance.music);
            SoundManager.Instance.BGM(SoundManager.Instance.bgmClip);
        }

        private void SoundController()
        {
            SoundManager.Instance.sound = !SoundManager.Instance.sound;
            PlayerPrefs.SetInt(SoundManager.SoundKey, SoundManager.Instance.sound ? 1 : 0);
            PlayerPrefs.Save();
            UpdateSoundState();
        }

        private void UpdateSoundState()
        {
            SoundManager.Instance.IsSoundEnabled = SoundManager.Instance.sound;
            soundOn.SetActive(SoundManager.Instance.sound);
            soundOff.SetActive(!SoundManager.Instance.sound);
        }
    }
}
