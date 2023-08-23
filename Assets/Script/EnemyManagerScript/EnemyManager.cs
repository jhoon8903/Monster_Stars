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
        [SerializeField] public List<EnemyBase> stageBoss;
    }
}