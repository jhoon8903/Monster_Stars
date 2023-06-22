using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
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

        private Dictionary<EnemyBase.SpawnZones, Transform> _spawnZones;
        private readonly System.Random _sysRandom = new System.Random();
        public int randomX;

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
            Debug.Log(count);
            for (var i = 0; i < count; i++)
            {
                StartCoroutine(SpawnEnemy(enemyType));
                yield return new WaitForSecondsRealtime(0.2f);
            }
        }

        public void SpawnBoss(int wave)
        {
            var bossObject = Instantiate(wave == 10 ? enemyManager.stage10BossPrefab : enemyManager.stage20BossPrefab,
                transform);
            var enemyBase = bossObject.GetComponent<EnemyBase>();
            enemyPool.enemyBases.Clear();
            enemyPool.enemyBases.Add(enemyBase);
            enemyBase.transform.position = gridManager.bossSpawnArea;
            enemyBase.gameObject.SetActive(true);
            enemyBase.Initialize();
            StartCoroutine(enemyPatternManager.Boss_Move(enemyBase.gameObject));
        }

        private IEnumerator SpawnEnemy(EnemyBase.EnemyTypes enemyType)
        {
            var enemyToSpawn = enemyPool.GetPooledEnemy(enemyType);
            var enemyBase = enemyToSpawn.GetComponent<EnemyBase>();

            enemyBase.transform.localScale = enemyBase.EnemyType switch
            {
                EnemyBase.EnemyTypes.Fast => Vector3.one * 0.6f,
                EnemyBase.EnemyTypes.Slow => Vector3.one * 1f,
                _ => Vector3.one * 0.8f
            };
            var spawnPosition = Vector3.zero;
            yield return StartCoroutine(GetRandomPointInBounds(enemyBase.SpawnZone, pos => spawnPosition = pos));

            enemyBase.transform.position = spawnPosition;
            enemyBase.gameObject.SetActive(true);
            enemyBase.Initialize();
            yield return StartCoroutine(enemyPatternManager.Zone_Move(enemyBase));
        }

        private IEnumerator GetRandomPointInBounds(EnemyBase.SpawnZones zone, System.Action<Vector3> callback)
        {
            var spawnPosition = Vector3.zero;
            if (zone == EnemyBase.SpawnZones.A)
            {
                var spawnPosY = _spawnZones[zone].position.y;

                if (gameManager.wave is 1 or 2 or 3)
                {
                    var characters = characterPool.UsePoolCharacterList();
                    var xPositions = (from character in characters
                            where character
                                .GetComponent<CharacterBase>().Level >= 2
                            select character.transform.position.x)
                        .ToList();

                    if (xPositions.Count > 0)
                    {
                        var randomIndex = _sysRandom.Next(xPositions.Count);
                        spawnPosition = new Vector3(xPositions[randomIndex], spawnPosY + _sysRandom.Next(-1, 2), 0);
                    }
                }
                else
                {
                    randomX = _sysRandom.Next(0, 6);
                    spawnPosition = new Vector3(randomX, spawnPosY + _sysRandom.Next(-1, 2), 0);
                }
            }
            callback?.Invoke(spawnPosition);
            yield return null;
        }
    }
}
