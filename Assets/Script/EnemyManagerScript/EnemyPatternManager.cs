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
        private bool _alreadySlow;
        private bool _alreadyRestrain;
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
            _moveSpeed = enemyBase.MoveSpeed * speedReductionFactor * moveSpeedOffset ;

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
            var enemyObject = _enemyRigidbodies[enemyBase];
            var targetPosition = new Vector2(enemyObject.transform.position.x, _endY);

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var step = moveToSpeed * Time.deltaTime;
                enemyObject.transform.position = Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                if (enemyBase.isRestraint)
                {
                   StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                   StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
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
                enemyObject.transform.position = Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);

                if (enemyObject.transform.position.Equals(targetPosition))
                {
                    break;
                }
 
                if (enemyBase.isRestraint)
                {
                    StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                    StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
                }
            }
        }
        private IEnumerator Zigzag(EnemyBase enemyBase, float moveToSpeed)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            var enemyObject = _enemyRigidbodies[enemyBase];
            var startPos = enemyObject.transform.position;
            var direction = Random.Range(0, 2) == 0 ? -1 : 1;
            var waypoints = new Vector2[5];
            for (var i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = new Vector2(startPos.x + (i % 2 == 0 ? direction * 2 : 0), startPos.y - (i * 2));
            }
            var waypointIndex = 0;
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var targetPosition = waypoints[waypointIndex];
                var journeyLength = Vector2.Distance(startPos, targetPosition);
                if (journeyLength <= 0.01f)
                {
                    waypointIndex++;
                    if (waypointIndex >= waypoints.Length) break;
                    continue;
                }
                var step = moveToSpeed * Time.deltaTime;
                startPos = Vector2.MoveTowards(startPos, targetPosition, step);
                if (enemyBase.isRestraint)
                {
                    StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                    StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
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
                enemyObject.transform.position = Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                if (enemyBase.isRestraint)
                {
                    StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                    StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
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
                    StartCoroutine(RestrainEffect(enemyBase, enemyObject, targetPosition, step));
                }
                if (enemyBase.isSlow)
                {
                    StartCoroutine(SlowEffect(enemyBase, enemyObject, targetPosition, step));
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
        private IEnumerator RestrainEffect(EnemyBase enemyBase , Component enemyObject, Vector2 targetPosition, float step)
        {
            var overTime = EnforceManager.Instance.IncreaseRestraintTime();
            var restraintColor = new Color(0.59f, 0.43f, 0f);
            var originColor = new Color(1, 1, 1);
            if (_alreadyRestrain) yield break;
            _alreadyRestrain = true;
            enemyBase.GetComponent<SpriteRenderer>().color = restraintColor;
            yield return new WaitForSeconds(overTime);
            while (gameManager.IsBattle)
            {
                enemyObject.transform.position =
                    Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                yield return null;
            }
            enemyBase.isRestraint = false;
            _alreadyRestrain = false;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
        }
        private IEnumerator SlowEffect(EnemyBase enemyBase , Component enemyObject, Vector2 targetPosition, float step)
        {
            var slowTime = 2f + (1f * EnforceManager.Instance.water2IncreaseSlowTime);
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            if (_alreadySlow) yield break;
            _alreadySlow = true;
            enemyBase.GetComponent<SpriteRenderer>().color = slowColor;
            if (EnforceManager.Instance.water2BleedAdditionalRestraint && Chance(20) && enemyBase.isBleed)
            {
                yield return new WaitForSeconds(1f);
            }
            var moveSpeedMultiplier = EnforceManager.Instance.waterIncreaseSlowPower ? 0.4f : 0.6f;
            step *= moveSpeedMultiplier;
            yield return new WaitForSeconds(slowTime);
           
            while (gameManager.IsBattle)
            {
                enemyObject.transform.position =
                    Vector2.MoveTowards(enemyObject.transform.position, targetPosition, step);
                yield return null;
            }
            enemyBase.isSlow = false;
            _alreadySlow = false;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
        }
    }
}
