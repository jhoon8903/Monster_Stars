using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.EnemyManagerScript
{
     [Serializable]
     public class EnemySettings
     {
          public GameObject enemyPrefab;
          public int poolSize;
     }
     public class EnemyManager : MonoBehaviour
     {
          public List<EnemySettings> enemyList;
     }
}

