using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Script.EnemyManagerScript
{
    public class EnemyPatternManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        private GameObject _enemyObjects;
        
        public IEnumerator Zone_Move()
        {
            var enemyObjectList = new List<GameObject>(enemySpawnManager.fieldList);
            foreach (var enemyObject in enemyObjectList)
            {
                _enemyObjects = enemyObject;
                var enemyBase = _enemyObjects.GetComponent<EnemyBase>();
                enemyBase.EnemyProperty();
                var endPosition = new Vector3(_enemyObjects.transform.position.x, -4.0f, 0);
                var duration = enemyBase.MoveSpeed * 10f;

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

        private static IEnumerator PatternACoroutine(GameObject enemyObject, Vector3 endPosition, float duration)
        {
            Tweener move = enemyObject.transform.DOMove(endPosition, duration).SetEase(Ease.Linear);
            yield return null;   
        }
    }
}