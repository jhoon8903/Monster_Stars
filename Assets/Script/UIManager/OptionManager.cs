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
}
