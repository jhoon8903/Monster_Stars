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
        public float moveSpeedOffset = 0.5f;
        private readonly System.Random _random = new System.Random();
        private bool _alreadySlow;
        private bool _alreadyRestrain;
        public IEnumerator Zone_Move(EnemyBase enemyBase)
        {
            
            var slowCount = EnforceManager.Instance.SlowCount();
            var speedReductionFactor = 1f + slowCount * 0.15f;
            _velocity = enemyBase.MoveSpeed * speedReductionFactor * moveSpeedOffset ;

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
            bossObject.Initialize();
            StartCoroutine(PatternACoroutine(bossObject, _velocity * 0.8f));
            yield return null;
        }

        private IEnumerator PatternACoroutine(EnemyBase enemyBase, float velocity)
        {
            
            var rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = rb;
            rb.velocity = new Vector2(0, -velocity);

            while (gameManager.IsBattle)
            {
               yield return StartCoroutine(gameManager.WaitForPanelToClose());
                if (enemyBase.isRestraint)
                {
                    yield return StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    yield return StartCoroutine(SlowEffect(enemyBase));
                }
                if (enemyBase.isPoison)
                {
                    yield return StartCoroutine(PoisonEffect(enemyBase));
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator RestrainEffect(EnemyBase enemyBase)
        {
            var overTime = EnforceManager.Instance.IncreaseRestraintTime();
            var restraintColor = new Color(0.59f, 0.43f, 0f);
            var originColor = new Color(1, 1, 1);
            if (_alreadyRestrain) yield break;
            _alreadyRestrain = true;
            enemyBase.GetComponent<SpriteRenderer>().color = restraintColor;
            _enemyRigidbodies[enemyBase].velocity = Vector2.zero;
            yield return new WaitForSeconds(overTime);
            _enemyRigidbodies[enemyBase].velocity = new Vector2(0, -_velocity);
            enemyBase.isRestraint = false;
            _alreadyRestrain = false;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
        }

        private IEnumerator SlowEffect(EnemyBase enemyBase)
        {
            var slowTime = 2f + (1f * EnforceManager.Instance.water2IncreaseSlowTime);
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            if (_alreadySlow) yield break;
            _alreadySlow = true;
            enemyBase.GetComponent<SpriteRenderer>().color = slowColor;
            if (EnforceManager.Instance.water2BleedAdditionalRestraint && _random.Next(100)<20 && enemyBase.isBleed)
            { 
                _enemyRigidbodies[enemyBase].velocity = Vector2.zero; 
                yield return new WaitForSeconds(1f);
            }
            _enemyRigidbodies[enemyBase].velocity = 
                EnforceManager.Instance.waterIncreaseSlowPower 
                    ? new Vector2(0, -_velocity*0.4f)
                    : new Vector2(0, -_velocity*0.6f);
            yield return new WaitForSeconds(slowTime);
            _enemyRigidbodies[enemyBase].velocity = new Vector2(0, -_velocity);
            enemyBase.isSlow = false;
            _alreadySlow = false;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
        }

        private IEnumerator PoisonEffect(EnemyBase enemyBase)
        {
            const float overtime = 1f;
            if (EnforceManager.Instance.firePoisonAdditionalStun && _random.Next(100)<20)
            {
                _enemyRigidbodies[enemyBase].velocity = Vector2.zero;
                yield return new WaitForSeconds(overtime);
            }
            _enemyRigidbodies[enemyBase].velocity = new Vector2(0, -_velocity);
        }
    }
}
