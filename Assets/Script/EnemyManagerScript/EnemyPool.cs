using System.Collections.Generic;
using UnityEngine;

namespace Script.EnemyManagerScript
{
      public class EnemyPool : MonoBehaviour
      {
            [SerializeField] private EnemyManager enemyManager;
            internal List<GameObject> PooledEnemy;

            public void Awake()
            {
                  PooledEnemy = new List<GameObject>();
                  foreach (var enemySettings in enemyManager.enemyList)
                  {
                        for (var i = 0; i < enemySettings.poolSize; i++)
                        {
                              var obj = Instantiate(enemySettings.enemyPrefab, transform, true);
                              obj.GetComponent<EnemyBase>().EnemyProperty();
                              obj.SetActive(false);
                              PooledEnemy.Add(obj);
                        }
                  }
            }
      }
}

