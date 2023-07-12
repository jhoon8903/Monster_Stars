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
            SceneManager.LoadScene("StageScene");
        }

        public void Resume()
        {
            pausePanel.SetActive(false);
            GameManager.Instance.GameSpeed(); 
        }

        public static void ReturnRobby()
        {
            StageManager.Instance.isStageClear = false;
            PlayerPrefs.SetInt("CurrentWave"+StageManager.Instance.currentStage,1);
            PlayerPrefs.DeleteKey("unitState");
            PlayerPrefs.SetInt("GridHeight", 6);
            SceneManager.LoadScene("SelectScene");
        }
    }
}
