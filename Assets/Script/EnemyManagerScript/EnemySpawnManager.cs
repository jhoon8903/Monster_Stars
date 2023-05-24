using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private GameObject enemySpawnZoneA;
        [SerializeField] private GameObject enemySpawnZoneB;
        [SerializeField] private GameObject enemySpawnZoneC;
        [SerializeField] private GameObject enemySpawnZoneD;
        [SerializeField] private GameObject enemySpawnZoneE;
        [SerializeField] private float spawnDelay = 2f;
        public List<GameObject> fieldList = new List<GameObject>();

        private Dictionary<EnemyBase.EnemyZone, Queue<GameObject>> _zoneDictionary;
        private Dictionary<EnemyBase.EnemyZone, GameObject> _spawnZoneDictionary; 

        public void Awake()
        {
            _zoneDictionary = new Dictionary<EnemyBase.EnemyZone, Queue<GameObject>>
            {
                { EnemyBase.EnemyZone.A, new Queue<GameObject>() },
                { EnemyBase.EnemyZone.B, new Queue<GameObject>() },
                { EnemyBase.EnemyZone.C, new Queue<GameObject>() },
                { EnemyBase.EnemyZone.D, new Queue<GameObject>() },
                { EnemyBase.EnemyZone.E, new Queue<GameObject>() },
            };

            _spawnZoneDictionary = new Dictionary<EnemyBase.EnemyZone, GameObject> 
            {
                { EnemyBase.EnemyZone.A, enemySpawnZoneA },
                { EnemyBase.EnemyZone.B, enemySpawnZoneB },
                { EnemyBase.EnemyZone.C, enemySpawnZoneC },
                { EnemyBase.EnemyZone.D, enemySpawnZoneD },
                { EnemyBase.EnemyZone.E, enemySpawnZoneE },
            };

            foreach (var enemyObject in enemyPool.PooledEnemy)
            {
                var spawnZone = enemyObject.GetComponent<EnemyBase>().SpawnZone;
                if (_zoneDictionary.TryGetValue(spawnZone, out var queue))
                {
                    queue.Enqueue(enemyObject);
                }
            }
        }

        public IEnumerator SpawnEnemies()
        {
            var spawnCounts = new Dictionary<EnemyBase.EnemyZone, int>
            {
                { EnemyBase.EnemyZone.A, 0 },
                { EnemyBase.EnemyZone.B, 0 },
                { EnemyBase.EnemyZone.C, 0 },
                { EnemyBase.EnemyZone.D, 0 },
                { EnemyBase.EnemyZone.E, 0 }
            };

            var spawnInProgress = true;

            while (spawnInProgress)
            {
                spawnInProgress = false; // reset each loop iteration

                foreach (var zone in _zoneDictionary
                             .Where(zone => zone.Value.Count > 0 && spawnCounts[zone.Key] < 10))
                {
                    spawnInProgress = true; // set flag to continue looping
                    spawnCounts[zone.Key]++; // increment spawn count for this zone

                    var enemyObject = zone.Value.Dequeue();
                    enemyObject.SetActive(true);
                    fieldList.Add(enemyObject);
                    var spawnPosition = zone.Key switch
                    {
                        EnemyBase.EnemyZone.A => new Vector3(Random.Range(0f, 5f), enemySpawnZoneA.transform.position.y, 0),
                        EnemyBase.EnemyZone.B => new Vector3(enemySpawnZoneB.transform.position.x, Random.Range(8f, 9f), 0),
                        EnemyBase.EnemyZone.C => new Vector3(enemySpawnZoneC.transform.position.x, Random.Range(8f, 9f), 0),
                        EnemyBase.EnemyZone.D => new Vector3(enemySpawnZoneD.transform.position.x, Random.Range(-0.5f, 0.5f), 0),
                        EnemyBase.EnemyZone.E => new Vector3(enemySpawnZoneE.transform.position.x, Random.Range(-0.5f, 0.5f), 0),
                        _ => new Vector3()
                    };
                    enemyObject.transform.position = spawnPosition;
                    enemyObject.transform.SetParent(_spawnZoneDictionary[zone.Key].transform);
                }

                if (spawnInProgress)
                    yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
