using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        private List<GameObject> _pooledDefaultEnemy;
        private List<GameObject> _poolEnemy;



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
        }

        public GameObject GetPooledEnemy(EnemyBase.EnemyTypes enemyType)
        {
            var spawnEnemy = _poolEnemy
                .FirstOrDefault(t =>
                    !t.activeInHierarchy &&
                    t.GetComponent<EnemyBase>().EnemyType == enemyType);
            _poolEnemy.Remove(spawnEnemy);

            if (_poolEnemy.Count == 0) _poolEnemy = _pooledDefaultEnemy.ToList();
            return spawnEnemy;
        }

        public void ReturnToPool(GameObject obj)
        { 
            obj.transform.localPosition = new Vector3(0, 20, 0);
            obj.SetActive(false);
        }
    }
}