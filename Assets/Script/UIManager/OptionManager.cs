using UnityEngine;

namespace Script.UIManager
{
    public class OptionManager : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button privacyPolicyBtn;
        [SerializeField] private UnityEngine.UI.Button termsOfServiceBtn;
        [SerializeField] private string privacyPolicy;
        [SerializeField] private string termsOfService;
        [SerializeField] private UnityEngine.UI.Image bgmBackground;
        [SerializeField] private UnityEngine.UI.Image soundEffectBg;
        [SerializeField] private GameObject korBtn;
        [SerializeField] private GameObject engBtn;
        [SerializeField] private GameObject korCheck;
        [SerializeField] private GameObject engCheck;
        [SerializeField] private SpriteRenderer bgmRenderer;
        [SerializeField] private SpriteRenderer bgmToggleRenderer;
        [SerializeField] private SpriteRenderer soundEffectRenderer;
        [SerializeField] private SpriteRenderer soundEffectToggleRenderer;

        private bool _bgm = true;
        private bool _sound = true;

        private const string BGMKey = "BGMState";
        private const string SoundKey = "SoundState";

        private void Start()
        {
            privacyPolicyBtn.onClick.AddListener(PrivacyPolicyURL);
            termsOfServiceBtn.onClick.AddListener(TermsOfServiceURL);
            var bgmBtn = bgmBackground.GetComponent<UnityEngine.UI.Button>();
            bgmBtn.onClick.AddListener(BgmOnOff);
            var soundEffectBtn = soundEffectBg.GetComponent<UnityEngine.UI.Button>();
            soundEffectBtn.onClick.AddListener(SoundOnOff);
            korBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ChangeLanguage);
            engBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ChangeLanguage);

            // 저장된 상태 확인
            if (PlayerPrefs.HasKey(BGMKey))
            {
                _bgm = PlayerPrefs.GetInt(BGMKey) == 1;
                SetBGMButtonState(_bgm);
            }

            if (PlayerPrefs.HasKey(SoundKey))
            {
                _sound = PlayerPrefs.GetInt(SoundKey) == 1;
                SetSoundButtonState(_sound);
            }

            if (PlayerPrefs.HasKey("Language"))
            {
                var language = PlayerPrefs.GetString("Language");
                switch (language)
                {
                    case "KOR":
                        // 한국어로 설정된 경우
                        // 한국어 체크 이미지 활성화, 영어 체크 이미지 비활성화 등의 처리
                        korCheck.SetActive(true);
                        engCheck.SetActive(false);
                        break;
                    case "ENG":
                        // 영어로 설정된 경우
                        // 영어 체크 이미지 활성화, 한국어 체크 이미지 비활성화 등의 처리
                        korCheck.SetActive(false);
                        engCheck.SetActive(true);
                        break;
                }
            }
            else
            {
                // 저장된 언어 정보가 없는 경우 기본값으로 한국어 설정
                // 한국어 체크 이미지 활성화, 영어 체크 이미지 비활성화 등의 처리
                korCheck.SetActive(true);
                engCheck.SetActive(false);

                // 한국어로 언어 정보 저장 (기본값 설정)
                PlayerPrefs.SetString("Language", "KOR");
            }
        }

        private void PrivacyPolicyURL()
        {
            Application.OpenURL(privacyPolicy);
        }

        private void TermsOfServiceURL()
        {
            Application.OpenURL(termsOfService);
        }

        private void BgmOnOff()
        {
            _bgm = !_bgm;
            SetBGMButtonState(_bgm);
            PlayerPrefs.SetInt(BGMKey, _bgm ? 1 : 0);
        }

        private void SoundOnOff()
        {
            _sound = !_sound;
            SetSoundButtonState(_sound);
            PlayerPrefs.SetInt(SoundKey, _sound ? 1 : 0);
        }

        private void SetBGMButtonState(bool state)
        {
            bgmRenderer.color = state ? Color.yellow : Color.gray;
            bgmToggleRenderer.color = state ? Color.white : Color.black;
            bgmToggleRenderer.transform.position += new Vector3(state ? 0.3f : -0.3f, 0f, 0f);
        }

        private void SetSoundButtonState(bool state)
        {
            soundEffectRenderer.color = state ? Color.yellow : Color.gray;
            soundEffectToggleRenderer.color = state ? Color.white : Color.black;
            soundEffectToggleRenderer.transform.position += new Vector3(state ? 0.3f : -0.3f, 0f, 0f);
        }

        private void ChangeLanguage()
        {
            var clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (clickedButton == korBtn.gameObject)
            {
                // 한국어 버튼이 눌렸을 때 실행할 동작
                Debug.Log("한국어 버튼이 눌렸습니다.");
                korCheck.SetActive(true);
                engCheck.SetActive(false);
                // 한국어로 언어 정보 저장
                PlayerPrefs.SetString("Language", "KOR");
            }
            else if (clickedButton == engBtn.gameObject)
            {
                // 영어 버튼이 눌렸을 때 실행할 동작
                Debug.Log("영어 버튼이 눌렸습니다.");
                korCheck.SetActive(false);
                engCheck.SetActive(true);
                // 영어로 언어 정보 저장
                PlayerPrefs.SetString("Language", "ENG");
            }
        }

    }
}
