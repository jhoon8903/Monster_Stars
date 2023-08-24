using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.RewardScript;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

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
        private readonly List<GameObject> _freezeEffectPool = new List<GameObject>();
        private Rigidbody2D _rb;
        private float _endY;
        private int _slowCount;
        private float _speedReductionFactor;
        public bool globalFreeze;
        public static EnemyPatternManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
           _endY = castle.GetComponent<BoxCollider2D>().transform.position.y;
        }
        public IEnumerator Zone_Move(EnemyBase enemyBase, EnemyBase.SpawnZones spawnZone)
        {
            enemyBase.gameObject.SetActive(true);
            enemyBase.Initialize();
            enemyBase.SpawnZone = spawnZone;
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

        private static IEnumerator WalkingEffect(Transform childTransform)
        {
            var originalScale = childTransform.localScale;
            var originalLocalPosition = childTransform.localPosition;
            const float verticalScaleAmount = 0.07f;
            const float horizontalScaleAmount = 0.05f;
            const float positionOffset = 0.15f;
            const float tweenDuration = 0.5f; // 애니메이션의 지속 시간을 조절합니다.
            var targetScale = new Vector3(originalScale.x + horizontalScaleAmount, originalScale.y - verticalScaleAmount, originalScale.z);
            var targetPosition = new Vector3(originalLocalPosition.x, originalLocalPosition.y - positionOffset, originalLocalPosition.z);
            while (true)
            {
                childTransform.DOLocalMove(targetPosition, tweenDuration).SetEase(Ease.InOutSine).OnComplete(() => 
                {
                    (targetPosition, originalLocalPosition) = (originalLocalPosition, targetPosition);
                });

                childTransform.DOScale(targetScale, tweenDuration).SetEase(Ease.InOutSine).OnComplete(() => 
                {
                    (targetScale, originalScale) = (originalScale, targetScale);
                });
                yield return new WaitForSeconds(tweenDuration);
            }
        }

        private IEnumerator Rain(EnemyBase enemyBase)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            var targetPosition = new Vector2(_enemyRigidbodies[enemyBase].transform.position.x, _endY);
            var childTransform = enemyBase.transform.Find("Sprite");
            StartCoroutine(WalkingEffect(childTransform));
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                if (enemyBase.isDead)
                {
                    StopCoroutine(WalkingEffect(childTransform));
                    yield break;
                }
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
                if (enemyBase.gameObject.activeInHierarchy)
                {
                    StartCoroutine(Effect(enemyBase));
                }
                else
                {
                    StopCoroutine(Effect(enemyBase));
                }
            }
        }
        private IEnumerator Diagonal(EnemyBase enemyBase)
        {
            _rb = enemyBase.GetComponent<Rigidbody2D>();
            _enemyRigidbodies[enemyBase] = _rb;
            var endX = enemyBase.SpawnZone == EnemyBase.SpawnZones.B ? Random.Range(4, 7) : Random.Range(-1, 2);
            var targetPosition = new Vector2(endX, _endY);
            var childTransform = enemyBase.transform.Find("Sprite");
            StartCoroutine(WalkingEffect(childTransform));
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                if (enemyBase.isDead)
                {
                    StopCoroutine(WalkingEffect(childTransform));
                    yield break;
                }
                _slowCount = EnforceManager.Instance.slowCount;
                _speedReductionFactor = 1f - _slowCount * 0.15f;
                if (_speedReductionFactor == 0)
                {
                    _speedReductionFactor = 1f;
                }
                _moveSpeed = enemyBase.moveSpeed * _speedReductionFactor * moveSpeedOffset * Time.deltaTime;
                _enemyRigidbodies[enemyBase].transform.position = Vector2.MoveTowards(_enemyRigidbodies[enemyBase].transform.position, targetPosition, _moveSpeed);
                if (enemyBase.gameObject.activeInHierarchy)
                {
                    StartCoroutine(Effect(enemyBase));
                }
                else
                {
                    StopCoroutine(Effect(enemyBase));
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
            var childTransform = enemyBase.transform.Find("Sprite");
            StartCoroutine(WalkingEffect(childTransform));
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                if (enemyBase.isDead)
                {
                    StopCoroutine(WalkingEffect(childTransform));
                    yield break;
                }
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
                if (enemyBase.gameObject.activeInHierarchy)
                {
                    StartCoroutine(Effect(enemyBase));
                }
                else
                {
                    StopCoroutine(Effect(enemyBase));
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
            var childTransform = enemyBase.transform.Find("Sprite");
            StartCoroutine(WalkingEffect(childTransform));
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                if (enemyBase.isDead)
                {
                    StopCoroutine(WalkingEffect(childTransform));
                    yield break;
                }
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
                if (enemyBase.gameObject.activeInHierarchy)
                {
                    StartCoroutine(Effect(enemyBase));
                }
                else
                {
                    StopCoroutine(Effect(enemyBase));
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
            var childTransform = enemyBase.transform.Find("Sprite");
            StartCoroutine(WalkingEffect(childTransform));
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                var journeyLength = Vector2.Distance(_enemyRigidbodies[enemyBase].transform.position, targetPosition);
                if (enemyBase.isDead)
                {
                    StopCoroutine(WalkingEffect(childTransform));
                    yield break;
                }
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
                if (enemyBase.gameObject.activeInHierarchy)
                {
                    StartCoroutine(Effect(enemyBase));
                }
                else
                {
                    StopCoroutine(Effect(enemyBase));
                }
            }
        }
        private IEnumerator Effect(EnemyBase enemyBase)
        {
            if (enemyBase.isBind)
            {
                StartCoroutine(BindEffect(enemyBase, enemyBase.Character));
            }
            if (enemyBase.isSlow)
            {
                StartCoroutine(SlowEffect(enemyBase, enemyBase.Character));
            }
            if (enemyBase.isKnockBack)
            {
                StartCoroutine(KnockBackEffect(enemyBase, enemyBase.Character));
            }
            if (enemyBase.isFreeze)
            {
                StartCoroutine(FreezeEffect(enemyBase, enemyBase.Character));
            }
            if (EnforceManager.Instance.ogreStatusAilmentSlowEffect)
            {
                if (enemyBase.isBind|| enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn || enemyBase.isPoison || enemyBase.isFreeze || enemyBase.isStun)
                {
                    StartCoroutine(SlowEffect(enemyBase, enemyBase.Character));
                }
            }
            if (enemyBase.isStun)
            {
                StartCoroutine(StunEffect(enemyBase, enemyBase.Character));
            }
            yield return null;
        }
        private static IEnumerator BindEffect(EnemyBase enemyBase, CharacterBase characterBase)
        {
            var restrainColor = new Color(1, 0.5f, 0);
            var originColor = new Color(1, 1, 1);
            var restrainTime = characterBase.bindTime;
            if (!enemyBase.AlreadyBind.TryGetValue(enemyBase, out var isAlreadyBind))
            {
                isAlreadyBind = false;
                enemyBase.AlreadyBind[enemyBase] = false;
            }
            if  (isAlreadyBind) yield break;
            enemyBase.AlreadyBind[enemyBase] = true;
            enemyBase.GetComponentInChildren<SpriteRenderer>().color = restrainColor;
            enemyBase.moveSpeed = 0;
            yield return new WaitForSeconds(restrainTime);
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.GetComponentInChildren<SpriteRenderer>().color = originColor;
            enemyBase.AlreadyBind[enemyBase] = false;
            enemyBase.BindStatus(false, characterBase);
            if (!enemyBase.IsBind.ContainsKey(characterBase)) yield break;
            enemyBase.statusList.Remove(enemyBase.IsBind[characterBase]);
        }
        private static IEnumerator SlowEffect(EnemyBase enemyBase, CharacterBase characterBase)
        {
            var slowColor = new Color(0f, 0.74f, 1);
            var originColor = new Color(1, 1, 1);
            var slowTime = characterBase.slowTime;
            var slowPower = characterBase.slowPower;
            if (!enemyBase.AlreadySlow.TryGetValue(enemyBase, out var isAlreadySlow))
            {
                isAlreadySlow = false;
                enemyBase.AlreadySlow[enemyBase] = false;
            }
            if (isAlreadySlow) yield break;
            enemyBase.AlreadySlow[enemyBase] = true;
            enemyBase.GetComponentInChildren<SpriteRenderer>().color = slowColor;
            enemyBase.moveSpeed *= slowPower;
            yield return new WaitForSeconds(slowTime);
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.GetComponentInChildren<SpriteRenderer>().color = originColor;
            enemyBase.AlreadySlow[enemyBase] = false;
            enemyBase.SlowStatus(false, characterBase);
            if (!enemyBase.IsSlow.ContainsKey(characterBase)) yield break;
            enemyBase.statusList.Remove(enemyBase.IsSlow[characterBase]);
        }
        private static IEnumerator KnockBackEffect(EnemyBase enemyBase, CharacterBase characterBase)
        {
            if (!enemyBase.AlreadyKnockBack.TryGetValue(enemyBase, out var isAlreadyKnockBack))
            {
                isAlreadyKnockBack = false;
                enemyBase.AlreadyKnockBack[enemyBase] = false;
            }
            if (isAlreadyKnockBack) yield break;
            enemyBase.AlreadyKnockBack[enemyBase] = true;
            var knockBackDirection = Vector2.up; 
            var knockBackForce = characterBase.knockBackPower;
            var knockBackTime = characterBase.knockBackTime;
            var rb = enemyBase.GetComponent<Rigidbody2D>();
            rb.AddForce(knockBackDirection * knockBackForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(knockBackTime);
            while (knockBackForce > 0.1)
            {
                knockBackForce -= Time.deltaTime;
                rb.velocity = knockBackDirection * knockBackForce;
                yield return null;
            }
            rb.velocity = Vector2.zero;
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.AlreadyKnockBack[enemyBase] = false;
            enemyBase.KnockBackStatus(false, characterBase);
            if (!enemyBase.IsKnockBack.ContainsKey(characterBase)) yield break;
            enemyBase.statusList.Remove(enemyBase.IsKnockBack[characterBase]);
        }
        private IEnumerator FreezeEffect(EnemyBase enemyBase, CharacterBase characterBase)
        {
            var freezeTime = characterBase.freezeTime;
            if (!enemyBase.AlreadyFreeze.TryGetValue(enemyBase, out var isAlreadyFreeze))
            {
                isAlreadyFreeze = false;
            }
            if  (isAlreadyFreeze) yield break;
            enemyBase.AlreadyFreeze[enemyBase] = true;
            var freeze = GetFreezeEffectFromPool();
            freeze.transform.position = enemyBase.transform.position;
            freeze.SetActive(true);
            enemyBase.moveSpeed = 0;
    
            float timer = 0;
            while (timer < freezeTime && !enemyBase.isDead)
            {
                timer += Time.deltaTime;
                yield return null; 
            }
            freeze.SetActive(false);
            _freezeEffectPool.Add(freeze);
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.AlreadyFreeze[enemyBase] = false;
            enemyBase.FreezeStatus(false, characterBase);
            if (!enemyBase.IsFreeze.ContainsKey(characterBase)) yield break;
            enemyBase.statusList.Remove(enemyBase.IsFreeze[characterBase]);
        }
        private GameObject GetFreezeEffectFromPool()
        {
            GameObject freeze;
            if (_freezeEffectPool.Count > 0)
            {
                freeze = _freezeEffectPool[^1];
                _freezeEffectPool.RemoveAt(_freezeEffectPool.Count - 1);
            }
            else
            {
                freeze = Instantiate(freezeEffect, transform, true);
            }
            return freeze;
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
                StartCoroutine(FreezeEffect(enemyBase, enemyBase.Character));
            }
            yield return null;
        }
        private IEnumerator BlizzardEffect()
        {
            blizzardEffect.SetActive(true);
            yield return new WaitForSeconds(1f);
            blizzardEffect.SetActive(false);
            globalFreeze = false;
        }
        private IEnumerator StunEffect(EnemyBase enemyBase, CharacterBase characterBase)
        {
            var stunColor = new Color(1f, 0.3f, 0);
            var originColor = enemyBase.GetComponentInChildren<SpriteRenderer>();
            var stunTime = characterBase.stunTime;
            if (!enemyBase.AlreadyStun.TryGetValue(enemyBase, out var isAlreadyStun))
            {
                isAlreadyStun = false;
                enemyBase.AlreadyStun[enemyBase] = false;
            }
            if (isAlreadyStun) yield break;
            enemyBase.AlreadyStun[enemyBase] = true;
            enemyBase.moveSpeed = 0;
            StartCoroutine(enemyBase.FlickerEffect(originColor, stunColor));
            yield return new WaitForSeconds(stunTime);
            StopCoroutine(enemyBase.FlickerEffect(originColor, stunColor));
            enemyBase.moveSpeed = enemyBase.originSpeed;
            enemyBase.AlreadyStun[enemyBase] = false;
            enemyBase.StunStatus(false, characterBase);
            if (!enemyBase.IsStun.ContainsKey(characterBase)) yield break;
            enemyBase.statusList.Remove(enemyBase.IsStun[characterBase]);
        }
    }
}
