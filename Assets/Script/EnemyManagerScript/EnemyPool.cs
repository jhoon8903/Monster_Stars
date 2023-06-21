using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private List<GameObject> pooledEnemy = new List<GameObject>();
        [SerializeField] private  List<GameObject> pooledDefaultEnemy = new List<GameObject>();

        public List<EnemyBase> enemyBases = new List<EnemyBase>();
        
        public void Awake()
        {
            foreach (var enemySettings in enemyManager.enemyList)
            {
                for (var i = 0; i < enemySettings.poolSize; i++)
                {
                    var obj = Instantiate(enemySettings.enemyPrefab, transform);
                    obj.GetComponent<EnemyBase>().number = i + 1;
                    obj.GetComponent<EnemyBase>().EnemyProperty();
                    obj.SetActive(false);
                    pooledDefaultEnemy.Add(obj);
                }
            }
            pooledEnemy = pooledDefaultEnemy.ToList();
        }

        public GameObject GetPooledEnemy(EnemyBase.EnemyTypes enemyType)
        {
            var spawnEnemy = pooledEnemy.FirstOrDefault(t => !t.activeInHierarchy && t.GetComponent<EnemyBase>().EnemyType == enemyType);
            pooledEnemy.Remove(spawnEnemy);
            if (spawnEnemy == null || pooledEnemy.Count(t => t.GetComponent<EnemyBase>().EnemyType == enemyType) < 1)
            {
                pooledEnemy.Clear();
                pooledEnemy = pooledDefaultEnemy.ToList();
                Debug.Log("Enemy List 초기화!");
            }
            if (spawnEnemy == null) return null;
            enemyBases.Add(spawnEnemy.GetComponent<EnemyBase>());
            return spawnEnemy;
        }


        public static void ReturnToPool(GameObject obj)
        {
            obj.GetComponent<EnemyBase>().isDead = false;
            obj.transform.localScale = Vector3.one;
            obj.SetActive(false);
        }
    }
}