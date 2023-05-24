using System;
using System.Collections.Generic;
using UnityEngine;


namespace Script.EnemyManagerScript
{
      public class EnemyPool : MonoBehaviour
      {
            [SerializeField] private EnemyManager enemyManager;
            [SerializeField] private int enemyPool;
            private List<GameObject> _pooledEnemy;
            private const int bossPool = 1;

            public void Awake()
            {
                  
            }
      }
}

