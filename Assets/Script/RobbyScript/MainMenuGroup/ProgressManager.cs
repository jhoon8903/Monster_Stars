using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Script.RobbyScript.MainMenuGroup
{
    public class ProgressManager : MonoBehaviour
    {
        public static ProgressManager Instance;

        public int clearStage;
        public int currentStage;
        public int selectStage;
        
        public int clearWave;
        public int currentWave;
        public int initWave = 1;
        public int maxWave;


        public void Awake()
        {
            Instance = this;
        }

    }
}