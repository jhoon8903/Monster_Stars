using System.Collections;
using System.Collections.Generic;
using Script;
using Script.UIManager;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject pauseBtn;
    [SerializeField]
    private GameObject robbyBtn;
    [SerializeField]
    private GameObject resumeBtn;



    // Start is called before the first frame update
    void Start()
    {
        pauseBtn.GetComponent<Button>().onClick.AddListener(Pause);
        robbyBtn.GetComponent<Button>().onClick.AddListener(()=>StageManager.Instance.LoadRobby());
        resumeBtn.GetComponent<Button>().onClick.AddListener(Resume);
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }


    private void Resume()
    {
        pausePanel.SetActive(false);
        GameManager.Instance.GameSpeed(); 
    }

    private void Retry()
    {
       
    }

}
