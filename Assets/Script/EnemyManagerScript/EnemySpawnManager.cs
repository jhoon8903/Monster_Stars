using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.PuzzleManagerGroup;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [SerializeField] private Transform spawnZoneF;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private EnemyPatternManager enemyPatternManager;

        private Dictionary<EnemyBase.SpawnZones, Transform> _spawnZones;
        public int randomX;
        public int randomY;

        private void Awake()
        {
            _spawnZones = new Dictionary<EnemyBase.SpawnZones, Transform>()
            {
                { EnemyBase.SpawnZones.A, spawnZoneA },
                { EnemyBase.SpawnZones.B, spawnZoneB },
                { EnemyBase.SpawnZones.C, spawnZoneC },
                { EnemyBase.SpawnZones.D, spawnZoneD },
                { EnemyBase.SpawnZones.E, spawnZoneE },
                { EnemyBase.SpawnZones.F, spawnZoneF }
            };
        }

        public IEnumerator SpawnEnemies(EnemyBase.EnemyTypes enemyType, int count, List<EnemyBase.SpawnZones> groupZone)
        {
            for (var i = 0; i < count; i++)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var spawnZone = groupZone[i % groupZone.Count];
                StartCoroutine(SpawnEnemy(enemyType, spawnZone));
            }
        }

        private IEnumerator SpawnEnemy(EnemyBase.EnemyTypes enemyType, EnemyBase.SpawnZones spawnZone)
        {
            var enemyToSpawn = enemyPool.GetPooledEnemy(enemyType, spawnZone);
            if (enemyToSpawn == null) yield break;
            var enemyBase = enemyToSpawn.GetComponent<EnemyBase>();
            var spawnPosition = Vector3.zero;
            yield return StartCoroutine(GetRandomPointInBounds(enemyBase.SpawnZone, pos => spawnPosition = pos));
            enemyBase.transform.position = spawnPosition;
            enemyBase.gameObject.SetActive(true);
            enemyBase.Initialize();
            yield return StartCoroutine(enemyPatternManager.Zone_Move(enemyBase));
        }

        public IEnumerator SpawnBoss(int wave)
        {
            var bossObject = Instantiate(enemyManager.stageBoss, transform);
            var enemyBase = bossObject.GetComponent<EnemyBase>();
            enemyPool.enemyBases.Clear();
            enemyPool.enemyBases.Add(enemyBase);
            enemyBase.transform.position = gridManager.bossSpawnArea;
            enemyBase.gameObject.SetActive(true);
            enemyBase.Initialize();
            enemyBase.healthPoint *= 1f + wave * 0.2f;
            yield return StartCoroutine(enemyPatternManager.Zone_Move(enemyBase));
        }
        private IEnumerator GetRandomPointInBounds(EnemyBase.SpawnZones zone, Action<Vector3> callback)
        {
            var spawnPosition = Vector3.zero;
            switch (zone)
            {
                case EnemyBase.SpawnZones.A:
                {
                    var spawnPosY = _spawnZones[zone].position.y;
                    if (StageManager.Instance.currentWave is 1 or 2 or 3)
                    {
                        var characters = characterPool.UsePoolCharacterList();
                        var xPositions = (
                            from character in characters 
                            let baseComponent = character.GetComponent<CharacterBase>() 
                            where baseComponent && baseComponent.unitPuzzleLevel >= 2 
                            select character.transform.position.x).ToList();
                        if (xPositions.Count > 0)
                        {
                            var randomIndex = Random.Range(0, xPositions.Count);
                            spawnPosition = new Vector3(xPositions[randomIndex], spawnPosY, 0);
                        }
                    }
                    else
                    {
                        randomX = Random.Range(0, 6);
                        spawnPosition = new Vector3(randomX, spawnPosY, 0);
                    }
                    break;
                }
                case EnemyBase.SpawnZones.B:
                {
                    var spawnPosX = _spawnZones[zone].position.x;
                    randomY = Random.Range(EnforceManager.Instance.addRowCount == 0 ? 6 : 7, 9);
                    spawnPosition = new Vector3(spawnPosX, randomY + 0.5f,0);
                    break;
                }
                case EnemyBase.SpawnZones.C:
                {
                    var spawnPosX = _spawnZones[zone].position.x;
                    randomY = Random.Range(EnforceManager.Instance.addRowCount == 0 ? 6 : 7, 9);
                    spawnPosition = new Vector3(spawnPosX, randomY + 0.5f,0);
                    break;
                }
                case EnemyBase.SpawnZones.D:
                {
                    var spawnPosY = _spawnZones[zone].position.y;
                    randomX = Random.Range(1, 5);
                    spawnPosition = new Vector3(randomX, spawnPosY, 0);
                    break;
                }
                case EnemyBase.SpawnZones.E:
                {
                    var spawnPosY = _spawnZones[zone].position.y;
                    spawnPosY = EnforceManager.Instance.addRowCount == 0 ? spawnPosY : spawnPosY + 1; 
                    randomX = Random.Range(2, 4);
                    spawnPosition = new Vector3(randomX, spawnPosY, 0);
                    break;
                }
                case EnemyBase.SpawnZones.F:
                {
                    var spawnPosY = _spawnZones[zone].position.y;
                    spawnPosY = EnforceManager.Instance.addRowCount == 0 ? spawnPosY : spawnPosY + 1; 
                    randomX = (Random.value > 0.5f) ? -1 : 6;
                    spawnPosition = new Vector3(randomX, spawnPosY, 0);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
            callback?.Invoke(spawnPosition);
            yield return null;
        }
    }
}
