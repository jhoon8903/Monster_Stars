using System;
using System.Collections;
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
            var enemyObjectList = enemySpawnManager.FieldList;
            foreach (var enemyObject in enemyObjectList)
            {
                _enemyObjects = enemyObject;
                var enemyBase = _enemyObjects.GetComponent<EnemyBase>();
                enemyBase.EnemyProperty();
                var endPosition = new Vector3(_enemyObjects.transform.position.x, -4.0f, 0);
                var duration = enemyBase.MoveSpeed;

                switch (enemyBase.SpawnZone)
                {
                    case EnemyBase.EnemyZone.A:
                        PatternA(_enemyObjects, endPosition, duration);
                        break;
                    case EnemyBase.EnemyZone.B:
                        break;
                    case EnemyBase.EnemyZone.C:
                        break;
                    case EnemyBase.EnemyZone.D:
                        break;
                    case EnemyBase.EnemyZone.E:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            yield return null;
        }

        private static void PatternA(GameObject enemyObject, Vector3 endPosition, float duration)
        {
            enemyObject.transform.DOMove(endPosition, duration).SetEase(Ease.Linear);
        }
    }
}

