using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.QuestGroup;
using Script.RewardScript;
using Script.UIManager;
using Script.WeaponScriptGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.EnemyManagerScript
{
    public class EnemyBase : MonoBehaviour
    {
        [SerializeField] private GameObject damageText;
        [SerializeField] private GameObject poisonAreaObject;
        private readonly List<GameObject> _damagePopupList = new List<GameObject>();
        public CharacterBase Character { get; private set; }
        private EnemyPool _enemyPool;
        private Slider _hpSlider;
        private bool _updateSlider;
        protected internal float CrushDamage; 
        public float moveSpeed;
        public float originSpeed;
        public enum EnemyTypes { Boss , Normal}
        protected internal EnemyTypes EnemyType;
        public enum RegistryTypes { Physics, Poison, Burn, Water, Darkness, None }
        protected internal RegistryTypes RegistryType; 
       
        private static readonly object Lock = new object();
        public enum KillReasons { ByPlayer }
        public float healthPoint;
        public float maxHealthPoint;
        public float currentHealth;
        public enum SpawnZones { A, B, C, D, E, F, None}
        protected internal SpawnZones SpawnZone;
        public string enemyDesc;

        public enum EnemyClasses
        {
           Farmer, SwordMan, Magician, Archer, Dryad, WarDancer, Marauders, Scout, Pirate, Militia, SpearMan, Pilgrim,
           Engineer, Miner, Hammerer, Quarreller, RoyalGuard, Druid, Wizard, Shaman, RuneSmith, Berserker
        }
        public EnemyClasses enemyClass;
        private GameObject _damagePopup;
        public bool isDead;
        public List<CharacterBase.UnitGroups> statusList = new List<CharacterBase.UnitGroups>();
        private bool _pooling;
        private readonly List<GameObject> _poisonPool = new List<GameObject>();
        // 속박 (Bind)
        public readonly Dictionary<EnemyBase, bool> AlreadyBind = new Dictionary<EnemyBase, bool>();
        public readonly Dictionary<CharacterBase, CharacterBase.UnitGroups> IsBind = new Dictionary<CharacterBase, CharacterBase.UnitGroups>();
        public bool isBind;
        public void BindStatus(bool value, CharacterBase characterBase = null)
        {
            isBind = value;
            if (!value) return;
            if (characterBase == null) return;
            IsBind[characterBase] = characterBase.unitGroup;
            Character = characterBase;
            statusList.Add(IsBind[characterBase]);
        }
        // 둔화 (Slow)
        public readonly Dictionary<EnemyBase, bool> AlreadySlow = new Dictionary<EnemyBase, bool>();
        public readonly Dictionary<CharacterBase, CharacterBase.UnitGroups> IsSlow = new Dictionary<CharacterBase, CharacterBase.UnitGroups>();
        public bool isSlow;
        public void SlowStatus(bool value, CharacterBase characterBase = null)
        {
            isSlow = value;
            if (!value) return;
            if (characterBase == null) return;
            IsSlow[characterBase] = characterBase.unitGroup;
            Character = characterBase;
            statusList.Add(IsSlow[characterBase]);
        }
        // 밀침 (KnockBack)
        public readonly Dictionary<EnemyBase, bool> AlreadyKnockBack = new Dictionary<EnemyBase, bool>();
        public readonly Dictionary<CharacterBase, CharacterBase.UnitGroups> IsKnockBack = new Dictionary<CharacterBase, CharacterBase.UnitGroups>();
        public bool isKnockBack;
        public void KnockBackStatus(bool value, CharacterBase characterBase = null)
        {
            isKnockBack = value;
            if (!value) return;
            if (characterBase == null) return;
            IsKnockBack[characterBase] = characterBase.unitGroup;
            Character = characterBase;
            statusList.Add(IsKnockBack[characterBase]);
        }
        // 빙결 (Freeze)
        public readonly Dictionary<EnemyBase, bool> AlreadyFreeze = new Dictionary<EnemyBase, bool>();
        public readonly Dictionary<CharacterBase, CharacterBase.UnitGroups> IsFreeze = new Dictionary<CharacterBase, CharacterBase.UnitGroups>();
        public bool isFreeze;
        public void FreezeStatus(bool value, CharacterBase characterBase = null)
        {
            isFreeze = value;
            if (!value) return;
            if (characterBase == null) return;
            IsFreeze[characterBase] = characterBase.unitGroup;
            Character = characterBase;
            statusList.Add(IsFreeze[characterBase]);
        }
        // 기절 (Stun)
        public readonly Dictionary<EnemyBase, bool> AlreadyStun = new Dictionary<EnemyBase, bool>();
        public readonly Dictionary<CharacterBase, CharacterBase.UnitGroups> IsStun = new Dictionary<CharacterBase, CharacterBase.UnitGroups>();
        public bool isStun;
        public void StunStatus(bool value, CharacterBase characterBase = null)
        {
            isStun = value;
            if (!value) return;
            if (characterBase == null) return;
            IsStun[characterBase] = characterBase.unitGroup;
            Character = characterBase;
            statusList.Add(IsStun[characterBase]);
        }
        // 중독 (Poison)
        public readonly Dictionary<EnemyBase, bool> AlreadyPoison = new Dictionary<EnemyBase, bool>();
        public readonly Dictionary<CharacterBase, CharacterBase.UnitGroups> IsPoison = new Dictionary<CharacterBase, CharacterBase.UnitGroups>();
        public bool isPoison;
        public void PoisonStatus(bool value, CharacterBase characterBase = null)
        {
            isPoison = value;
            if (!value) return;
            if (characterBase == null) return;
            IsPoison[characterBase] = characterBase.unitGroup;
            Character = characterBase;
            statusList.Add(IsPoison[characterBase]);
            var poisonAttack = WeaponBase.Instance.PoisonEffect(this, characterBase);
            var poisonColor = new Color(0.18f, 1f, 0.1f);
            var originColor = gameObject.GetComponentInChildren<SpriteRenderer>();
            if (!gameObject.activeInHierarchy || !isPoison) return;
            StartCoroutine(poisonAttack);
            if (!gameObject.activeInHierarchy || !isPoison) return;
            StartCoroutine(FlickerEffect(originColor, poisonColor));
        }
        // 화상 (Burn)
        public readonly Dictionary<EnemyBase, bool> AlreadyBurn = new Dictionary<EnemyBase, bool>();
        public readonly Dictionary<CharacterBase, CharacterBase.UnitGroups> IsBurn = new Dictionary<CharacterBase, CharacterBase.UnitGroups>();
        public bool isBurn;
        public void BurnStatus(bool value, CharacterBase characterBase = null)
        {
            isBurn = value;
            if (!value) return;
            if (characterBase == null) return;
            IsBurn[characterBase] = characterBase.unitGroup;
            Character = characterBase;
            statusList.Add(IsBurn[characterBase]);
            var burningAttack = WeaponBase.Instance.BurningEffect(this, characterBase);
            var burningColor = new Color(1f, 0, 0.4f);
            var originColor = gameObject.GetComponentInChildren<SpriteRenderer>();
            if (!gameObject.activeInHierarchy || !isBurn) return;
            StartCoroutine(burningAttack);
            if (!gameObject.activeInHierarchy || !isBurn) return;
            StartCoroutine(FlickerEffect(originColor, burningColor));
        }
        // 출혈 (Bleed)
        public readonly Dictionary<EnemyBase, bool> AlreadyBleed = new Dictionary<EnemyBase, bool>();
        public readonly Dictionary<CharacterBase, CharacterBase.UnitGroups> IsBleed = new Dictionary<CharacterBase, CharacterBase.UnitGroups>();
        public bool isBleed;
        public void BleedStatus(bool value, CharacterBase characterBase = null)
        {
            isBleed = value;
            if (!value) return;
            if (characterBase == null) return;
            IsBleed[characterBase] = characterBase.unitGroup;
            Character = characterBase;
            statusList.Add(IsBleed[characterBase]);
            var bleedColor = new Color(1f, 0, 0.4f);
            var originColor = gameObject.GetComponentInChildren<SpriteRenderer>();
            var bleedAttack = WeaponBase.Instance.BleedEffect(this, characterBase);
            if (!gameObject.activeInHierarchy || !isBleed) return;
            StartCoroutine(bleedAttack);
            if (!gameObject.activeInHierarchy || !isBleed) return;
            StartCoroutine(FlickerEffect(originColor, bleedColor));
        }
        public virtual void Initialize()
        {
            SpawnZone = SpawnZones.A;
            _hpSlider = GetComponentInChildren<Slider>(true);
            if (!_pooling)
            {
                for (var i = 0; i < 10; i++)
                {
                    _damagePopup = Instantiate(damageText, gameObject.transform, false);
                    _damagePopupList.Add(_damagePopup);
                    _damagePopup.SetActive(false);
                    _pooling = true;
                }
            }
            if (EnemyType != EnemyTypes.Boss)
            {
                _hpSlider.gameObject.SetActive(false);
            }
            LoadEnemyHealthData();
            maxHealthPoint = healthPoint;
            currentHealth = maxHealthPoint;
            _hpSlider.maxValue = maxHealthPoint;
            _hpSlider.value = currentHealth;
            StartCoroutine(UpdateHpSlider());
            moveSpeed = originSpeed;
        }
        private void LoadEnemyHealthData()
        {
            var enemyHPs = Resources.Load<TextAsset>("EnemyHpData");
            var data = enemyHPs.text.Split(new[] { '\n' });
            if (StageManager.Instance == null) return;
            var stageData = data[StageManager.Instance.latestStage].Split(",");
            var healthPointData = stageData[1].Split(" ");
            var baseHealthPoint = int.Parse(healthPointData[StageManager.Instance.currentWave - 1]);
            healthPoint = baseHealthPoint;
        }
        private IEnumerator DamageTextPopup(int damage)
        {
            if (!gameObject.activeInHierarchy) yield break;

            foreach (var popup in _damagePopupList)
            {
                if (popup.activeInHierarchy) continue;
                var pos = gameObject.transform.position;
                popup.transform.position = new Vector3(pos.x,pos.y + 0.5f,0f);
                if (EnemyType == EnemyTypes.Boss)
                {
                    popup.transform.position = new Vector3(pos.x,pos.y + 1.7f,0f);
                    popup.transform.localScale = new Vector3(3,3,0);
                }
                Vector2 startPosition = popup.transform.position;
                var endPosition = new Vector2(startPosition.x, startPosition.y + 0.2f);
                if (damage == 0) continue;
                popup.SetActive(true);
                popup.GetComponent<TextMeshPro>().text = damage.ToString();

                float t = 0;
                const float speed = 1f;
                while (t < 1)
                {
                    t += Time.deltaTime * speed;
                    popup.transform.position = Vector2.Lerp(startPosition, endPosition, t);
                    yield return null;
                }
                yield return new WaitForSeconds(0.1f); // Adjust this time as needed
                popup.SetActive(false);
            }
        }
        private readonly Dictionary<CharacterBase.UnitGroups, int> _cumulativeDamageByGroup = new Dictionary<CharacterBase.UnitGroups, int>();
        public void ReceiveDamage(EnemyBase detectEnemy, float damage, CharacterBase atkUnit, KillReasons reason = KillReasons.ByPlayer)
        {
            lock (Lock)
            {
                var receiveDamage = (int)damage;
                if (_cumulativeDamageByGroup.ContainsKey(atkUnit.unitGroup))
                    _cumulativeDamageByGroup[atkUnit.unitGroup] += receiveDamage;
                else
                    _cumulativeDamageByGroup[atkUnit.unitGroup] = receiveDamage;
                PlayerPrefs.SetInt($"{atkUnit.unitGroup}DPS", _cumulativeDamageByGroup[atkUnit.unitGroup]);
                PlayerPrefs.Save();
                if (isDead) return;
                currentHealth -= receiveDamage;
                if (!gameObject.activeInHierarchy) return;
                StartCoroutine(DamageTextPopup(receiveDamage));
                _updateSlider = true;
                if (currentHealth > 0f || isDead) return;
                isDead = true;
                ExpManager.Instance.HandleEnemyKilled(reason);
                if (EnforceManager.Instance.octopusShackledExplosion && atkUnit.unitGroup == CharacterBase.UnitGroups.Octopus)
                {
                    StartCoroutine(ExplosionDamage(detectEnemy, damage, atkUnit));
                }
                else if (EnforceManager.Instance.beholderBurnedEnemyExplosion && atkUnit.unitGroup == CharacterBase.UnitGroups.Beholder)
                {
                    StartCoroutine(ExplosionDamage(detectEnemy, damage, atkUnit));
                }

                if (EnforceManager.Instance.cobraSpawnPoisonArea && detectEnemy.isPoison)
                {
                    StartCoroutine(PoisonArea(detectEnemy, damage, atkUnit));
                }

                if (EnforceManager.Instance.darkElfExplosionBoost && atkUnit.unitGroup == CharacterBase.UnitGroups.DarkElf)
                {
                    if (detectEnemy.isBind || detectEnemy.isPoison || detectEnemy.isBleed || detectEnemy.isBurn ||
                        detectEnemy.isFreeze || detectEnemy.isSlow || detectEnemy.isStun || detectEnemy.isKnockBack)
                    {
                        StartCoroutine(ExplosionDamage(detectEnemy, damage, atkUnit));
                    }
                }
                EnemyKilledEvents(detectEnemy, atkUnit);
            }
        }
        private IEnumerator PoisonArea(Component detectEnemy, float damage, CharacterBase atkUnit)
        {
            var areaDuration = atkUnit.poisonAreaTime;
            const float areaRadius = 0.5f;
            var poisonPool = GetPoisonPool();
            poisonPool.transform.position = detectEnemy.transform.position;
            poisonPool.SetActive(true);
            var timer = 0;
            while (timer < areaDuration)
            {
                var poolArea = Physics2D.OverlapCircleAll(poisonPool.transform.position, areaRadius,
                    1 << LayerMask.NameToLayer("Enemy"));
                foreach (var enemy in poolArea)
                {
                    var enemyBase = enemy.GetComponent<EnemyBase>();
                    if (enemyBase != null && !enemyBase.isDead)
                    {
                        enemyBase.ReceiveDamage(enemyBase, damage*2f, atkUnit);
                    }
                }
                timer += 1;
                yield return new WaitForSeconds(1f);
            }
            poisonPool.SetActive(false);
            _poisonPool.Add(poisonPool);
        }
        private GameObject GetPoisonPool()
        {
            GameObject poisonPool;
            if (_poisonPool.Count > 0)
            {
                poisonPool = _poisonPool[^1];
                _poisonPool.RemoveAt(_poisonPool.Count -1);
            }
            else
            {
                poisonPool = Instantiate(poisonAreaObject, transform, true);
            }

            return poisonPool;
        }
        private IEnumerator ExplosionDamage(EnemyBase detectEnemy, float damage, CharacterBase atkUnit)
        {
            if (atkUnit.unitGroup is CharacterBase.UnitGroups.Beholder)
            {
                damage *= 2f;
            }
            var enemyPosition = detectEnemy.transform.position;
            var explosionSize = atkUnit.unitGroup == CharacterBase.UnitGroups.DarkElf ? 2f : 1f;
            var colliders = Physics2D.OverlapCircleAll(enemyPosition, explosionSize);
            foreach (var enemyObject in colliders)
            {
                if (!enemyObject.gameObject.CompareTag("Enemy") || !enemyObject.gameObject.activeInHierarchy) continue;
                var nearEnemy = enemyObject.GetComponent<EnemyBase>();
                var explosionDamage = GetComponent<WeaponBase>().DamageCalculator(damage, detectEnemy, atkUnit);
                ReceiveDamage(nearEnemy, (int)explosionDamage, atkUnit);
            }
            yield return null;
        }
        public void EnemyKilledEvents(EnemyBase detectedEnemy, CharacterBase characterBase = null)
        {
            detectedEnemy.StopAllCoroutines();
            foreach (var popup in detectedEnemy._damagePopupList)
            {
                popup.SetActive(false);
            }
            var enemyPool = FindObjectOfType<EnemyPool>();
            if (StageManager.Instance != null) StageManager.Instance.EnemyDestroyEvent(detectedEnemy);
            detectedEnemy.isDead = false;
            detectedEnemy.statusList.Clear();
            detectedEnemy.IsBind.Clear();
            detectedEnemy.AlreadyBind.Clear();
            detectedEnemy.IsSlow.Clear();
            detectedEnemy.AlreadySlow.Clear();
            detectedEnemy.IsKnockBack.Clear();
            detectedEnemy.AlreadyKnockBack.Clear();
            detectedEnemy.IsFreeze.Clear();
            detectedEnemy.AlreadyFreeze.Clear();
            detectedEnemy.IsStun.Clear();
            detectedEnemy.AlreadyStun.Clear();
            detectedEnemy.IsPoison.Clear();
            detectedEnemy.AlreadyPoison.Clear();
            detectedEnemy.IsBurn.Clear();
            detectedEnemy.AlreadyBurn.Clear();
            detectedEnemy.IsBleed.Clear();
            detectedEnemy.AlreadyBleed.Clear();
            detectedEnemy.transform.localScale = Vector3.one;
            detectedEnemy.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            detectedEnemy.BindStatus(false, characterBase);
            detectedEnemy.SlowStatus(false, characterBase);
            detectedEnemy.KnockBackStatus(false, characterBase);
            detectedEnemy.FreezeStatus(false, characterBase);
            detectedEnemy.StunStatus(false, characterBase);
            detectedEnemy.PoisonStatus(false, characterBase);
            detectedEnemy.BurnStatus(false, characterBase);
            detectedEnemy.BleedStatus(false, characterBase);
            if (characterBase != null)
            {
                characterBase.DetectEnemies().Remove(detectedEnemy.gameObject);
                characterBase.AttackCounts.Remove(detectedEnemy);
            }
            Quest.Instance.KillEnemyQuest();
            enemyPool.ReturnToPool(detectedEnemy);
        }
        private IEnumerator UpdateHpSlider()
        {
            while (true)
            {
                if (_updateSlider)
                {
                    yield return _hpSlider.DOValue(currentHealth, 0.1f);
                    _updateSlider = false;
                }
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        public IEnumerator FlickerEffect(SpriteRenderer render, Color targetColor)
        {
            var originalColor = render.color;
            var elapsedTime = 0f;
            while ( isPoison || isBleed || isBurn)
            {
                if (isDead) yield break;
                var value = Mathf.Abs(Mathf.Sin(elapsedTime / 0.5f * Mathf.PI));
                render.color = Color.Lerp(originalColor, targetColor, value);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            render.color = originalColor;
        }
    }
}