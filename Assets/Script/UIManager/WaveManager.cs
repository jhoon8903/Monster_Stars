using Script.EnemyManagerScript;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Script.UIManager
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EnemyPool enemyPool;
        public List<EnemyBase> enemies = new List<EnemyBase>();

        
        private static readonly object EnemyLock = new object();

        private static (int normal, int slow, int fast, int sets) GetSpawnCountForWave(int wave)
        {
            if (wave is 10 or 20) 
            {
                return (0, 0, 0, 0);
            }
    
            var baseCount = wave;
            if (wave > 10)
            {
                baseCount = wave - 10;
            }
    
            var normalCount = baseCount + 3;
            var slowCount = baseCount - 1;
            var fastCount = baseCount - 1;
            var sets = wave <= 10 ? 2 : 3;
            return (normalCount, slowCount, fastCount, sets);
        }

        public IEnumerator WaveController(int wave)
        {
            var (normalCount, slowCount, fastCount, sets) = GetSpawnCountForWave(wave);

            if (wave is 10 or 20)
            {
                enemySpawnManager.SpawnBoss(wave);
            }
            else
            {
                for (var i = 0; i < sets; i++)
                {
                    StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.BasicA, normalCount/2));
                    StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.BasicD, normalCount/2));
                    StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Slow, slowCount));
                    StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Fast, fastCount));
                    yield return new WaitForSeconds(3f);
                }
            }
        }

        public void EnemyDestroyEvent(EnemyBase enemyBase)
        {
      
            lock (EnemyLock)
            {
                enemies = enemyPool.enemyBases;
                enemies.Remove(enemyBase);
                if (enemies.Count != 0) return;
                StartCoroutine(gameManager.ContinueOrLose());
            }
        }
    }
}
