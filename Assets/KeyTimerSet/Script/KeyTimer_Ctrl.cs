using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class KeyTimer_Ctrl : MonoBehaviour {

    public int Keycount; 
    public Text KeyText; 

    public Text TimerText;

    public int MaxTime; 
    public int Maxcount; 

    DateTime startTime;
    string startTimestr;

    public Reward_Ctrl reward_Ctrl;


    void Start()
    {
        if (PlayerPrefs.HasKey("KEY_COUNT") == false)
        {
            Keycount = Maxcount;
            PlayerPrefs.SetInt("KEY_COUNT", Keycount);
            KeyText.text = Keycount.ToString();
            DayCheck();
            TimerText.text = "Full";
        }
        else
        {
            Keycount = PlayerPrefs.GetInt("KEY_COUNT");
            if (Keycount >= Maxcount)
            {
                Keycount = Maxcount;
                TimerText.text = "Full";
            }
            PlayerPrefs.SetInt("KEY_COUNT",Keycount);
            KeyText.text = Keycount.ToString();

            RefilKey();
        }
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.R))
       {
           PlayerPrefs.DeleteAll();
           Debug.Log("ALL_Data_Reset");
       }
       // Game Exit
       if (Input.GetKeyDown(KeyCode.Escape))
       {
           Application.Quit();
       }
    }

    TimeSpan ts;
    public void RefilKey()
    {
        Keycount = PlayerPrefs.GetInt("KEY_COUNT");
        KeyText.text = Keycount.ToString();
        Time_Load();

        if (Keycount < Maxcount)
        {
            //Time check
            ts = DateTime.Now - startTime;
            // Maxtime check key plus
            if (ts.TotalSeconds >= MaxTime)   //10 Minutes 600  //30 Minutes 1800
            {
                Debug.Log("keyPlus");

                DayCheck(); // Time Reset

                Debug.Log(ts.TotalSeconds);

                OfflineKeyCount();
                if (Keycount >= Maxcount)
                {
                    Keycount = Maxcount;
                    TimerText.text = "Full";
                }
                PlayerPrefs.SetInt("KEY_COUNT", Keycount);
                KeyText.text = Keycount.ToString();

                RefilKey();
            }
            else
            {
                KeyTimer_Play = true;
                StartCoroutine(RoutineTimer()); // Timer Start
            }
        }
        else
        {
            TimerText.text = "Full";
            Debug.Log("Full");
        }
    }

    bool KeyTimer_Play;
    private IEnumerator RoutineTimer()
    {
        while (KeyTimer_Play == true)
        {
            ts = DateTime.Now - startTime;

            TimeSpan timercount = new TimeSpan(0, 0, MaxTime) - ts; // MaxTime - currentTime

            if (timercount.TotalSeconds <= 0)
            {
                Debug.Log("TimerStop");
                KeyTimer_Play = false;
                RefilKey();
                break;
            }
            else
            {
                TimerText.text = string.Format("{0:D2}:{1:D2}", timercount.Minutes, timercount.Seconds);

                yield return new WaitForSeconds(1.0f);
                if (Keycount >= Maxcount) break;
                // Time remaining
                TimerText.text = string.Format("{0:D2}:{1:D2}",timercount.Minutes, timercount.Seconds);
            }
        }
    }

    // Check Time Load
    void Time_Load()
    {
        startTimestr = PlayerPrefs.GetString("KEYTimer");
        startTime = DateTime.Parse(startTimestr); 
    }

    // Timer Reset
    public void DayCheck()
    {
        startTime = DateTime.Now;
        startTimestr = startTime.ToString(); 
        PlayerPrefs.SetString("KEYTimer", startTimestr);
        Debug.Log("TimerReset");
    }

    public void KeyUse()
    {
        if (Keycount > 0)
        {
            if (Keycount == Maxcount) DayCheck();
            Keycount--;
            PlayerPrefs.SetInt("KEY_COUNT", Keycount);
            KeyText.text = Keycount.ToString();
            RefilKey();
        }
        else
        {
            Debug.Log("AdMob Ads");
            reward_Ctrl.AdsPanel.SetActive(true);
        }
    }

    void OfflineKeyCount()
    {
        if (ts.TotalSeconds >= MaxTime) Keycount++;
        if (ts.TotalSeconds >= MaxTime * 2) Keycount++;
        if (ts.TotalSeconds >= MaxTime * 3) Keycount++;
        if (ts.TotalSeconds >= MaxTime * 4) Keycount++;
        if (ts.TotalSeconds >= MaxTime * 5) Keycount = Maxcount;
    }

    public void RewardVideo_Key()
    {
        Keycount = Maxcount;
        PlayerPrefs.SetInt("KEY_COUNT", Keycount);
        KeyText.text = Keycount.ToString();
        TimerText.text = "Full";
    }
}
