using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Script.CharacterGroupScript;
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
        public bool IncreaseRestraintTime { get; set; } = false;
        public bool IncreaseRestraintDamage { get; set; } = false;

        public IEnumerator Zone_Move(GameObject enemyObject)
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
            yield return null;
        }

        public IEnumerator Boss_Move(GameObject boss)
        {
            _enemyObjects = boss;
            var bossObject = _enemyObjects.GetComponent<EnemyBase>();
            bossObject.EnemyProperty();
            bossObject.Initialize();
            var position = _enemyObjects.transform.position;
            var endPosition = new Vector3(position.x, castle.transform.position.y-5, 0);
            var duration = bossObject.MoveSpeed * 50f;
            StartCoroutine(PatternACoroutine(_enemyObjects, endPosition, duration));
            yield return null;
        }

        private IEnumerator PatternACoroutine(GameObject enemyObject, Vector3 endPosition, float duration)
        {
            gameManager.GameSpeed();
            var totalEnemyCount = waveManager.enemyTotalCount;
            var enemyBase = enemyObject.GetComponent<EnemyBase>();
            enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);

            while (totalEnemyCount > 0)
            {
                if (enemyBase.IsRestraint)
                {
                   StartCoroutine(RestrainEffect(enemyBase, endPosition, duration));
                }
                else if (enemyBase.IsSlow)
                {
                   StartCoroutine(SlowEffect(enemyBase, endPosition, duration));
                }
                yield return new WaitForSecondsRealtime(0.2f); // add some delay to prevent infinite loop
                totalEnemyCount = waveManager.enemyTotalCount;
            }
        }

        private IEnumerator RestrainEffect(EnemyBase enemyBase, Vector3 endPosition, float duration)
        {
            var characterDamage = enemyBase.attackChar.GetComponent<CharacterBase>().defaultDamage;
            var overTime = IncreaseRestraintTime ? 2f : 1f;
            var restraintDamage = IncreaseRestraintDamage ? characterDamage / 5 : characterDamage / 10;
            var restraintColor = new Color(0.59f, 0.43f, 0f);
            var originColor = new Color(1, 1, 1);
            
            enemyBase.GetComponent<SpriteRenderer>().DOColor(restraintColor, 0.1f);
            DOTween.Kill(enemyBase.transform);
            StartCoroutine(RestraintDamageOverTime(enemyBase, restraintDamage, overTime));
            
            yield return new WaitForSecondsRealtime(overTime);
            
            yield return enemyBase.IsRestraint = false;
            enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
            enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
        }

        private static IEnumerator RestraintDamageOverTime(EnemyBase enemyBase, float damage, float overTime)
        {
            while (overTime > 0)
            {
                enemyBase.ReceiveDamage(damage, CharacterBase.UnitProperties.Divine);
                yield return new WaitForSecondsRealtime(0.5f);
                overTime -= 0.5f;
            }
        }

        private IEnumerator SlowEffect(EnemyBase enemyBase, Vector3 endPosition, float duration)
        {
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            enemyBase.GetComponent<SpriteRenderer>().DOColor(slowColor, 0.1f);
            enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration * 1.6f).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(2f);
            yield return enemyBase.IsSlow = false;
            enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
            enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
        }
    }
}