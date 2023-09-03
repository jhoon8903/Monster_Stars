using DG.Tweening;
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
                settingPanel.transform.localScale = Vector3.zero; 
                settingPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            }); 
            closeBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.popupClose);
                settingPanel.transform.DOScale(0.1f, 0.3f).SetEase(Ease.InBack).OnComplete(() => settingPanel.SetActive(false));
            });
           musicOn.GetComponent<Button>().onClick.AddListener(MusicController);
           musicOff.GetComponent<Button>().onClick.AddListener(MusicController);
           soundOn.GetComponent<Button>().onClick.AddListener(SoundController);
           soundOff.GetComponent<Button>().onClick.AddListener(SoundController);
           SoundManager.Instance.music = bool.Parse(PlayerPrefs.GetString(SoundManager.MusicKey, "true"));
           SoundManager.Instance.sound = bool.Parse(PlayerPrefs.GetString(SoundManager.SoundKey, "true"));
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
            PlayerPrefs.SetString(SoundManager.MusicKey, $"{SoundManager.Instance.music}");
            PlayerPrefs.Save();
            UpdateMusicState();
        }

        private void UpdateMusicState()
        {
            SoundManager.Instance.IsMusicEnabled = SoundManager.Instance.music;
            musicOn.SetActive(SoundManager.Instance.IsMusicEnabled);
            musicOff.SetActive(!SoundManager.Instance.IsMusicEnabled);
            SoundManager.Instance.BGM(SoundManager.Instance.bgmClip);
        }

        private void SoundController()
        {
            SoundManager.Instance.sound = !SoundManager.Instance.sound;
            PlayerPrefs.SetString(SoundManager.SoundKey, $"{SoundManager.Instance.sound}");
            PlayerPrefs.Save();
            UpdateSoundState();
        }

        private void UpdateSoundState()
        {
            SoundManager.Instance.IsSoundEnabled = SoundManager.Instance.sound;
            soundOn.SetActive(SoundManager.Instance.IsSoundEnabled);
            soundOff.SetActive(!SoundManager.Instance.IsSoundEnabled);
        }
    }
}
