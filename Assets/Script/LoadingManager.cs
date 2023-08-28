using System.Collections;
using Script.RobbyScript.MainMenuGroup;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class LoadingManager : MonoBehaviour 
    {
        [SerializeField] private Slider loadingSlider;
        public bool isFirstContact;
        public static LoadingManager Instance;

        public void Start()
        {
            Instance = this;
            if (!bool.Parse(PlayerPrefs.GetString(MainPanel.IsLoadingKey, "true")))
            {
                Instance.gameObject.SetActive(false);
            }
            else
            {
                Time.timeScale = 1f;
                if (PlayerPrefs.HasKey("TutorialKey"))
                {
                    isFirstContact = PlayerPrefs.GetInt("TutorialKey", 1) == 1;
                }
                else
                {
                    PlayerPrefs.SetInt("TutorialKey", 1);
                    isFirstContact = true;
                }
                StartCoroutine(UpdateLoadingBar());
            }
        }

        private IEnumerator UpdateLoadingBar()
        {
            const float duration = 2f;
            var progress = 0f;

            loadingSlider.value = 0f;

            while (progress < 1f)
            {
               
                progress += Time.deltaTime / duration;
                loadingSlider.value = Mathf.Clamp01(progress);
                yield return null;
            }
            loadingSlider.value = 1f;

            if (isFirstContact)
            {
                MainPanel.Instance.StartGame();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
