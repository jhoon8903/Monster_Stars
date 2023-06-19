using Script.EnemyManagerScript;
using UnityEngine;
using System.Collections;
using Script.CharacterManagerScript;

namespace Script.UIManager
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AtkManager atkManager;
        
        public int enemyTotalCount;
        public int set;
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
            CountSet(normalCount, slowCount, fastCount, sets);

            if (wave is 10 or 20)
            {
                enemySpawnManager.SpawnBoss(wave);
                enemyTotalCount = 1;
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

        private void CountSet(int n,int s,int f,int sets)
        {
            set = sets;
            enemyTotalCount = (n + s + f) * set;
        }

        public void EnemyDestroyEvent()
        {
            lock (EnemyLock)
            {
                Debug.Log($"Enemy destroyed, current total count: {enemyTotalCount}.");
                enemyTotalCount -= 1;
                Debug.Log($"After destroying an enemy, total count: {enemyTotalCount}.");
                if (atkManager.enemyList.Count != 0) return;
                StartCoroutine(gameManager.ContinueOrLose());
            }
        }


    }
}
