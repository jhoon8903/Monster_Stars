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
           _music = PlayerPrefs.GetInt(MusicKey) == 1;
           _sound = PlayerPrefs.GetInt(SoundKey) == 1;
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
            _music = !_music;
            PlayerPrefs.SetInt(MusicKey, _music ? 1 : 0);
            PlayerPrefs.Save();
            UpdateMusicState();
        }

        private void UpdateMusicState()
        {
            SoundManager.Instance.IsMusicEnabled = _music;
            musicOn.SetActive(_music);
            musicOff.SetActive(!_music);
        }

        private void SoundController()
        {
            _sound = !_sound;
            PlayerPrefs.SetInt(SoundKey, _sound ? 1 : 0);
            PlayerPrefs.Save();
            UpdateSoundState();
        }

        private void UpdateSoundState()
        {
            SoundManager.Instance.IsSoundEnabled = _sound;
            soundOn.SetActive(_sound);
            soundOff.SetActive(!_sound);
        }
    }
}
