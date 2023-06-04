using Script.EnemyManagerScript;
using UnityEngine;
using System.Collections;

namespace Script.UIManager
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawnManager enemySpawnManager;

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

        public IEnumerator StartWave(int wave)
        {
            var (normalCount, slowCount, fastCount, sets) = GetSpawnCountForWave(wave);

            if (wave is 10 or 20)
            {
                enemySpawnManager.SpawnBoss(wave);
                yield break;
            }

            for (var i = 0; i < sets; i++)
            {
                StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Basic, normalCount));
                StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Slow, slowCount));
                StartCoroutine(enemySpawnManager.SpawnEnemies(EnemyBase.EnemyTypes.Fast, fastCount));
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
