using System.Collections;
using System.Collections.Generic;
using Script.RewardScript;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.EnemyManagerScript
{
    public class EnemyPatternManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject castle;
        private readonly Dictionary<EnemyBase, Rigidbody2D> _enemyRigidbodies = new Dictionary<EnemyBase, Rigidbody2D>();
        private float _moveSpeed;
        public float moveSpeedOffset;
        private readonly System.Random _random = new System.Random();
        private bool _alreadyRestrain;
        private Dictionary<EnemyBase, bool> _alreadySlow = new Dictionary<EnemyBase, bool>();
        private Rigidbody2D _rb;
        private float _endY;

        private void Awake()
        {
           _endY = castle.GetComponent<BoxCollider2D>().transform.position.y;
        }

        private bool Chance(int percent)
        {
            return _random.Next(100) < percent;
        }

        public IEnumerator Zone_Move(EnemyBase enemyBase)
        {
            var slowCount = EnforceManager.Instance.slowCount;
            var speedReductionFactor = 1f + slowCount * 0.15f;
            _moveSpeed = enemyBase.MoveSpeed * speedReductionFactor * moveSpeedOffset * Time.deltaTime;

            if (enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
            {
                enemyBase.Initialize();
                StartCoroutine(Rain(enemyBase, _moveSpeed * 0.8f));
            }
            switch (enemyBase.SpawnZone)
            {
                case EnemyBase.SpawnZones.A:
                    StartCoroutine(Rain(enemyBase, _moveSpeed));
                    break;
                case EnemyBase.SpawnZones.B:
                case EnemyBase.SpawnZones.C:
                    StartCoroutine(Diagonal(enemyBase, _moveSpeed));
                    break;
                case EnemyBase.SpawnZones.D:
                    StartCoroutine(Zigzag(enemyBase, _moveSpeed));
                    break;
                case EnemyBase.SpawnZones.E:
                    StartCoroutine(OutSide(enemyBase, _moveSpeed));
                    break;
                case EnemyBase.SpawnZones.F:
                    StartCoroutine(InSide(enemyBase, _moveSpeed));
                    break;
                default:
                    Debug.Log("어디에도 속하지 않음");
                    break;
            }
            yield return null;
        }

        private IEnumerator Rain(EnemyBase enemyBase, float moveToSpeed)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            var targetPosition = new Vector2(_enemyRigidbodies[enemyBase].transform.position.x, _endY);

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, moveToSpeed);
                if (enemyBase.isRestraint)
                { 
                   StartCoroutine(RestrainEffect(enemyBase, _enemyRigidbodies[enemyBase], targetPosition, moveToSpeed));
                }
                if (enemyBase.isSlow)
                { 
                   StartCoroutine(SlowEffect(enemyBase, _enemyRigidbodies[enemyBase], targetPosition, moveToSpeed));
                }
            }
        }
        private IEnumerator Diagonal(EnemyBase enemyBase, float moveToSpeed)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            var enemyObject = _enemyRigidbodies[enemyBase];
            var endX = enemyBase.SpawnZone == EnemyBase.SpawnZones.B ? Random.Range(4, 7) : Random.Range(-1, 2);
            var targetPosition = new Vector2(endX, _endY);

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var step = moveToSpeed * Time.deltaTime;

                if (enemyObject.transform.position.Equals(targetPosition))
                {
                    break;
                }
 
                if (enemyBase.isRestraint)
                {
                    yield return    StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                    yield return    StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
                }
                else
                {
                    enemyObject.transform.position = Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                }
            }
        }
        private IEnumerator Zigzag(EnemyBase enemyBase, float moveToSpeed)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            var enemyObject = _enemyRigidbodies[enemyBase];
            var direction = Random.Range(0, 2) == 0 ? -1 : 1;
            var waypoints = new Vector2[5];
            for (var i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = new Vector2(enemyObject.transform.position.x + (i % 2 == 0 ? direction * 2 : 0), enemyObject.transform.position.y - (i * 2));
            }
            var waypointIndex = 0;
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var targetPosition = waypoints[waypointIndex];
                var journeyLength = Vector2.Distance(enemyObject.transform.position, targetPosition);
                if (journeyLength <= 0.01f)
                {
                    waypointIndex++;
                    if (waypointIndex >= waypoints.Length) break;
                    continue;
                }
                var step = moveToSpeed * Time.deltaTime;
                if (enemyBase.isRestraint)
                {
                    yield return    StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                    yield return    StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
                }
                else
                {
                    enemyObject.transform.position = Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                }
            }
        }
        private IEnumerator OutSide(EnemyBase enemyBase, float moveToSpeed)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            var enemyObject = _enemyRigidbodies[enemyBase];
            var targetPosition = enemyObject.transform.position.x <= 2 ? new Vector2(-1, enemyObject.transform.position.y) : new Vector2(6, enemyObject.transform.position.y);

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
               
                var journeyLength = Vector2.Distance(enemyObject.transform.position, targetPosition);

                if (journeyLength <= 0.01f)
                {
                    targetPosition = new Vector2(targetPosition.x, _endY);
                }

                var step = moveToSpeed * Time.deltaTime;
                if (enemyBase.isRestraint)
                {
                    yield return    StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                    yield return    StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
                }
                else
                {
                    enemyObject.transform.position = Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                }
            }
        }
        private IEnumerator InSide(EnemyBase enemyBase, float moveToSpeed)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            var enemyObject = _enemyRigidbodies[enemyBase];
            Vector3 targetPosition;

            if (enemyObject.transform.position.x <= -1) 
            {
                var randomX = Random.Range(3, 6);
                targetPosition = new Vector3(randomX, enemyObject.transform.position.y);
            }
            else 
            {
                var randomX = Random.Range(0, 3);
                targetPosition = new Vector3(randomX, enemyObject.transform.position.y);
            }

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                
                var step = moveToSpeed * Time.deltaTime;
               
                if (enemyBase.isRestraint)
                {
                    yield return    StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                    yield return    StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
                }
                else
                {
                    enemyObject.transform.position = Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                }
                
                if (enemyObject.transform.position.x.Equals(targetPosition.x))
                {
                    targetPosition = new Vector3(targetPosition.x, _endY);
                }
            }
        }
        private IEnumerator RestrainEffect(EnemyBase enemyBase, Component enemyObject, Vector2 targetPosition, float step)
        {
            var restrainColor = new Color(1, 0.5f, 0);
            var originColor = new Color(1, 1, 1);
            if (_alreadyRestrain) yield break;
            _alreadyRestrain = true;
            enemyBase.GetComponent<SpriteRenderer>().color = restrainColor;
            step = 0;
            var restrainTime = EnforceManager.Instance.firePoisonAdditionalStun && Chance(20) && enemyBase.isBleed
                ? 1f : 1f + 1f * EnforceManager.Instance.IncreaseRestraintTime();
            while (gameManager.IsBattle)
            { 
                yield return StartCoroutine(gameManager.WaitForPanelToClose()); ;
                enemyObject.transform.position = Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                restrainTime -= Time.deltaTime;
                if (restrainTime <= 0) yield break;
                yield return null;
            }
            enemyBase.isRestraint = false;
            _alreadyRestrain = false;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
        }


        private IEnumerator SlowEffect(EnemyBase enemyBase , Component enemyObject, Vector2 targetPosition, float step)
        {
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            var slowTime = EnforceManager.Instance.water2BleedAdditionalRestraint && Chance(20) && enemyObject.GetComponent<EnemyBase>().isBleed 
                ? 1f : 2f + 1f * EnforceManager.Instance.water2IncreaseSlowTime;
            var moveSpeedMultiplier = EnforceManager.Instance.waterIncreaseSlowPower ? 0.04f : 0.06f;

            if (!_alreadySlow.TryGetValue(enemyBase, out enemyObject.GetComponent<EnemyBase>().isSlow))
            {
                enemyObject.GetComponent<EnemyBase>().isSlow= false;
                _alreadySlow[enemyBase] = false;
            }

            if (enemyObject.GetComponent<EnemyBase>().isSlow) yield break;

            _alreadySlow[enemyBase] = true;
            enemyObject.GetComponent<SpriteRenderer>().color = slowColor;
            var adjustedStep = step * moveSpeedMultiplier;

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                enemyObject.transform.position =
                    Vector3.MoveTowards(enemyObject.transform.position, targetPosition, adjustedStep);
                slowTime -= Time.deltaTime;

                if (slowTime <= 0) yield break;
            }
            enemyObject.GetComponent<SpriteRenderer>().color = originColor;
            enemyObject.GetComponent<EnemyBase>().isSlow = false;
            _alreadySlow[enemyBase] = false;
        }
    }
}
