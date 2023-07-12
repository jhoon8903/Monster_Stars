using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RedirectManager : MonoBehaviour
{

    [SerializeField] private UnityEngine.UI.Button PrivacyPolicyBtn;
    [SerializeField] private UnityEngine.UI.Button TermsOfServiceBtn;
    [SerializeField] private string PrivacyPolicy;
    [SerializeField] private string TermsOfService;

    // Start is called before the first frame update
    void Start()
    {
        PrivacyPolicyBtn.onClick.AddListener(PrivacyPolicyURL);
        TermsOfServiceBtn.onClick.AddListener(TermsOfServiceURL);
    }

    public void PrivacyPolicyURL()
    {
        Application.OpenURL(PrivacyPolicy);
    }

    public void TermsOfServiceURL()
    {
        Application.OpenURL(TermsOfService);
    }

}
