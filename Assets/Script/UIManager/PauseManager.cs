using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject pauseBtn;
        [SerializeField] private GameObject robbyBtn;
        [SerializeField] private GameObject resumeBtn;
        [SerializeField] private GameObject retryBtn;
          
        private void Start()
        {
            pauseBtn.GetComponent<Button>().onClick.AddListener(Pause);
            robbyBtn.GetComponent<Button>().onClick.AddListener(ReturnRobby);
            resumeBtn.GetComponent<Button>().onClick.AddListener(Resume);
            retryBtn.GetComponent<Button>().onClick.AddListener(Retry);
        }

        private void Pause()
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }

        private void Retry()
        {
            pausePanel.SetActive(false);
        }

        public void Resume()
        {
            pausePanel.SetActive(false);
            GameManager.Instance.GameSpeed(); 
        }

        public static void ReturnRobby()
        {
            StageManager.Instance.isStageClear = false;
            PlayerPrefs.SetInt(StageManager.Instance.currentWaveKey,1);
            PlayerPrefs.DeleteKey("unitState");
            SceneManager.LoadScene("SelectScene");
        }
    }
}
