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
        
        public void Start()
        {
            foreach (var enemySettings in enemyManager.enemyList)
            {
                for (var i = 0; i < enemySettings.poolSize; i++)
                {
                    var obj = Instantiate(enemySettings.enemyPrefab, transform);
                    obj.GetComponent<EnemyBase>().number = i + 1;
                    obj.GetComponent<EnemyBase>().Initialize();
                    obj.SetActive(false);
                    pooledDefaultEnemy.Add(obj);
                }
            }
            pooledEnemy = pooledDefaultEnemy.ToList();
        }

        public GameObject GetPooledEnemy(EnemyBase.EnemyTypes enemyType, EnemyBase.SpawnZones spawnZones)
        {
            var spawnEnemy = pooledEnemy.FirstOrDefault(t => !t.activeInHierarchy && t.GetComponent<EnemyBase>().EnemyType == enemyType && t.GetComponent<EnemyBase>().SpawnZone == spawnZones);

            if (spawnEnemy == null)
            {
                var enemySettings = enemyManager.enemyList.FirstOrDefault(es => es.enemyPrefab.GetComponent<EnemyBase>().EnemyType == enemyType && es.enemyPrefab.GetComponent<EnemyBase>().SpawnZone == spawnZones);
                if (enemySettings != null)
                {
                    spawnEnemy = Instantiate(enemySettings.enemyPrefab, transform);
                    spawnEnemy.GetComponent<EnemyBase>().number = pooledDefaultEnemy.Count + 1;
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
            enemyBase.isPoison = false;
            enemyBase.isSlow = false;
            enemyBase.isBind = false;
            enemyBase.isBurn = false;
            enemyBase.isBleed = false;
            enemyBase.isDead = false;
            enemyBase.transform.localScale = Vector3.one;
            enemyBase.GetComponent<SpriteRenderer>().color = Color.white;
            enemyBaseGameObject.SetActive(false);
        }
    }
}