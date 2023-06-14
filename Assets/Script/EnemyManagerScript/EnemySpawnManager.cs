using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.UIManager;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private Transform spawnZoneA;
        [SerializeField] private Transform spawnZoneB;
        [SerializeField] private Transform spawnZoneC;
        [SerializeField] private Transform spawnZoneD;
        [SerializeField] private Transform spawnZoneE;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private EnemyPatternManager enemyPatternManager;
        [SerializeField] private WaveManager waveManager;

        private Dictionary<EnemyBase.SpawnZones, Transform> _spawnZones;

        private void Start()
        {
            _spawnZones = new Dictionary<EnemyBase.SpawnZones, Transform>()
            {
                { EnemyBase.SpawnZones.A, spawnZoneA },
                { EnemyBase.SpawnZones.B, spawnZoneB },
                { EnemyBase.SpawnZones.C, spawnZoneC },
                { EnemyBase.SpawnZones.D, spawnZoneD },
                { EnemyBase.SpawnZones.E, spawnZoneE },
            };
        }

        public IEnumerator SpawnEnemies(EnemyBase.EnemyTypes enemyType, int count)
        {
            for (var i = 0; i < count; i++)
            {
                StartCoroutine(SpawnEnemy(enemyType));
            }
            yield return null;
        }

        public void SpawnBoss(int wave)
        {
            var bossObject = Instantiate(wave == 10 ? enemyManager.stage10BossPrefab : enemyManager.stage20BossPrefab,
                transform);
            bossObject.transform.position = gridManager.bossSpawnArea;
            var enemyBase = bossObject.GetComponent<EnemyBase>();
            bossObject.SetActive(true);
            enemyBase.Initialize();
            waveManager.set = 1;
            StartCoroutine(enemyPatternManager.Boss_Move(bossObject));
        }

        private IEnumerator SpawnEnemy(EnemyBase.EnemyTypes enemyType)
        {
            var enemyToSpawn = enemyPool.GetPooledEnemy(enemyType);
            if (enemyToSpawn == null) yield return
                enemyToSpawn.transform.localScale = enemyToSpawn.GetComponent<EnemyBase>().EnemyType switch
                {
                    EnemyBase.EnemyTypes.Fast => Vector3.one * 0.6f,
                    EnemyBase.EnemyTypes.Slow => Vector3.one * 1f,
                    _ => Vector3.one * 0.8f
                };
            var enemyZone = enemyToSpawn.GetComponent<EnemyBase>().SpawnZone;
            var spawnPos = GetRandomPointInBounds(enemyZone);
            var enemyBase = enemyToSpawn.GetComponent<EnemyBase>();
            enemyToSpawn.transform.position = spawnPos;
            enemyToSpawn.SetActive(true);
            enemyBase.Initialize();
            yield return StartCoroutine(enemyPatternManager.Zone_Move(enemyToSpawn));
        }

        private Vector3 GetRandomPointInBounds(EnemyBase.SpawnZones zone)
        {
            var spawnPos = _spawnZones[zone].position;
            if (zone == EnemyBase.SpawnZones.A)
            {
                float xPosition;
                if (gameManager.wave is 1 or 2 or 3)
                {
                    var characters = characterPool.UsePoolCharacterList();
                    var xPositions = (from character in characters where character
                        .GetComponent<CharacterBase>().Level >= 2 select character.transform.position.x)
                        .ToList();

                    if (xPositions.Count > 0)
                    {
                        xPosition = xPositions[Random.Range(0, xPositions.Count)];
                        return new Vector3(xPosition, spawnPos.y + Random.Range(-1f, 1f), 0);
                    }
                }
                xPosition = Random.Range(0, 6);
                return new Vector3(xPosition, 9.5f + Random.Range(-1f, 1f) , 0);
            }
            else
            {
                return spawnPos;
            }
        }
    }
}