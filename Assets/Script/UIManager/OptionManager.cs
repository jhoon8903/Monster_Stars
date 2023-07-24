using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button PrivacyPolicyBtn;
    [SerializeField] private UnityEngine.UI.Button TermsOfServiceBtn;
    [SerializeField] private string PrivacyPolicy;
    [SerializeField] private string TermsOfService;

    [SerializeField] private UnityEngine.UI.Image BGMBG;
    [SerializeField] private UnityEngine.UI.Image SoundEffectBG;

    [SerializeField] private GameObject KORBtn;
    [SerializeField] private GameObject ENGBtn;
    [SerializeField] private GameObject KorCheck;
    [SerializeField] private GameObject EngCheck;

    [SerializeField] private SpriteRenderer BGMRenderer;
    [SerializeField] private SpriteRenderer BGMToggleRenderer;

    [SerializeField] private SpriteRenderer SoundEffectRenderer;
    [SerializeField] private SpriteRenderer SoundEffectToggleRenderer;

    private bool BGM = true;
    private bool Sound = true;

    private const string BGMKey = "BGMState";
    private const string SoundKey = "SoundState";

    // Start is called before the first frame update
    void Start()
    {
        PrivacyPolicyBtn.onClick.AddListener(PrivacyPolicyURL);
        TermsOfServiceBtn.onClick.AddListener(TermsOfServiceURL);

        UnityEngine.UI.Button BGMBtn = BGMBG.GetComponent<UnityEngine.UI.Button>();
        BGMBtn.onClick.AddListener(BgmONOff);
        UnityEngine.UI.Button SoundEffectBtn = SoundEffectBG.GetComponent<UnityEngine.UI.Button>();
        SoundEffectBtn.onClick.AddListener(SoundOnOff);

        KORBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ChangeLanguage);
        ENGBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ChangeLanguage);

        // 저장된 상태 확인
        if (PlayerPrefs.HasKey(BGMKey))
        {
            BGM = PlayerPrefs.GetInt(BGMKey) == 1;
            SetBGMButtonState(BGM);
        }

        if (PlayerPrefs.HasKey(SoundKey))
        {
            Sound = PlayerPrefs.GetInt(SoundKey) == 1;
            SetSoundButtonState(Sound);
        }

        if (PlayerPrefs.HasKey("Language"))
        {
            string language = PlayerPrefs.GetString("Language");
            if (language == "KOR")
            {
                // 한국어로 설정된 경우
                // 한국어 체크 이미지 활성화, 영어 체크 이미지 비활성화 등의 처리
                KorCheck.SetActive(true);
                EngCheck.SetActive(false);
            }
            else if (language == "ENG")
            {
                // 영어로 설정된 경우
                // 영어 체크 이미지 활성화, 한국어 체크 이미지 비활성화 등의 처리
                KorCheck.SetActive(false);
                EngCheck.SetActive(true);
            }
        }
        else
        {
            // 저장된 언어 정보가 없는 경우 기본값으로 한국어 설정
            // 한국어 체크 이미지 활성화, 영어 체크 이미지 비활성화 등의 처리
            KorCheck.SetActive(true);
            EngCheck.SetActive(false);

            // 한국어로 언어 정보 저장 (기본값 설정)
            PlayerPrefs.SetString("Language", "KOR");
        }
    }

    public void PrivacyPolicyURL()
    {
        Application.OpenURL(PrivacyPolicy);
    }

    public void TermsOfServiceURL()
    {
        Application.OpenURL(TermsOfService);
    }

    private void BgmONOff()
    {
        BGM = !BGM;
        SetBGMButtonState(BGM);
        PlayerPrefs.SetInt(BGMKey, BGM ? 1 : 0);
    }

    private void SoundOnOff()
    {
        Sound = !Sound;
        SetSoundButtonState(Sound);
        PlayerPrefs.SetInt(SoundKey, Sound ? 1 : 0);
    }

    private void SetBGMButtonState(bool state)
    {
        BGMRenderer.color = state ? Color.yellow : Color.gray;
        BGMToggleRenderer.color = state ? Color.white : Color.black;
        BGMToggleRenderer.transform.position += new Vector3(state ? 0.3f : -0.3f, 0f, 0f);
    }

    private void SetSoundButtonState(bool state)
    {
        SoundEffectRenderer.color = state ? Color.yellow : Color.gray;
        SoundEffectToggleRenderer.color = state ? Color.white : Color.black;
        SoundEffectToggleRenderer.transform.position += new Vector3(state ? 0.3f : -0.3f, 0f, 0f);
    }

    public void ChangeLanguage()
    {
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        if (clickedButton == KORBtn.gameObject)
        {
            // 한국어 버튼이 눌렸을 때 실행할 동작
            Debug.Log("한국어 버튼이 눌렸습니다.");
            KorCheck.SetActive(true);
            EngCheck.SetActive(false);
            // 한국어로 언어 정보 저장
            PlayerPrefs.SetString("Language", "KOR");
        }
        else if (clickedButton == ENGBtn.gameObject)
        {
            // 영어 버튼이 눌렸을 때 실행할 동작
            Debug.Log("영어 버튼이 눌렸습니다.");
            KorCheck.SetActive(false);
            EngCheck.SetActive(true);
            // 영어로 언어 정보 저장
            PlayerPrefs.SetString("Language", "ENG");
        }
    }

}
