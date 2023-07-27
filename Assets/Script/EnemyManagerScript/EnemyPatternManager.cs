using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RewardScript;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script.EnemyManagerScript
{
    public class EnemyPatternManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject castle;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private GameObject blizzardEffect;
        [SerializeField] private GameObject freezeEffect;
        private readonly Dictionary<EnemyBase, Rigidbody2D> _enemyRigidbodies = new Dictionary<EnemyBase, Rigidbody2D>();
        private float _moveSpeed;
        public float moveSpeedOffset;
        private readonly System.Random _random = new System.Random();
        private readonly Dictionary<EnemyBase, bool> _alreadySlow = new Dictionary<EnemyBase, bool>();
        private readonly Dictionary<EnemyBase, bool> _alreadyRestrain = new Dictionary<EnemyBase, bool>();
        private readonly Dictionary<EnemyBase, bool> _alreadyKnockBack = new Dictionary<EnemyBase, bool>();
        private readonly Dictionary<EnemyBase, bool> _alreadyStatusSlow = new Dictionary<EnemyBase, bool>();
        private readonly Dictionary<EnemyBase, bool> _alreadyFreeze = new Dictionary<EnemyBase, bool>();
        private Rigidbody2D _rb;
        private float _endY;
        private int _slowCount;
        private float _speedReductionFactor;
        public bool globalFreeze;


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

                StartCoroutine(Effect(enemyBase));
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
                
                StartCoroutine(Effect(enemyBase));
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

                StartCoroutine(Effect(enemyBase));
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

                StartCoroutine(Effect(enemyBase));
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
                StartCoroutine(Effect(enemyBase));
            }
        }

        private IEnumerator Effect(EnemyBase enemyBase)
        {
            if (enemyBase.isBind)
            {
                StartCoroutine(BindEffect(enemyBase));
            }
            if (enemyBase.isSlowC || enemyBase.isSlowE)
            { 
                StartCoroutine(SlowEffect(enemyBase));
            }
            if ( enemyBase.isKnockBack )
            {
                StartCoroutine(KnockBackEffect(enemyBase));
            }
            if (enemyBase.isFreeze || enemyBase.isFreezeE)
            {
                StartCoroutine(FreezeEffect(enemyBase));
            }
            if (EnforceManager.Instance.darkStatusAilmentSlowEffect)
            {
                if (enemyBase.isBind || enemyBase.isSlowC || enemyBase.isSlowE || enemyBase.isBleed || enemyBase.isBurnG || enemyBase.isPoison || enemyBase.isFreeze)
                {
                    StartCoroutine(StatusSlowEffect(enemyBase));
                }
            }
            yield return null;
        }
        private IEnumerator BindEffect(EnemyBase enemyBase)
        {
            var restrainColor = new Color(1, 0.5f, 0);
            var originColor = new Color(1, 1, 1);
            float restrainTime;
            
            if (enemyBase.isBurningPoison)
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
        }
        private IEnumerator SlowEffect(EnemyBase enemyBase)
        {
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            var slowTime = 0f;
            var slowPower = 0f;
            if (enemyBase.isSlowC)
            {
                slowTime = 1f + EnforceManager.Instance.waterSlowDurationBoost;
                slowPower = EnforceManager.Instance.waterSlowCPowerBoost ? 0.55f : 0.7f;
            }
            else if (enemyBase.isSlowE)
            {
                slowTime = 1.5f + EnforceManager.Instance.water2SlowTimeBoost;
                slowPower = EnforceManager.Instance.water2SlowPowerBoost ? 0.6f : 0.7f;
            }
            if (!_alreadySlow.TryGetValue(enemyBase, out var isAlreadySlow))
            {
                isAlreadySlow = false;
                _alreadySlow[enemyBase] = false;
            }
            if (isAlreadySlow) yield break;
            _alreadySlow[enemyBase] = true;
            enemyBase.GetComponent<SpriteRenderer>().color = slowColor;
            enemyBase.moveSpeed *= slowPower;
            yield return new WaitForSeconds(slowTime);
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.GetComponent<SpriteRenderer>().color = originColor;
            _alreadySlow[enemyBase] = false;
            if (enemyBase.isSlowC)
            {
                enemyBase.isSlowC = false;
            }
            else if (enemyBase.isSlowE)
            {
                enemyBase.isSlowE = false;
            }
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
            var knockBackForce = 1f;
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
        public IEnumerator GlobalFreezeEffect()
        {
            if (globalFreeze) yield break;
            globalFreeze = true;
            StartCoroutine(BlizzardEffect());
            AtkManager.Instance.groupCAtkCount = 0;
            var enemyList = enemyPool.enemyBases;
            foreach (var enemyBase in enemyList.Select(enemy => enemy.GetComponent<EnemyBase>()))
            {
                StartCoroutine(FreezeEffect(enemyBase));
            }
            yield return null;
        }

        private IEnumerator FreezeEffect(EnemyBase enemyBase)
        {
            var freezeTime = 1f;
            if (enemyBase.isFreezeE && EnforceManager.Instance.water2FreezeTimeBoost)
            {
                freezeTime = 1.5f;
            }
            if (!_alreadyFreeze.TryGetValue(enemyBase, out var isAlreadyFreeze))
            {
                isAlreadyFreeze = false;
                _alreadyFreeze[enemyBase] = false;
            }
            if  (isAlreadyFreeze) yield break;
            _alreadyRestrain[enemyBase] = true;
            var freeze =  Instantiate(freezeEffect, transform, true);
            freeze.transform.position = enemyBase.transform.position;
            freeze.SetActive(true);
            enemyBase.moveSpeed = 0;
            yield return new WaitForSeconds(freezeTime);
            Destroy(freeze);
            enemyBase.moveSpeed = enemyBase.originSpeed;
            if (enemyBase.isFreeze)
            {
                enemyBase.isFreeze = false;
            }
            else if (enemyBase.isFreezeE)
            {
                enemyBase.isFreezeE = false;
            }
            _alreadyFreeze[enemyBase] = false;
        }

        private IEnumerator BlizzardEffect()
        {
            blizzardEffect.SetActive(true);
            yield return new WaitForSeconds(1f);
            blizzardEffect.SetActive(false);
            globalFreeze = false;
        }
    }
}
