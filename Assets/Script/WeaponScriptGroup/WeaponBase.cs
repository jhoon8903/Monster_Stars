using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class WeaponBase : MonoBehaviour
    {
        public bool isInUse;
        protected float Speed { get; set; }
        protected float Damage { get; set; }
        protected float Distance { get; private set; }
        protected CharacterBase CharacterBase { get; private set; }
        protected Color Sprite { get; set; }
        private readonly System.Random _random = new System.Random();
        private EnemyBase _poisonedEnemy;
        protected readonly List<EnemyBase> HitEnemy = new List<EnemyBase>();
        public Vector2 direction;
        protected bool HasHit;
        public static WeaponBase Instance { get; private set; }
        private readonly Dictionary<EnemyBase, int> _burnStacks = new Dictionary<EnemyBase, int>();

        public void Awake()
        {
            Instance = this;
        }

        public void InitializeWeapon(CharacterBase characterBase , GameObject target = null)
        {
            CharacterBase = characterBase;
            Distance = characterBase.defaultAtkDistance;
            Damage = characterBase.DefaultDamage;
            Speed = characterBase.projectileSpeed * 2f;
            Sprite = GetComponent<SpriteRenderer>().color;
            HasHit = false;
        }
        public virtual IEnumerator UseWeapon()
        {
            isInUse = true;
            yield return null;
        }
        public void StopUseWeapon(GameObject weapon)
        {
            isInUse = false;
            HitEnemy.Clear();
            AtkManager.Instance.weaponsList.Remove(weapon);
            WeaponsPool.ReturnToPool(weapon);
        }
        private bool Chance(int percent)
        {
            return _random.Next(100) < percent;
        }
        protected void AtkEffect(EnemyBase enemyObject, CharacterBase characterBase)
        {
            if (enemyObject == null) return;
            switch (characterBase.UnitEffect)
            {
                case CharacterBase.UnitEffects.Bind:
                    switch (characterBase.unitGroup)
                    {
                        case CharacterBase.UnitGroups.A:
                            if (Chance(characterBase.effectChance))
                            {
                                enemyObject.BindStatus(true, characterBase);
                            }
                            break;
                    }
                    break;
                case CharacterBase.UnitEffects.Slow:
                    switch (characterBase.unitGroup)
                    {
                       case CharacterBase.UnitGroups.E:
                           if (EnforceManager.Instance.water2Freeze && Chance(characterBase.effectChance))
                           {
                               enemyObject.FreezeStatus(true, characterBase);
                           }
                           else
                           {
                               enemyObject.SlowStatus(true, characterBase);
                           }
                           break;
                       case CharacterBase.UnitGroups.C:
                           if (EnforceManager.Instance.waterFreeze && Chance(characterBase.effectChance))
                           {
                               enemyObject.FreezeStatus(true, characterBase);
                           }
                           else
                           {
                               enemyObject.SlowStatus(true, characterBase);
                           }
                           break;
                    }
                    break;
                case CharacterBase.UnitEffects.None:
                    switch (characterBase.unitGroup)
                    {
                        case CharacterBase.UnitGroups.B:
                            if (EnforceManager.Instance.darkKnockBackChance && Chance(characterBase.effectChance))
                            {
                                enemyObject.KnockBackStatus(true, characterBase);
                            }
                            break;
                        case CharacterBase.UnitGroups.K:
                            if (EnforceManager.Instance.dark2StatusPoison)
                            {
                                if (enemyObject.statusList.Count == 0)
                                {
                                    enemyObject.PoisonStatus(true, characterBase);
                                }
                            }
                            break;
                    }
                    break;
                case CharacterBase.UnitEffects.Poison:
                    switch (characterBase.unitGroup)
                    {
                        case CharacterBase.UnitGroups.F:
                            if (EnforceManager.Instance.poisonPerHitEffect)
                            {
                                enemyObject.PoisonStatus(true, characterBase);
                            }
                            break;
                        case CharacterBase.UnitGroups.I:
                            enemyObject.PoisonStatus(true, characterBase);
                            if ( EnforceManager.Instance.poison2StunToChance && Chance(characterBase.effectChance))
                            {
                                enemyObject.StunStatus(true, characterBase);
                            }
                            break;
                    }
                    break;
                case CharacterBase.UnitEffects.Burn:
                    switch (characterBase.unitGroup)
                    {
                        case CharacterBase.UnitGroups.G:
                            enemyObject.BurnStatus(true, characterBase);
                            break;
                        case CharacterBase.UnitGroups.H:
                            if (EnforceManager.Instance.fireBurnPerAttackEffect)
                            {
                                enemyObject.BurnStatus(true, characterBase);
                            }
                            break;
                    }
                    break;
                case CharacterBase.UnitEffects.Bleed:
                    switch (characterBase.unitGroup)
                    {
                        case CharacterBase.UnitGroups.D:
                            if (EnforceManager.Instance.physicalBindBleed && enemyObject.isBind)
                            {
                                enemyObject.BleedStatus(true, characterBase);
                            }
                            break;
                        case CharacterBase.UnitGroups.J:
                            enemyObject.BleedStatus(true, characterBase);
                            break;
                    }
                    break;
     
                default:
                    return;
            }
        }
        protected internal float DamageCalculator(float damage,EnemyBase enemyBase, CharacterBase characterBase)
        {
            if (enemyBase.IsFreeze.ContainsKey(characterBase) && enemyBase.IsFreeze[characterBase] == CharacterBase.UnitGroups.C)
            {
                damage *= 1.15f;
            }

            if (enemyBase.isFreeze || EnforceManager.Instance.water2FreezeDamageBoost)
            {
                damage *= 1.15f;
            }

            switch (characterBase.unitGroup)
            {
                case CharacterBase.UnitGroups.A:
                    if (EnforceManager.Instance.divinePoisonDamageBoost && enemyBase.isPoison)
                    {
                        damage *= 1.25f;
                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Divine)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.B:
                    if (enemyBase.isFreeze ||enemyBase.isBind || enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn || enemyBase.isPoison)
                    {
                        if (EnforceManager.Instance.darkStatusAilmentDamageBoost)
                        {
                            if (_random.Next(100) < 50)
                            {
                                damage *= 1.15f;
                            }
                        }
                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Darkness)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.C:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Water)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
               case CharacterBase.UnitGroups.D:
                   if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Physics)
                   {
                       damage *= 0.8f;
                   }
                   return damage;
               case CharacterBase.UnitGroups.E:
                   if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Water)
                   {
                       damage *= 0.8f;
                   }
                   return damage;
                case CharacterBase.UnitGroups.F:
                   if (EnforceManager.Instance.poisonBleedingEnemyDamageBoost && enemyBase.isBleed )
                   {
                       damage *= 1.8f;
                   }
                   if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Poison)
                   {
                       damage *= 0.8f;
                   }
                   return damage;
                case CharacterBase.UnitGroups.G:
                    if (EnforceManager.Instance.fire2FreezeDamageBoost && enemyBase.isFreeze)
                    {
                        damage *= 2f;
                    }
                    if (EnforceManager.Instance.fire2BossDamageBoost && enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
                    {
                        damage += 1.3f;
                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Burn)
                    {
                        damage *= EnforceManager.Instance.fire2ChangeProperty ? 1f : 0.8f;
                    }
                    return damage; 
                case CharacterBase.UnitGroups.H:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Burn)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.I:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Poison)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.J:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Physics)
                    {
                        damage *= 0.8f;
                    }

                    if (enemyBase.isPoison && EnforceManager.Instance.physical2PoisonDamageBoost)
                    {
                        damage *= 1.6f;
                    }

                    if (EnforceManager.Instance.physical2BossBoost && enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
                    {
                        damage *= 1.3f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.K:
                    if (EnforceManager.Instance.dark2SameEnemyBoost)
                    {
                        if (!characterBase.AttackCounts.TryGetValue(enemyBase, out var attackCount))
                        {
                            attackCount = 0;
                        }

                        if (attackCount < 10)
                        {
                            damage *= 1.05f;
                            attackCount++;
                            characterBase.AttackCounts[enemyBase] = attackCount;
                        }
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Darkness)
                    {
                        damage *= 0.8f;
                    }

                    if (!EnforceManager.Instance.dark2StatusDamageBoost) return damage;
                    if (enemyBase.statusList.Count <= 0) return damage;
                    damage *= enemyBase.statusList.Count >= 5 ? 5 * 1.15f : enemyBase.statusList.Count * 1.15f;
                    return damage; 
            }
            return damage;
        }
        public IEnumerator PoisonEffect(EnemyBase hitEnemy, CharacterBase characterBase)
        {
            var dotDamage = characterBase.dotDamage;
            var poisonDotDamage = DamageCalculator(dotDamage, hitEnemy, characterBase);
            var venomDuration = characterBase.poisonTime;
            if (!hitEnemy.AlreadyPoison.TryGetValue(hitEnemy, out var isAlreadyPoison))
            {
                isAlreadyPoison = false;
                hitEnemy.AlreadyPoison[hitEnemy] = false;
            }
            if (isAlreadyPoison) yield break;
            hitEnemy.AlreadyPoison[hitEnemy] = true;
            for (var i = 0; i < venomDuration; i++)
            { 
                if (hitEnemy.isDead) yield break;
                hitEnemy.ReceiveDamage(hitEnemy,(int)poisonDotDamage, characterBase);
                yield return new WaitForSeconds(1f);
            }
            hitEnemy.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f);
            hitEnemy.PoisonStatus(false, characterBase);
            hitEnemy.AlreadyPoison[hitEnemy] = false;
            if (!hitEnemy.IsPoison.ContainsKey(characterBase)) yield break;
            hitEnemy.statusList.Remove(hitEnemy.IsPoison[characterBase]);
        }
        public IEnumerator BurningEffect(EnemyBase hitEnemy, CharacterBase characterBase)
        {
            var maxStack = characterBase.effectStack;
            if (!hitEnemy.AlreadyBurn.TryGetValue(hitEnemy, out var isAlreadyBurn))
            {
                isAlreadyBurn = false;
                hitEnemy.AlreadyBurn[hitEnemy] = false;
            }
            if (isAlreadyBurn) yield break;
            hitEnemy.AlreadyBurn[hitEnemy] = true;
            if (!_burnStacks.ContainsKey(hitEnemy))
            {
                _burnStacks[hitEnemy] = 1;
            }
            else
            {
                _burnStacks[hitEnemy] = Math.Min(_burnStacks[hitEnemy] + 1, maxStack);
            }
            var dotDamage = characterBase.dotDamage;
            var burnDotDamage = DamageCalculator(dotDamage, hitEnemy, characterBase) * _burnStacks[hitEnemy];
            var burningDuration = characterBase.burnTime; 
            for (var i = 0; i < burningDuration; i++)
            {
                if (hitEnemy.isDead) yield break;
                hitEnemy.ReceiveDamage(hitEnemy, (int)burnDotDamage , characterBase);
                yield return new WaitForSeconds(1f);
            }
            _burnStacks[hitEnemy]--;
            if (_burnStacks[hitEnemy] > 0) yield break;
            _burnStacks.Remove(hitEnemy);
            hitEnemy.AlreadyBurn[hitEnemy] = false;
            hitEnemy.BurnStatus(false, characterBase);
            if (!hitEnemy.IsBurn.ContainsKey(characterBase)) yield break;
            hitEnemy.statusList.Remove(hitEnemy.IsBurn[characterBase]);
        }
        public IEnumerator BleedEffect(EnemyBase hitEnemy, CharacterBase characterBase)
        {
            var dotDamage  = characterBase.dotDamage;
            if (hitEnemy.EnemyType == EnemyBase.EnemyTypes.Boss &&
                characterBase.unitGroup == CharacterBase.UnitGroups.J && 
                EnforceManager.Instance.physical2BossBoost)
            {
                dotDamage *= 0.7f;
            }
            var bleedDotDamage = DamageCalculator(dotDamage, hitEnemy, characterBase);
            var bleedDuration = characterBase.bleedTime;
            if (!hitEnemy.AlreadyBleed.TryGetValue(hitEnemy, out var isAlreadyBleed))
            {
                isAlreadyBleed = false;
                hitEnemy.AlreadyBleed[hitEnemy] = false;
            }
            if (isAlreadyBleed) yield break;
            hitEnemy.AlreadyBleed[hitEnemy] = true;
            for(var i = 0; i < bleedDuration; i++)
            {
                if (hitEnemy.isDead) yield break;
                hitEnemy.ReceiveDamage(hitEnemy, (int)bleedDotDamage, characterBase);
                yield return new WaitForSeconds(1f);
            }
            hitEnemy.AlreadyBleed[hitEnemy] = false;
            hitEnemy.BleedStatus(false, characterBase);
            if (!hitEnemy.IsBleed.ContainsKey(characterBase)) yield break;
            hitEnemy.statusList.Remove(hitEnemy.IsBleed[characterBase]);
        }
        // protected static void InstantKill(EnemyBase target)
        // {
        //     const EnemyBase.KillReasons reason = EnemyBase.KillReasons.ByPlayer;
        //     ExpManager.Instance.HandleEnemyKilled(reason);
        //     FindObjectOfType<EnemyBase>().EnemyKilledEvents(target);
        // }
    }
}

