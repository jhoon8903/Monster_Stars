using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.RewardScript;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private List<GameObject> pooledEnemy = new List<GameObject>();
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
            if (spawnEnemy == null) return null;
            enemyBases.Add(spawnEnemy.GetComponent<EnemyBase>());
            return spawnEnemy;
        }

        public void ClearList()
        {
            pooledEnemy.Clear();
            pooledEnemy = pooledDefaultEnemy.ToList();
            Debug.Log("Enemy List 초기화!");
        }

        public void ReturnToPool(EnemyBase enemyBase)
        {
            if (pooledEnemy.Contains(enemyBase.gameObject))
            {
                pooledEnemy.Remove(enemyBase.gameObject);
            }
            enemyBase.isDead = false;
            enemyBase.transform.localScale = Vector3.one;
            enemyBase.gameObject.SetActive(false);
            DOTween.Kill(enemyBase,true);
        }
    }
}