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

        private void Start()
        {
            pauseBtn.GetComponent<Button>().onClick.AddListener(Pause);
            robbyBtn.GetComponent<Button>().onClick.AddListener(ReturnRobby);
            resumeBtn.GetComponent<Button>().onClick.AddListener(Resume);
        }

        private void Pause()
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }

        public void Resume()
        {
            pausePanel.SetActive(false);
            GameManager.Instance.GameSpeed(); 
        }

        public static void ReturnRobby()
        {
            StageManager.Instance.isStageClear = false;
            SceneManager.LoadScene("SelectScene");
        }
    }
}
