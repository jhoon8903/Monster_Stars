using System.Collections;
using System.Collections.Generic;
using Script.RewardScript;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyPatternManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        private readonly Dictionary<EnemyBase, Rigidbody2D> _enemyRigidbodies = new Dictionary<EnemyBase, Rigidbody2D>();
        private float _velocity;
        private readonly System.Random _random = new System.Random();
        public IEnumerator Zone_Move(EnemyBase enemyBase)
        {
            var slowCount = EnforceManager.Instance.SlowCount();
            var speedReductionFactor = 1f + slowCount * 0.15f;
            _velocity = enemyBase.MoveSpeed * speedReductionFactor * 0.5f ;

            switch (enemyBase.SpawnZone)
            {
                case EnemyBase.SpawnZones.A:
                    StartCoroutine(PatternACoroutine(enemyBase, _velocity));
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
            StartCoroutine(PatternACoroutine(bossObject, _velocity * 0.8f));
            yield return null;
        }

        private IEnumerator PatternACoroutine(EnemyBase enemyBase, float velocity)
        {
            gameManager.GameSpeed();
            var rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = rb;
            rb.velocity = new Vector2(0, -velocity);

            while (gameManager.IsBattle)
            {
                if (enemyBase.isRestraint)
                {
                    yield return StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    yield return StartCoroutine(SlowEffect(enemyBase));
                }
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        private IEnumerator RestrainEffect(EnemyBase enemyBase)
        {
            var overTime = EnforceManager.Instance.IncreaseRestraintTime();
            var restraintColor = new Color(0.59f, 0.43f, 0f);
            var originColor = new Color(1, 1, 1);

            enemyBase.GetComponent<SpriteRenderer>().color = restraintColor;
            _enemyRigidbodies[enemyBase].velocity = Vector2.zero;
            yield return new WaitForSecondsRealtime(overTime);
            _enemyRigidbodies[enemyBase].velocity = new Vector2(0, _velocity);
            enemyBase.isRestraint = false;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
        }

        private IEnumerator SlowEffect(EnemyBase enemyBase)
        {
            var slowTime = EnforceManager.Instance.waterIncreaseSlowTime;
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);

            enemyBase.GetComponent<SpriteRenderer>().color = slowColor;
            if (EnforceManager.Instance.waterStun && _random.Next(100) < 15)
            {
                _enemyRigidbodies[enemyBase].velocity = Vector2.zero;
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                _enemyRigidbodies[enemyBase].velocity = 
                    EnforceManager.Instance.waterIncreaseSlowPower 
                        ? new Vector2(0, -_velocity*0.6f) 
                        : new Vector2(0, -_velocity*0.4f);
                yield return new WaitForSecondsRealtime(slowTime);
            }
            _enemyRigidbodies[enemyBase].velocity = new Vector2(0, -_velocity);
            enemyBase.isSlow = false;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
        }
    }
}
