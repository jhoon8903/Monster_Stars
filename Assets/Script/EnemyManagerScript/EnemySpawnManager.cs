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
        [SerializeField] private ExpManager expManager;
        [SerializeField] private GridManager gridManager;

        private Dictionary<EnemyBase.SpawnZones, Transform> _spawnZones;
        public List<GameObject> fieldList = new List<GameObject>();

        private void Start()
        {
            expManager = GetComponent<ExpManager>();
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
                SpawnEnemy(enemyType);
            }
            yield return null;
        }
        public void SpawnBoss(int wave)
        {
            var bossObject = Instantiate(wave == 10 ? enemyManager.stage10BossPrefab : enemyManager.stage20BossPrefab, transform);
            bossObject.transform.position = gridManager.bossSpawnArea;
            bossObject.SetActive(true);
            fieldList.Add(bossObject);
        }

        private void SpawnEnemy(EnemyBase.EnemyTypes enemyType)
        {
            var enemyToSpawn = enemyPool.GetPooledEnemy(enemyType);
            if (enemyToSpawn == null)
            {
                return;
            }
            var enemyZone = enemyToSpawn.GetComponent<EnemyBase>().SpawnZone;
            var spawnPos = GetRandomPointInBounds(enemyZone);
            enemyToSpawn.transform.position = spawnPos;
            enemyToSpawn.SetActive(true);
            fieldList.Add(enemyToSpawn);
            var enemyBase = enemyToSpawn.GetComponent<EnemyBase>();
            enemyBase.OnEnemyKilled += reason => { fieldList.Remove(enemyToSpawn); };
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
                        return new Vector3(xPosition, spawnPos.y + Random.Range(-0.5f, 0.5f), 0);
                    }
                }
                xPosition = Random.Range(0, 6);
                return new Vector3(xPosition, spawnPos.y + Random.Range(-0.5f, 0.5f), 0);
            }
            else
            {
                return spawnPos;
            }
        }
    }
}