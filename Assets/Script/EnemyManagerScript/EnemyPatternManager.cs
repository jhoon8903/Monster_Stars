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
        private Tween _movementTween;
        private Tween _defaultTween;

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
                        StartCoroutine(PatternACoroutine(_enemyObjects, endPosition, _duration));
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

        private IEnumerator PatternACoroutine(GameObject enemyObject, Vector3 endPosition, float duration)
        {
            gameManager.GameSpeed();
            var wave = waveManager.set;
            var totalEnemyCount = waveManager.enemyTotalCount;
            var enemyBase = enemyObject.GetComponent<EnemyBase>();


            while (totalEnemyCount > 0)
            {
                if (enemyBase.IsRestraint)
                {
                    yield return StartCoroutine(RestrainEffect(enemyBase));
                }
                else if (enemyBase.IsSlow) 
                { 
                    yield return StartCoroutine(SlowEffect(enemyBase, endPosition, duration));
                }
                else if (!enemyBase.IsSlow && !enemyBase.IsRestraint)
                {
                    if (_movementTween != null && _movementTween.IsPlaying())
                    {
                        _movementTween.Kill();
                    }
                    yield return _defaultTween= enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
                    // yield return _movementTween.WaitForCompletion();
                    _movementTween = _defaultTween;
                }
                totalEnemyCount = waveManager.enemyTotalCount;
                Debug.Log(totalEnemyCount);
            }
        }

        private IEnumerator RestrainEffect(EnemyBase enemyBase)
        {
            var restraintColor = new Color(0.59f, 0.43f, 0f);
            var originColor = new Color(1, 1, 1);
            enemyBase.GetComponent<SpriteRenderer>().DOColor(restraintColor, 0.1f);
            
            DOTween.Kill(enemyBase.transform);
            yield return new WaitForSecondsRealtime(1f);
            enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
            enemyBase.IsRestraint = false;
            // yield return _defaultTween;
        }

        private IEnumerator SlowEffect(EnemyBase enemyBase, Vector3 endPosition, float duration)
        {
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            enemyBase.GetComponent<SpriteRenderer>().DOColor(slowColor, 0.1f);
            
            // Check if a tween is already active and kill it
            if (_movementTween != null && _movementTween.IsPlaying())
            {
                _movementTween.Kill();
            }
            // Create a new tween and store it in movementTween
            _movementTween = enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration * 2.5f).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(2f);
            enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
            enemyBase.IsSlow = false;
        }
    }
}
