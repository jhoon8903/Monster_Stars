using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.UIManager;

namespace Script.EnemyManagerScript
{
    public class EnemyPatternManager : MonoBehaviour
    {
        [SerializeField] private GameObject castle;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private GameManager gameManager;
        private GameObject _enemyObjects;
        private float _duration;

        public IEnumerator Zone_Move(IEnumerable<GameObject> spawnList)
        {
            var spawnListCopy = new List<GameObject>(spawnList);
            foreach (var enemyObject in spawnListCopy)
            {
                _enemyObjects = enemyObject;
                var enemyBase = _enemyObjects.GetComponent<EnemyBase>();
                enemyBase.EnemyProperty();
                var position = _enemyObjects.transform.position;
                var endPosition = new Vector3(position.x, castle.transform.position.y-5, 0);
                var slowCount = FindObjectOfType<CharacterManager>().slowCount;
                if (slowCount >=1)
                {
                    var speedReductionFactor = 1f + slowCount * 0.15f;
                    speedReductionFactor = Mathf.Min(speedReductionFactor, 1.6f);
                    _duration = enemyBase.MoveSpeed * 50f * speedReductionFactor;
                }
                else
                {
                    _duration = enemyBase.MoveSpeed * 50f;
                }

                switch (enemyBase.SpawnZone)
                {
                    case EnemyBase.SpawnZones.A:
                        PatternACoroutine(_enemyObjects, endPosition, _duration);
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
            yield return null;
        }

        public IEnumerator Boss_Move(GameObject boss)
        {
            _enemyObjects = boss;
            var bossObject = _enemyObjects.GetComponent<EnemyBase>();
            bossObject.EnemyProperty();
            var position = _enemyObjects.transform.position;
            var endPosition = new Vector3(position.x, castle.transform.position.y-5, 0);
            var duration = bossObject.MoveSpeed * 50f;
            PatternACoroutine(_enemyObjects, endPosition, duration);
            yield return null;
        }

        private void PatternACoroutine(GameObject enemyObject, Vector3 endPosition, float duration)
        {
            gameManager.GameSpeed();
            var wave = waveManager.set;
            var enemyBase = enemyObject.GetComponent<EnemyBase>();

            while (wave > 0)
            {
                Debug.Log($"속박: {enemyBase.IsRestraint} / 둔화: {enemyBase.IsSlow}");

                if (enemyBase.IsRestraint)
                { 
                    StartCoroutine(RestrainEffect(enemyBase));
                }
                else if (enemyBase.IsSlow)
                {
                    StartCoroutine(SlowEffect(enemyBase, endPosition, duration));
                }
                else
                {
                    enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
                }
                wave -= 1;
            }
        }

        private static IEnumerator RestrainEffect(EnemyBase enemyBase)
        {
            Debug.Log("속박");
            var restraintColor = new Color(0.59f, 0.43f, 0f);
            var originColor = new Color(1, 1, 1);
            
            enemyBase.GetComponent<SpriteRenderer>().DOColor(restraintColor, 0.2f);
            DOTween.Kill(enemyBase.transform);
            yield return new WaitForSecondsRealtime(1f);
            enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.2f);
            enemyBase.IsRestraint = false;
            Debug.Log("속박해제");
        }

        private static IEnumerator SlowEffect(EnemyBase enemyBase, Vector3 endPosition, float duration)
        {
            Debug.Log("둔화");
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            
            enemyBase.GetComponent<SpriteRenderer>().DOColor(slowColor, 0.2f);
            enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration*2.5f).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(1f);
            enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.2f);
            enemyBase.IsSlow = false;
            Debug.Log("둔화해제");
        }
    }
}