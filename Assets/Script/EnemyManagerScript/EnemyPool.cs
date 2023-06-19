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

        public List<EnemyBase> enemyBases = new List<EnemyBase>();
        public void Awake()
        {
            
            _pooledDefaultEnemy = new List<GameObject>();
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

            _poolEnemy = _pooledDefaultEnemy.ToList();

        public GameObject GetPooledEnemy(EnemyBase.EnemyTypes enemyType)
        {
            var spawnEnemy = _pooledEnemy.FirstOrDefault(t => !t.activeInHierarchy && t.GetComponent<EnemyBase>().EnemyType == enemyType);
            _pooledEnemy.Remove(spawnEnemy);
            if (_pooledEnemy.Count == 0) _pooledEnemy = _pooledDefaultEnemy;

            enemyBases.Add(spawnEnemy.GetComponent<EnemyBase>());
            return spawnEnemy;
        }


        public static void ReturnToPool(GameObject obj)
        {
            obj.transform.localScale = Vector3.one;
            obj.SetActive(false);
        }
    }
}
}