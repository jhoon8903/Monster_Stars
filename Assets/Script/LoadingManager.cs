using System.Collections;
using Script.QuestGroup;
using Script.RobbyScript.MainMenuGroup;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class LoadingManager : MonoBehaviour
    {
        [SerializeField] private MainPanel mainPanel;
        [SerializeField] private Slider loadingSlider;
        public bool isFirstContact;
        public bool isLoading = true;

        public void Start()
        {
            isLoading = bool.Parse(PlayerPrefs.GetString("Loading", "true"));
            if (isLoading)
            {
                Time.timeScale = 1f;
                if (PlayerPrefs.HasKey("TutorialKey"))
                {
                    isFirstContact = bool.Parse(PlayerPrefs.GetString("TutorialKey", "true"));
                }
                else
                {
                    isFirstContact = true;
                    PlayerPrefs.SetString("TutorialKey", "true");
                }
                StartCoroutine(UpdateLoadingBar());
            }
            else
            {
                gameObject.SetActive(false);
            }
            QuestManager.Instance.SetQuest();
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
                mainPanel.StartGame();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
