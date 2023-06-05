using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        private List<GameObject> _pooledEnemy;
        

        public void Awake()
        {
            
            _pooledEnemy = new List<GameObject>();
            foreach (var enemySettings in enemyManager.enemyList)
            {
                for (var i = 0; i < enemySettings.poolSize; i++)
                {
                    var obj = Instantiate(enemySettings.enemyPrefab, transform);
                    obj.GetComponent<EnemyBase>().number = i + 1;
                    obj.GetComponent<EnemyBase>().EnemyProperty();
                    obj.SetActive(false);
                    _pooledEnemy.Add(obj);
                }
            }
        }

        public GameObject GetPooledEnemy(EnemyBase.EnemyTypes enemyType)
        {
            return _pooledEnemy
                .FirstOrDefault(t => 
                    !t.activeInHierarchy && 
                    t.GetComponent<EnemyBase>().EnemyType == enemyType);
        }

        public void ReturnToPool(GameObject obj)
        { 
            obj.transform.localPosition = new Vector3(0, 20, 0);
            Debug.Log($"obj Return Position: {obj.transform.position} {obj.GetComponent<EnemyBase>().number}");
            obj.SetActive(false);
        }
    }
}