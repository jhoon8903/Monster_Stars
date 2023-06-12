using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.UIManager;

namespace Script.EnemyManagerScript
{
    public class EnemyPatternManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private GameObject castle;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private GameManager gameManager;
        private GameObject _enemyObjects;
        
        public IEnumerator Zone_Move()
        {
            var enemyObjectList = new List<GameObject>(enemyPool.SpawnEnemy());
            foreach (var enemyObject in enemyObjectList)
            {
                _enemyObjects = enemyObject;
                var enemyBase = _enemyObjects.GetComponent<EnemyBase>();
                enemyBase.EnemyProperty();
                var position = _enemyObjects.transform.position;
                var endPosition = new Vector3(position.x, castle.transform.position.y-5, 0);
                var duration = enemyBase.MoveSpeed * 50f;

                switch (enemyBase.SpawnZone)
                {
                    case EnemyBase.SpawnZones.A:
                        yield return StartCoroutine(PatternACoroutine(_enemyObjects, endPosition, duration));
                        break;
                    case EnemyBase.SpawnZones.B:
                        break;
                    case EnemyBase.SpawnZones.C:
                        break;
                    case EnemyBase.SpawnZones.D:
                        break;
                    case EnemyBase.SpawnZones.E:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public IEnumerator Boss_Move(GameObject boss)
        {
            _enemyObjects = boss;
            var bossObject = _enemyObjects.GetComponent<EnemyBase>();
            bossObject.EnemyProperty();
            var position = _enemyObjects.transform.position;
            var endPosition = new Vector3(position.x, castle.transform.position.y-5, 0);
            var duration = bossObject.MoveSpeed * 50f;
            yield return StartCoroutine(PatternACoroutine(_enemyObjects, endPosition, duration));
        }

        private IEnumerator PatternACoroutine(GameObject enemyObject, Vector3 endPosition, float duration)
        {
            gameManager.GameSpeed();
            var wave = waveManager.set;
            while (wave > 0)
            {
                enemyObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
                wave -= 1;
                
                yield return null;
            }
        }
    }
}