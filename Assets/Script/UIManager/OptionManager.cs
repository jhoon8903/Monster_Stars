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

    // Start is called before the first frame update
    void Start()
    {
        PrivacyPolicyBtn.onClick.AddListener(PrivacyPolicyURL);
        TermsOfServiceBtn.onClick.AddListener(TermsOfServiceURL);

        UnityEngine.UI.Button BGMBtn = BGMBG.GetComponent<UnityEngine.UI.Button>();
        BGMBtn.onClick.AddListener(BgmONOff);
        UnityEngine.UI.Button SoundEffectBtn = SoundEffectBG.GetComponent<UnityEngine.UI.Button>();
        SoundEffectBtn.onClick.AddListener(SoundOnOff);
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
        if (BGM)
        {
            BGMRenderer.color = Color.gray;
            BGMToggleRenderer.color = Color.black;
            BGMToggleRenderer.transform.position += new Vector3(-0.3f, 0f, 0f); // 현재 위치에서 x 좌표에 -10을 더하여 이동 
            BGM = false;
        }
        else
        {
            BGMRenderer.color = Color.yellow;
            BGMToggleRenderer.color = Color.white;
            BGMToggleRenderer.transform.position += new Vector3(0.3f, 0f, 0f); // 현재 위치에서 x 좌표에 -10을 더하여 이동 
            BGM = true;
        }
    }

    private void SoundOnOff()
    {
        if (Sound)
        {
            SoundEffectRenderer.color = Color.gray;
            SoundEffectToggleRenderer.color = Color.black;
            SoundEffectToggleRenderer.transform.position += new Vector3(-0.3f, 0f, 0f); // 현재 위치에서 x 좌표에 -10을 더하여 이동 
            Sound = false;
        }
        else
        {
            SoundEffectRenderer.color = Color.yellow;
            SoundEffectToggleRenderer.color = Color.white;
            SoundEffectToggleRenderer.transform.position += new Vector3(0.3f, 0f, 0f); // 현재 위치에서 x 좌표에 -10을 더하여 이동 
            Sound = true;
        }
    }
}
