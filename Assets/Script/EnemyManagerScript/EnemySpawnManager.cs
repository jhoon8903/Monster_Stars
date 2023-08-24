using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyScript;
using Script.PuzzleManagerGroup;
using Script.RewardScript;
using Script.UIManager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;
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
        
        // Enemy Desc
        [SerializeField] private GameObject enemyDescPanel;
        [SerializeField] private Image enemySprite;
        [SerializeField] private TextMeshProUGUI enemyDesc;
        [SerializeField] private List<Sprite> regSprite;
        [SerializeField] private Image regIcon;
        [SerializeField] private TextMeshProUGUI regText;
        [Serializable] public class EnemySpriteClass
        {
            public EnemyBase.EnemyClasses enemyClasses;
            public Sprite enemySprite;
        }
        public List<EnemySpriteClass> enemySpriteList;

        private Dictionary<EnemyBase.SpawnZones, Transform> _spawnZones;
        public int randomX;
        public int randomY;
        private List<EnemyBase.EnemyClasses> _enemyClassList = new List<EnemyBase.EnemyClasses>();
        private EnemyBase _bossObject;

        private void Awake()
        {
            _spawnZones = new Dictionary<EnemyBase.SpawnZones, Transform>
            {
                { EnemyBase.SpawnZones.A, spawnZoneA },
                { EnemyBase.SpawnZones.B, spawnZoneB },
                { EnemyBase.SpawnZones.C, spawnZoneC },
                { EnemyBase.SpawnZones.D, spawnZoneD },
                { EnemyBase.SpawnZones.E, spawnZoneE },
                { EnemyBase.SpawnZones.F, spawnZoneF }
            };
        }

        public IEnumerator SpawnEnemies(int? count, EnemyBase.EnemyClasses? enemyClass, List<EnemyBase.SpawnZones> groupZone)
        {
            for (var i = 0; i < count; i++)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var spawnZone = groupZone[i % groupZone.Count];
                StartCoroutine(SpawnEnemy(enemyClass, spawnZone));
                yield return new WaitForSeconds(0.2f);
            }
        }
        private IEnumerator SpawnEnemy(EnemyBase.EnemyClasses? enemyClass, EnemyBase.SpawnZones spawnZone)
        {
            var enemyToSpawn = enemyPool.GetPooledEnemy(enemyClass);
            if (enemyToSpawn == null) yield break;
            var enemyBase = enemyToSpawn.GetComponent<EnemyBase>();
            StartCoroutine(GetEnemyDesc(enemyBase));
            var spawnPosition = new Vector3(0, 20, 0);
            yield return StartCoroutine(GetRandomPointInBounds(enemyBase.SpawnZone, pos => spawnPosition = pos));
            enemyBase.transform.position = spawnPosition;
            yield return StartCoroutine(enemyPatternManager.Zone_Move(enemyBase, spawnZone));
        }
        private IEnumerator GetEnemyDesc(EnemyBase enemyBase)
        { 
            LoadEnemyClassList();
            if (!_enemyClassList.Contains(enemyBase.enemyClass))
            {
                _enemyClassList.Add(enemyBase.enemyClass);
                SaveEnemyClassList();
                enemyDescPanel.SetActive(true);
                foreach (var enemy in enemySpriteList.Where(enemy => enemyBase.enemyClass == enemy.enemyClasses))
                {
                    enemySprite.sprite = enemy.enemySprite;
                }
                enemyDesc.text = enemyBase.enemyDesc;
                switch (enemyBase.RegistryType)
                {
                    case EnemyBase.RegistryTypes.Burn:
                        regIcon.sprite = regSprite[0];
                        regText.text = "-20%";
                        break;
                    case EnemyBase.RegistryTypes.Darkness:
                        regIcon.sprite = regSprite[1];
                        regText.text = "-20%";
                        break;
                    case EnemyBase.RegistryTypes.Water:
                        regIcon.sprite = regSprite[2];
                        regText.text = "-20%";
                        break;
                    case EnemyBase.RegistryTypes.Physics:
                        regIcon.sprite = regSprite[3];
                        regText.text = "-20%";
                        break;
                    case EnemyBase.RegistryTypes.Poison:
                        regIcon.sprite = regSprite[4];
                        regText.text = "-20%";
                        break;
                    case EnemyBase.RegistryTypes.None:
                        regIcon.gameObject.SetActive(false);
                        regText.text = null;
                        break;
                }
            }
            yield return new WaitForSecondsRealtime(4f);
            enemyDescPanel.SetActive(false);
       
        }
        private void SaveEnemyClassList()
        {
            var serializedData = string.Join(",", _enemyClassList.Select(e => e.ToString()).ToArray());
            PlayerPrefs.SetString("EnemyClassList", serializedData);
            PlayerPrefs.Save();
        }
        private void LoadEnemyClassList()
        {
            if (!PlayerPrefs.HasKey("EnemyClassList")) return;
            var serializedData = PlayerPrefs.GetString("EnemyClassList");
            _enemyClassList = serializedData.Split(',').Select(e => (EnemyBase.EnemyClasses)Enum.Parse(typeof(EnemyBase.EnemyClasses), e)).ToList();
        }

        public IEnumerator SpawnBoss(EnemyBase.EnemyClasses? bossClass, EnemyBase.SpawnZones spawnZone)
        {
           
            if (_bossObject != null)
            {
                Destroy(_bossObject);
            }
            enemyPool.enemyBases.Clear();
            foreach (var enemyBase in enemyManager.stageBoss.Where(enemyBase => enemyBase.enemyClass == bossClass))
            {
                _bossObject = Instantiate(enemyBase, transform);
                _bossObject.transform.position = gridManager.bossSpawnArea;
                yield return StartCoroutine(enemyPatternManager.Zone_Move(_bossObject, spawnZone));
            }
        }
        private IEnumerator GetRandomPointInBounds(EnemyBase.SpawnZones zone, Action<Vector3> callback)
        {
            var spawnPosition = new Vector3(0,20,0);
            switch (zone)
            {
                case EnemyBase.SpawnZones.A:
                {
                    var spawnPosY = _spawnZones[zone].position.y;
                    if (StageManager.Instance != null && StageManager.Instance.currentWave is 1 or 2 or 3)
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
                    randomY = Random.Range(EnforceManager.Instance.addRow ? 6 : 7, 9);
                    spawnPosition = new Vector3(spawnPosX, randomY + 0.5f,0);
                    break;
                }
                case EnemyBase.SpawnZones.C:
                {
                    var spawnPosX = _spawnZones[zone].position.x;
                    randomY = Random.Range(EnforceManager.Instance.addRow ? 6 : 7, 9);
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
                    var spawnPosY = gridManager.gridHeight;
                    randomX = Random.Range(2, 4);
                    spawnPosition = new Vector3(randomX, spawnPosY, 0);
                    break;
                }
                case EnemyBase.SpawnZones.F:
                {
                    var spawnPosY = gridManager.gridHeight; 
                    randomX = (Random.value > 0.5f) ? -1 : 6;
                    spawnPosition = new Vector3(randomX, spawnPosY, 0);
                    break;
                }
                case EnemyBase.SpawnZones.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
            callback?.Invoke(spawnPosition);
            yield return null;
        }
    }
}
