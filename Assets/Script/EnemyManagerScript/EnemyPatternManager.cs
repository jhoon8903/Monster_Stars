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
        private readonly Dictionary<EnemyBase, bool> _alreadySlow = new Dictionary<EnemyBase, bool>();
        private readonly Dictionary<EnemyBase, bool> _alreadyRestrain = new Dictionary<EnemyBase, bool>();
        private Rigidbody2D _rb;
        private float _endY;
        private int _slowCount;
        private float _speedReductionFactor;

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
            if (enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
            {
                enemyBase.Initialize();
                StartCoroutine(Rain(enemyBase));
            }

            switch (enemyBase.SpawnZone)
            {
                case EnemyBase.SpawnZones.A:
                    StartCoroutine(Rain(enemyBase));
                    break;
                case EnemyBase.SpawnZones.B:
                case EnemyBase.SpawnZones.C:
                    StartCoroutine(Diagonal(enemyBase));
                    break;
                case EnemyBase.SpawnZones.D:
                    StartCoroutine(Zigzag(enemyBase));
                    break;
                case EnemyBase.SpawnZones.E:
                    StartCoroutine(OutSide(enemyBase));
                    break;
                case EnemyBase.SpawnZones.F:
                    StartCoroutine(InSide(enemyBase));
                    break;
                default:
                    Debug.Log("어디에도 속하지 않음");
                    break;
            }
            yield return null;
        }

        private IEnumerator Rain(EnemyBase enemyBase)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            
            var targetPosition = new Vector2(_enemyRigidbodies[enemyBase].transform.position.x, _endY);

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                _slowCount = EnforceManager.Instance.slowCount;
                _speedReductionFactor = 1f + _slowCount * 0.15f;
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed );
               
                if (enemyBase.isRestraint)
                {
                   StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                { 
                   StartCoroutine(SlowEffect(enemyBase));
                }
            }
        }
        private IEnumerator Diagonal(EnemyBase enemyBase)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;

            var endX = enemyBase.SpawnZone == EnemyBase.SpawnZones.B ? Random.Range(4, 7) : Random.Range(-1, 2);
            var targetPosition = new Vector2(endX, _endY);

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                _slowCount = EnforceManager.Instance.slowCount;
                _speedReductionFactor = 1f + _slowCount * 0.15f;
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
                
                if (enemyBase.isRestraint)
                { 
                    StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    StartCoroutine(SlowEffect(enemyBase));
                }
            }
        }
        private IEnumerator Zigzag(EnemyBase enemyBase)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;

            var direction = Random.Range(0, 2) == 0 ? -1 : 1;
            
            var waypoints = new Vector2[5];
            
            for (var i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = new Vector2(_enemyRigidbodies[enemyBase].transform.position.x + (i % 2 == 0 ? direction * 2 : 0), _enemyRigidbodies[enemyBase].transform.position.y - (i * 2));
            }
            
            var waypointIndex = 0;
           
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var targetPosition = waypoints[waypointIndex];
                var journeyLength = Vector2.Distance(_enemyRigidbodies[enemyBase].transform.position, targetPosition);
                
                _slowCount = EnforceManager.Instance.slowCount;
                _speedReductionFactor = 1f + _slowCount * 0.15f;
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
                
                if (journeyLength <= 0.01f)
                {
                    waypointIndex++;
                    if (waypointIndex >= waypoints.Length) break;
                    continue;
                }

                if (enemyBase.isRestraint)
                {
                    StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                   StartCoroutine(SlowEffect(enemyBase));
                }
            }
        }
        private IEnumerator OutSide(EnemyBase enemyBase)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;

            var targetPosition = _enemyRigidbodies[enemyBase].transform.position.x <= 2 
                ? new Vector2(-1, _enemyRigidbodies[enemyBase].transform.position.y) 
                : new Vector2(6, _enemyRigidbodies[enemyBase].transform.position.y);

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var journeyLength = Vector2.Distance(_enemyRigidbodies[enemyBase].transform.position, targetPosition);
                _slowCount = EnforceManager.Instance.slowCount;
                _speedReductionFactor = 1f + _slowCount * 0.15f;
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
               
                if (journeyLength <= 0.01f)
                {
                    targetPosition = new Vector2(targetPosition.x, _endY);
                }

                if (enemyBase.isRestraint)
                { 
                    StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                { 
                    StartCoroutine(SlowEffect(enemyBase));
                }
            }
        }
        private IEnumerator InSide(EnemyBase enemyBase)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            Vector3 targetPosition;

            if (_enemyRigidbodies[enemyBase].transform.position.x <= -1) 
            {
                var randomX = Random.Range(3, 6);
                targetPosition = new Vector3(randomX, _enemyRigidbodies[enemyBase].transform.position.y);
            }
            else 
            {
                var randomX = Random.Range(0, 3);
                targetPosition = new Vector3(randomX, _enemyRigidbodies[enemyBase].transform.position.y);
            }

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var journeyLength = Vector2.Distance(_enemyRigidbodies[enemyBase].transform.position, targetPosition);
                
                _slowCount = EnforceManager.Instance.slowCount;
                _speedReductionFactor = 1f + _slowCount * 0.15f;
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
                
                if (journeyLength <= 0.01f)
                {
                    targetPosition = new Vector3(targetPosition.x, _endY);
                }
                
                if (enemyBase.isRestraint)
                { 
                    StartCoroutine(RestrainEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                { 
                    StartCoroutine(SlowEffect(enemyBase));
                }
            }
        }
        private IEnumerator RestrainEffect(EnemyBase enemyBase)
        {
            var restrainColor = new Color(1, 0.5f, 0);
            var originColor = new Color(1, 1, 1);
            var restrainTime = EnforceManager.Instance.firePoisonAdditionalStun && Chance(20) && enemyBase.isBleed ? 1f : 1f + 1f * EnforceManager.Instance.IncreaseRestraintTime();
            var originalSpeed = enemyBase.moveSpeed;
            
            if (!_alreadyRestrain.TryGetValue(enemyBase, out var isAlreadyRestraint))
            {
                isAlreadyRestraint = false;
                _alreadyRestrain[enemyBase] = false;
            }

            if  (isAlreadyRestraint) yield break;
            _alreadyRestrain[enemyBase] = true;
            enemyBase.GetComponent<SpriteRenderer>().color = restrainColor;
            enemyBase.moveSpeed = 0;
            yield return new WaitForSeconds(restrainTime);
            enemyBase.moveSpeed = originalSpeed;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
            _alreadyRestrain[enemyBase] = false;
            enemyBase.isRestraint = false;
        }
        private IEnumerator SlowEffect(EnemyBase enemyBase)
        {
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            var slowTime = EnforceManager.Instance.water2BleedAdditionalRestraint && Chance(20) && enemyBase.isBleed ? 1f : 2f + 1f * EnforceManager.Instance.water2IncreaseSlowTime;
            var moveSpeedMultiplier = EnforceManager.Instance.waterIncreaseSlowPower ? 0.4f : 0.6f;
            var originalSpeed = enemyBase.moveSpeed;

            if (!_alreadySlow.TryGetValue(enemyBase, out var isAlreadySlow))
            {
                isAlreadySlow = false;
                _alreadySlow[enemyBase] = false;
            }

            if (isAlreadySlow) yield break;
            _alreadySlow[enemyBase] = true;
            enemyBase.GetComponent<SpriteRenderer>().color = slowColor;
            enemyBase.moveSpeed *= moveSpeedMultiplier;
            yield return new WaitForSeconds(slowTime);
            enemyBase.moveSpeed = originalSpeed;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
            _alreadySlow[enemyBase] = false;
            enemyBase.isSlow = false;
        }
    }
}
