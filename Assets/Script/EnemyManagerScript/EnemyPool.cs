using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] public List<GameObject> pooledEnemy = new List<GameObject>();
        [SerializeField] private  List<GameObject> pooledDefaultEnemy = new List<GameObject>();
        public List<EnemyBase> enemyBases = new List<EnemyBase>();
        public List<EnemyBase> bossList = new List<EnemyBase>();
        
        public void Start()
        {
            foreach (var enemySettings in enemyManager.enemyList)
            {
                for (var i = 0; i < enemySettings.poolSize; i++)
                {
                    var obj = Instantiate(enemySettings.enemyPrefab, transform);
                    obj.GetComponent<EnemyBase>().Initialize();
                    obj.SetActive(false);
                    pooledDefaultEnemy.Add(obj);
                }
            }
            pooledEnemy = pooledDefaultEnemy.ToList();

            foreach (var bossObj in enemyManager.stageBoss)
            {
                for (var i = 0; i < 1; i++)
                {
                    var obj = Instantiate(bossObj, transform);
                    obj.GetComponent<EnemyBase>().Initialize();
                    obj.gameObject.SetActive(false);
                    bossList.Add(obj);
                }
            }
        }

        public GameObject GetPooledEnemy(EnemyBase.EnemyClasses? enemyClass)
        {
            var spawnEnemy = pooledEnemy.FirstOrDefault(t => !t.activeInHierarchy && t.GetComponent<EnemyBase>().enemyClass == enemyClass);
            if (spawnEnemy == null)
            {
                var enemySettings = enemyManager.enemyList.FirstOrDefault(es => es.enemyPrefab.GetComponent<EnemyBase>().enemyClass == enemyClass);
                if (enemySettings != null)
                {
                    spawnEnemy = Instantiate(enemySettings.enemyPrefab, transform);
                    spawnEnemy.GetComponent<EnemyBase>().Initialize();
                    spawnEnemy.SetActive(false);
                    pooledDefaultEnemy.Add(spawnEnemy);
                    pooledEnemy.Add(spawnEnemy);
                }
            }
            if (spawnEnemy == null) return spawnEnemy;
            pooledEnemy.Remove(spawnEnemy);
            enemyBases.Add(spawnEnemy.GetComponent<EnemyBase>());
            return spawnEnemy;
        }

        public void ClearList()
        {
            pooledEnemy.Clear();
            pooledEnemy = pooledDefaultEnemy.ToList();
        }

        public void ReturnToPool(EnemyBase enemyBase)
        {
            var enemyBaseGameObject = enemyBase.gameObject;
            if (pooledEnemy.Contains(enemyBaseGameObject))
            {
                pooledEnemy.Remove(enemyBaseGameObject);
            }
            enemyBaseGameObject.SetActive(false);
        }
    }
}