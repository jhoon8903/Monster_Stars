using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        private List<GameObject> _pooledEnemy = new List<GameObject>();
        private readonly List<GameObject> _pooledDefaultEnemy = new List<GameObject>();
       
        
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
                    _pooledDefaultEnemy.Add(obj);
                }
            }
            _pooledEnemy = _pooledDefaultEnemy.ToList();
        }

        public GameObject GetPooledEnemy(EnemyBase.EnemyTypes enemyType)
        {
            var spawnEnemy = _pooledEnemy
                .FirstOrDefault(t => !t.activeInHierarchy && t.GetComponent<EnemyBase>().EnemyType == enemyType);
            _pooledEnemy.Remove(spawnEnemy);
            if (_pooledEnemy.Count == 0) _pooledEnemy = _pooledDefaultEnemy.ToList();
            return spawnEnemy;
        }

        public IEnumerable<GameObject> SpawnEnemy()
        {
            return _pooledEnemy.Where(enemy => enemy.activeSelf).ToList();
        }

        public void ReturnToPool(GameObject obj)
        {
            obj.transform.position = new Vector3(-4,-4,0);
            obj.transform.localPosition = new Vector3(0, 20, 0);
            obj.SetActive(false);
        }
    }
}