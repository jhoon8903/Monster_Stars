using System.Collections;
using System.Collections.Generic;
using Script.RewardScript;
using Script.UIManager;
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
        private Rigidbody2D _rb;

        private bool Chance(int percent)
        {
            return _random.Next(100) < percent;
        }

        public IEnumerator Zone_Move(EnemyBase enemyBase)
        {
            var slowCount = EnforceManager.Instance.slowCount;
            var speedReductionFactor = 1f + slowCount * 0.15f;
            _velocity = enemyBase.MoveSpeed * speedReductionFactor * moveSpeedOffset ;

            if (enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
            {
                enemyBase.Initialize();
                StartCoroutine(Rain(enemyBase, _velocity * 0.8f));
            }
            switch (enemyBase.SpawnZone)
            {
                case EnemyBase.SpawnZones.A:
                    StartCoroutine(Rain(enemyBase, _velocity));
                    break;
                case EnemyBase.SpawnZones.B:
                case EnemyBase.SpawnZones.C:
                    StartCoroutine(Diagonal(enemyBase, _velocity));
                    break;
                case EnemyBase.SpawnZones.D:
                    StartCoroutine(Zigzag(enemyBase, _velocity));
                    break;
                case EnemyBase.SpawnZones.E:
                    StartCoroutine(OutSide(enemyBase, _velocity));
                    break;
                case EnemyBase.SpawnZones.F:
                    StartCoroutine(InSide(enemyBase, _velocity));
                    break;
                default:
                    Debug.Log("어디에도 속하지 않음");
                    break;
            }
            yield return null;
        }

        private IEnumerator Rain(EnemyBase enemyBase, float velocity)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            _rb.velocity = new Vector2(0, -velocity);
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
                yield return new WaitForSeconds(0.1f);
            }
        }
        private IEnumerator Diagonal(EnemyBase enemyBase, float velocity)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;

            var startPosition = enemyBase.transform.position;
            var endX = enemyBase.SpawnZone == EnemyBase.SpawnZones.B ? Random.Range(4, 7) : Random.Range(-1, 2);
            var endPosition = new Vector2(endX, -1);
            var journeyLength = Vector2.Distance(startPosition, endPosition);
            var startTime = Time.time;
            while (gameManager.IsBattle)
            {
                var distCovered = (Time.time - startTime) * velocity;
                var fractionOfJourney = distCovered / journeyLength;
                _rb.position = Vector2.Lerp(startPosition, endPosition, fractionOfJourney);
                if (fractionOfJourney >= 1)
                {
                    break;
                }
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                if (enemyBase.isRestraint)
                {
                    yield return StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    yield return StartCoroutine(SlowEffect(enemyBase));
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        private IEnumerator Zigzag(EnemyBase enemyBase, float velocity)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            Vector2 startPosition = enemyBase.transform.position;
            var direction = Random.Range(0, 2) == 0 ? -1 : 1;
            var waypoints = new Vector2[5];
            for (var i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = new Vector2(startPosition.x + (i % 2 == 0 ? direction * 2 : 0), startPosition.y - (i * 2));
            }
            var waypointIndex = 0;
            while (gameManager.IsBattle)
            {
                var targetPosition = waypoints[waypointIndex];
                var journeyLength = Vector2.Distance(_rb.position, targetPosition);
                if (journeyLength <= 0.01f)
                {
                    waypointIndex++;
                    if (waypointIndex >= waypoints.Length) break;
                    continue;
                }
                var step = velocity * Time.deltaTime;
                _rb.position = Vector2.MoveTowards(_rb.position, targetPosition, step);
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                if (enemyBase.isRestraint)
                {
                    yield return StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    yield return StartCoroutine(SlowEffect(enemyBase));
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        private IEnumerator OutSide(EnemyBase enemyBase, float velocity)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;

            Vector2 startPosition = enemyBase.transform.position;
            var targetPosition = startPosition.x <= 2 
                ? new Vector2(-1, startPosition.y) 
                : new Vector2(6, startPosition.y);

            while (gameManager.IsBattle)
            {
                var journeyLength = Vector2.Distance(_rb.position, targetPosition);

                if (journeyLength <= 0.01f)
                {
                    targetPosition = new Vector2(targetPosition.x, -velocity);
                }

                var step = velocity * Time.deltaTime;
                _rb.position = Vector2.MoveTowards(_rb.position, targetPosition, step);

                yield return StartCoroutine(gameManager.WaitForPanelToClose());

                if (enemyBase.isRestraint)
                {
                    yield return StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    yield return StartCoroutine(SlowEffect(enemyBase));
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        private IEnumerator InSide(EnemyBase enemyBase, float velocity)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;

            Vector2 startPosition = enemyBase.transform.position;
            Vector2 targetPosition;

            if (startPosition.x <= -1) 
            {
                var randomX = Random.Range(3, 6);
                targetPosition = new Vector2(randomX, startPosition.y);
            }
            else 
            {
                var randomX = Random.Range(0, 3);
                targetPosition = new Vector2(randomX, startPosition.y);
            }

            while (gameManager.IsBattle)
            {
                var journeyLength = Vector2.Distance(_rb.position, targetPosition);

                if (journeyLength <= 0.01f)
                {
                    targetPosition = new Vector2(targetPosition.x, -velocity);
                }

                var step = velocity * Time.deltaTime;
                _rb.position = Vector2.MoveTowards(_rb.position, targetPosition, step);

                yield return StartCoroutine(gameManager.WaitForPanelToClose());

                if (enemyBase.isRestraint)
                {
                    yield return StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    yield return StartCoroutine(SlowEffect(enemyBase));
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
            if (EnforceManager.Instance.water2BleedAdditionalRestraint && Chance(20) && enemyBase.isBleed)
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
    }
}
