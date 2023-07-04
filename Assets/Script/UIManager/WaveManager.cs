using Script.EnemyManagerScript;
using UnityEngine;
using System.Collections;

namespace Script.UIManager
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private EnemyPool enemyPool;

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

            if (wave % 10 == 0)
            {
                yield return StartCoroutine(enemySpawnManager.SpawnBoss(wave));
            }
            else
            {
                for (var i = 0; i < sets; i++)
                {
                    var normalCountA = normalCount / 2;
                    var normalCountB = normalCount / 2;

                    if (normalCount % 2 == 1)
                    {
                        normalCountA += 1; 
                    }

                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.BasicA, normalCountA));
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.BasicD, normalCountB));
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Slow, slowCount));
                    yield return StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Fast, fastCount)); 
                    yield return new WaitForSeconds(3f);
                }
            }
        }

        public void EnemyDestroyEvent(EnemyBase enemyBase)
        {
            enemyPool.enemyBases.Remove(enemyBase);
            if (enemyPool.enemyBases.Count != 0) return;
            StartCoroutine(StageManager.Instance.WaveClear());
        }
    }
}
