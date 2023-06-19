using System.Collections;
using UnityEngine;
using DG.Tweening;
using Script.RewardScript;
using Script.UIManager;
using Random = System.Random;

namespace Script.EnemyManagerScript
{
    public class EnemyPatternManager : MonoBehaviour
    {
        [SerializeField] private GameObject castle;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EnforceManager enforceManager;
        private GameObject _enemyObjects;
        private float _duration;
        private readonly Random _random = new System.Random();

        public IEnumerator Zone_Move(GameObject enemyObject)
        {
            _enemyObjects = enemyObject;
            var enemyBase = _enemyObjects.GetComponent<EnemyBase>(); 
            enemyBase.EnemyProperty();
            var position = _enemyObjects.transform.position;
            var endPosition = new Vector3(position.x, castle.transform.position.y-5, 0);
            var slowCount = enforceManager.slowCount;
            var speedReductionFactor = 1f + slowCount * 0.15f;
            speedReductionFactor = Mathf.Min(speedReductionFactor, 1.6f);
            _duration = enemyBase.MoveSpeed * 40f * speedReductionFactor;

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
                    Debug.Log("어디에도 속하지 않음");
                    break;
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
            var duration = bossObject.MoveSpeed * 40f;
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
                else
                {
                    yield return null;
                }
                yield return new WaitForSecondsRealtime(0.2f); // add some delay to prevent infinite loop
                totalEnemyCount = waveManager.enemyTotalCount;
            }
        }

        private IEnumerator RestrainEffect(EnemyBase enemyBase, Vector3 endPosition, float duration)
        {
            var overTime = enforceManager.IncreaseRestraintTime();
            var restraintColor = new Color(0.59f, 0.43f, 0f); 
            var originColor = new Color(1, 1, 1);
            
            enemyBase.GetComponent<SpriteRenderer>().DOColor(restraintColor, 0.1f);
            DOTween.Kill(enemyBase.transform);

            yield return new WaitForSecondsRealtime(overTime);

            yield return enemyBase.IsRestraint = false;
            enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
            enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
            }

        private IEnumerator SlowEffect(EnemyBase enemyBase, Vector3 endPosition, float duration)
        {
            var slowTime = enforceManager.waterIncreaseSlowTime;
            var slowPowerDuration = enforceManager.waterIncreaseSlowPower ? 2.2f : 1.6f;
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            if (enforceManager.waterStun)
            {
                if (_random.Next(100) < 15)
                {
                    enemyBase.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 1f, 1f, 0.3f), 0.1f);
                    DOTween.Kill(enemyBase.transform);
                    yield return new WaitForSecondsRealtime(1f);
                    enemyBase.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 1f, 1f, 1f), 0.1f);
                    enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
                }
                else
                {
                    enemyBase.GetComponent<SpriteRenderer>().DOColor(slowColor, 0.1f);
                    enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration * slowPowerDuration).SetEase(Ease.Linear);
                    yield return new WaitForSecondsRealtime(slowTime);
                    yield return enemyBase.IsSlow = false;
                    enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
                    enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
                }
            }
            else
            {
                enemyBase.GetComponent<SpriteRenderer>().DOColor(slowColor, 0.1f);
                enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration * slowPowerDuration).SetEase(Ease.Linear);
                yield return new WaitForSecondsRealtime(slowTime);
                yield return enemyBase.IsSlow = false;
                enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
                enemyBase.gameObject.transform.DOMoveY(endPosition.y, duration).SetEase(Ease.Linear);
            }
        }
    }
}