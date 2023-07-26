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
        [SerializeField] private EnemyPool enemyPool;
        private readonly Dictionary<EnemyBase, Rigidbody2D> _enemyRigidbodies = new Dictionary<EnemyBase, Rigidbody2D>();
        private float _moveSpeed;
        public float moveSpeedOffset;
        private readonly System.Random _random = new System.Random();
        private readonly Dictionary<EnemyBase, bool> _alreadySlow = new Dictionary<EnemyBase, bool>();
        private readonly Dictionary<EnemyBase, bool> _alreadyRestrain = new Dictionary<EnemyBase, bool>();
        public Dictionary<EnemyBase, bool> _alreadyKnockBack = new Dictionary<EnemyBase, bool>();
        private readonly Dictionary<EnemyBase, bool> _alreadyStatusSlow = new Dictionary<EnemyBase, bool>();
        private Rigidbody2D _rb;
        private float _endY;
        private int _slowCount;
        private float _speedReductionFactor;
        public bool globalSlow;

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

            _alreadyKnockBack.TryAdd(enemyBase, false);

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
            _alreadyKnockBack.TryAdd(enemyBase, false);
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                _slowCount = EnforceManager.Instance.slowCount;
                _speedReductionFactor = 1f - _slowCount * 0.15f;
                if (_speedReductionFactor == 0)
                {
                    _speedReductionFactor = 1f;
                }
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;

                if (_rb != null)
                {
                    _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
                }

                if (enemyBase.isBind || enemyBase.isSlowStun || enemyBase.isSlowBleedStun || enemyBase.isBurningPoison)
                { 
                    StartCoroutine(BindEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                { 
                   StartCoroutine(SlowEffect(enemyBase));
                }
                if ( enemyBase.isKnockBack )
                {
                    StartCoroutine(KnockBackEffect(enemyBase));
                }
                if (EnforceManager.Instance.darkStatusAilmentSlowEffect)
                {
                    if (enemyBase.isBind || enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn ||
                        enemyBase.isPoison)
                    {
                        StartCoroutine(StatusSlowEffect(enemyBase));
                    }
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
                _speedReductionFactor = 1f - _slowCount * 0.15f;
                if (_speedReductionFactor == 0)
                {
                    _speedReductionFactor = 1f;
                }
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
                
                if (enemyBase.isBind || enemyBase.isSlowStun || enemyBase.isSlowBleedStun || enemyBase.isBurningPoison)
                { 
                    StartCoroutine(BindEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    StartCoroutine(SlowEffect(enemyBase));
                }
                if ( enemyBase.isKnockBack )
                {
                    StartCoroutine(KnockBackEffect(enemyBase));
                }
                if (EnforceManager.Instance.darkStatusAilmentSlowEffect)
                {
                    if (enemyBase.isBind || enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn ||
                        enemyBase.isPoison)
                    {
                        StartCoroutine(StatusSlowEffect(enemyBase));
                    }
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
                waypoints[i] = new Vector2(_enemyRigidbodies[enemyBase].transform.position.x + direction, _enemyRigidbodies[enemyBase].transform.position.y - (2 * (i + 1)));
                direction *= -1;
            }

            var waypointIndex = 0;

            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var targetPosition = waypoints[waypointIndex];
                var journeyLength = Vector2.Distance(_enemyRigidbodies[enemyBase].transform.position, targetPosition);

                _slowCount = EnforceManager.Instance.slowCount;
                _speedReductionFactor = 1f - _slowCount * 0.15f;
                if (_speedReductionFactor == 0)
                {
                    _speedReductionFactor = 1f;
                }
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;

                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);

                if (journeyLength <= 0.01f)
                {
                    waypointIndex++;
                    if (waypointIndex >= waypoints.Length) break;
                    continue;
                }

                if (enemyBase.isBind || enemyBase.isSlowStun || enemyBase.isSlowBleedStun || enemyBase.isBurningPoison)
                { 
                    StartCoroutine(BindEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                {
                    StartCoroutine(SlowEffect(enemyBase));
                }
                if ( enemyBase.isKnockBack )
                {
                    StartCoroutine(KnockBackEffect(enemyBase));
                }
                if (EnforceManager.Instance.darkStatusAilmentSlowEffect)
                {
                    if (enemyBase.isBind || enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn ||
                        enemyBase.isPoison)
                    {
                        StartCoroutine(StatusSlowEffect(enemyBase));
                    }
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
                _speedReductionFactor = 1f - _slowCount * 0.15f;
                if (_speedReductionFactor == 0)
                {
                    _speedReductionFactor = 1f;
                }
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
               
                if (journeyLength <= 0.01f)
                {
                    targetPosition = new Vector2(targetPosition.x, _endY);
                }

                if (enemyBase.isBind || enemyBase.isSlowStun || enemyBase.isSlowBleedStun || enemyBase.isBurningPoison)
                { 
                    StartCoroutine(BindEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                { 
                    StartCoroutine(SlowEffect(enemyBase));
                }
                if ( enemyBase.isKnockBack )
                {
                    StartCoroutine(KnockBackEffect(enemyBase));
                }
                if (EnforceManager.Instance.darkStatusAilmentSlowEffect)
                {
                    if (enemyBase.isBind || enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn ||
                        enemyBase.isPoison)
                    {
                        StartCoroutine(StatusSlowEffect(enemyBase));
                    }
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
                _speedReductionFactor = 1f - _slowCount * 0.15f;
                if (_speedReductionFactor == 0)
                {
                    _speedReductionFactor = 1f;
                }
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
                
                if (journeyLength <= 0.01f)
                {
                    targetPosition = new Vector3(targetPosition.x, _endY);
                }
                
                if (enemyBase.isBind || enemyBase.isSlowStun || enemyBase.isSlowBleedStun || enemyBase.isBurningPoison)
                { 
                    StartCoroutine(BindEffect(enemyBase));
                }
                if (enemyBase.isSlow)
                { 
                    StartCoroutine(SlowEffect(enemyBase));
                }
                if ( enemyBase.isKnockBack )
                {
                    StartCoroutine(KnockBackEffect(enemyBase));
                }
                if (EnforceManager.Instance.darkStatusAilmentSlowEffect)
                {
                    if (enemyBase.isBind || enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn ||
                        enemyBase.isPoison)
                    {
                        StartCoroutine(StatusSlowEffect(enemyBase));
                    }
                }
            }
        }
        private IEnumerator BindEffect(EnemyBase enemyBase)
        {
            var restrainColor = new Color(1, 0.5f, 0);
            var originColor = new Color(1, 1, 1);
            float restrainTime;
 
            if (enemyBase.isSlowStun)
            {
                restrainTime = 0.1f;
            }
            else if (enemyBase.isSlowBleedStun)
            {
                restrainTime = 0.5f;
            }
            else if (enemyBase.isBurningPoison)
            {
                restrainTime = 0.5f;
            }
            else
            {
                restrainTime = EnforceManager.Instance.divineBindDurationBoost? 1f : 1.5f;
            }

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
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
            _alreadyRestrain[enemyBase] = false;
            enemyBase.isBind = false;
            enemyBase.isSlowStun = false;
        }
        private IEnumerator SlowEffect(EnemyBase enemyBase)
        {
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            var slowTime = 0f;
            var moveSpeedMultiplier = 0f;
            if (enemyBase.isSlowC)
            {
                slowTime = 3f;
                moveSpeedMultiplier = 0.6f;
                enemyBase.isSlowC = false;
            }
            else if (enemyBase.isSlowE)
            {
                slowTime = 1.5f + EnforceManager.Instance.water2DebuffDurationIncrease * 0.1f;
                moveSpeedMultiplier = EnforceManager.Instance.water2DebuffStrengthIncrease ? 0.6f : 0.7f;
                enemyBase.isSlowE = false;
            }
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
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
            _alreadySlow[enemyBase] = false;
            enemyBase.isSlow = false;
        }
        private IEnumerator KnockBackEffect(EnemyBase enemyBase)
        {
            if (!_alreadyKnockBack.TryGetValue(enemyBase, out var isAlreadyKnockBack))
            {
                isAlreadyKnockBack = false;
                _alreadyKnockBack[enemyBase] = false;
            }
            if (isAlreadyKnockBack) yield break;
            _alreadyKnockBack[enemyBase] = true;
            enemyBase.isKnockBack = true;

            var knockBackDirection = Vector2.up; 
            var knockBackForce = 1.3f;
            var rb = enemyBase.GetComponent<Rigidbody2D>();
            rb.AddForce(knockBackDirection * knockBackForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
            while (knockBackForce > 0.1)
            {
                knockBackForce -= Time.deltaTime;
                rb.velocity = knockBackDirection * knockBackForce;
                yield return null;
            }
            rb.velocity = Vector2.zero;
            enemyBase.moveSpeed = enemyBase.originSpeed;
            _alreadyKnockBack[enemyBase] = false;
            enemyBase.isKnockBack = false;
        }
        private IEnumerator StatusSlowEffect(EnemyBase enemyBase)
        {
            const float slowTime = 1f;
            const float moveSpeedMultiplier = 0.8f;
            var slowColor = new Color(0.2311f, 0.2593f, 1f, 1f);
            var originColor = new Color(1f, 1f, 1f);

            if (!_alreadyStatusSlow.TryGetValue(enemyBase, out var isAlreadyStatusSlow))
            {
                isAlreadyStatusSlow = false;
                _alreadyStatusSlow[enemyBase] = false;
            }
            if(isAlreadyStatusSlow) yield break;
            _alreadyStatusSlow[enemyBase] = true;
            enemyBase.GetComponent<SpriteRenderer>().color = slowColor;
            enemyBase.moveSpeed *= moveSpeedMultiplier;
            yield return new WaitForSeconds(slowTime);
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
            _alreadyStatusSlow[enemyBase] = false;
        }
        public IEnumerator GlobalSlowEffect()
        {
            if (globalSlow) yield break;
            globalSlow = true;
            var enemyList = enemyPool.enemyBases;
            foreach (var enemy in enemyList)
            {
                var enemyBase = enemy.GetComponent<EnemyBase>();
                enemyBase.GetComponent<SpriteRenderer>().color = Color.cyan;
                enemyBase.moveSpeed *= 0.8f;
                yield return new WaitForSeconds(1f);
                enemyBase.GetComponent<SpriteRenderer>().color = Color.white;
                enemyBase.moveSpeed = enemyBase.originSpeed;
            }
            globalSlow = false;
        }
    }
}
