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
        [SerializeField] public List<EnemySettings> enemyList;
        [Header("Boss Prefabs")]
        [SerializeField] public GameObject stage10BossPrefab;  // Stage 10 Boss Prefab
        [SerializeField] public GameObject stage20BossPrefab;  // Stage 20 Boss Prefab

        public void DecreaseEnemyMoveSpeed(int decreaseAmount)
        {
          
        }
    }
}