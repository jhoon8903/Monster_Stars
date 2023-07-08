using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyScript;
using Script.PuzzleManagerGroup;
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

        private Dictionary<EnemyBase.SpawnZones, Transform> _spawnZones;
        public int randomX;

        private void Awake()
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
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                StartCoroutine(SpawnEnemy(enemyType));
                yield return new WaitForSeconds(0.2f);
            }
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
                        spawnPosition = new Vector3(xPositions[randomIndex], spawnPosY + Random.Range(-1, 2), 0);
                    }
                }
                else
                {
                    randomX = Random.Range(0, 6);
                    spawnPosition = new Vector3(randomX, spawnPosY + Random.Range(-1, 2), 0);
                }
            }
            callback?.Invoke(spawnPosition);
            yield return null;
        }
    }
}
