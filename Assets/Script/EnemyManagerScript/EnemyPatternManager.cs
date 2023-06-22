using System.Collections;
using UnityEngine;
using DG.Tweening;
using Script.RewardScript;
using Random = System.Random;

namespace Script.EnemyManagerScript
{
    public class EnemyPatternManager : MonoBehaviour
    {
        [SerializeField] private GameObject castle;
        [SerializeField] private GameManager gameManager;
        private float _duration;
        private readonly Random _random = new System.Random();

        public IEnumerator Zone_Move(EnemyBase enemyBase)
        {
            var endPosition = castle.transform.position.y-4;
            var slowCount = EnforceManager.Instance.SlowCount();
            var speedReductionFactor = 1f + slowCount * 0.15f;
            _duration = enemyBase.MoveSpeed * 40f * speedReductionFactor;

            switch (enemyBase.SpawnZone)
            {
                case EnemyBase.SpawnZones.A:
                    StartCoroutine(PatternACoroutine(enemyBase, endPosition, _duration));
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
            var bossObject = boss.GetComponent<EnemyBase>();
            bossObject.EnemyProperty();
            bossObject.Initialize();
            var endPosition = castle.transform.position.y-5;
            var duration = bossObject.MoveSpeed * 40f;
            StartCoroutine(PatternACoroutine(bossObject, endPosition, duration));
            yield return null;
        }

        private IEnumerator PatternACoroutine(EnemyBase enemyBase, float endPosition, float duration)
        {
            gameManager.GameSpeed();
             enemyBase.transform.DOMoveY(endPosition, duration).SetEase(Ease.Linear);
            while (gameManager.IsBattle)
            {
                if (enemyBase.isRestraint)
                {
                    StartCoroutine(RestrainEffect(enemyBase, endPosition, duration));
                }
                
                if (enemyBase.isSlow)
                {
                    
                    StartCoroutine(SlowEffect(enemyBase, endPosition, duration));
                }
                yield return new WaitForSecondsRealtime(0.1f); // add some delay to prevent infinite loop
            }
        }

        private IEnumerator RestrainEffect(EnemyBase enemyBase, float endPosition, float duration)
        {
            var overTime = EnforceManager.Instance.IncreaseRestraintTime();
            var restraintColor = new Color(0.59f, 0.43f, 0f); 
            var originColor = new Color(1, 1, 1);
            
            yield return enemyBase.GetComponent<SpriteRenderer>().DOColor(restraintColor, 0.1f);
            yield return DOTween.Pause(overTime);
            yield return enemyBase.isRestraint = false;
            enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
        }

        private IEnumerator SlowEffect(EnemyBase enemyBase, float endPosition, float duration)
        {
            var slowTime = EnforceManager.Instance.waterIncreaseSlowTime;
            var slowPowerDuration = EnforceManager.Instance.waterIncreaseSlowPower ? 2.2f : 1.6f;
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            if (EnforceManager.Instance.waterStun && _random.Next(100) < 15)
            {
                yield return enemyBase.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 1f, 1f, 0.3f), 0.1f);
                enemyBase.DOPause();
                yield return new WaitForSecondsRealtime(1f);
                enemyBase.DOPlay();
                enemyBase.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 1f, 1f, 1f), 0.1f);
            }
            else
            {
                enemyBase.GetComponent<SpriteRenderer>().DOColor(slowColor, 0.1f);
                enemyBase.DOPause();
                var slow = enemyBase.gameObject.transform.DOMoveY(endPosition, duration * slowPowerDuration).SetEase(Ease.Linear);
                yield return new WaitForSecondsRealtime(slowTime);
                DOTween.Kill(slow);
                enemyBase.DOPlay();
                yield return enemyBase.isSlow = false;
                enemyBase.GetComponent<SpriteRenderer>().DOColor(originColor, 0.1f);
            }
        }
    }
}