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
                        case CharacterBase.UnitGroups.Octopus:
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
                       case CharacterBase.UnitGroups.Fishman:
                           if (EnforceManager.Instance.fishmanFreeze && Chance(characterBase.effectChance))
                           {
                               enemyObject.FreezeStatus(true, characterBase);
                           }
                           else
                           {
                               enemyObject.SlowStatus(true, characterBase);
                           }
                           break;
                       case CharacterBase.UnitGroups.DeathChiller:
                           if (EnforceManager.Instance.deathChillerFreeze && Chance(characterBase.effectChance))
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
                        case CharacterBase.UnitGroups.Octopus:
                            if (EnforceManager.Instance.octopusPoisonAttack && Chance(20))
                            {
                                enemyObject.PoisonStatus(true, characterBase);
                            }
                            break;
                        case CharacterBase.UnitGroups.Ogre:
                            if (EnforceManager.Instance.ogreKnockBackChance && Chance(characterBase.effectChance))
                            {
                                enemyObject.KnockBackStatus(true, characterBase);
                            }
                            break;
                        case CharacterBase.UnitGroups.DarkElf:
                            if (EnforceManager.Instance.darkElfStatusPoison)
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
                        case CharacterBase.UnitGroups.Skeleton:
                            if (EnforceManager.Instance.skeletonPerHitEffect)
                            {
                                enemyObject.PoisonStatus(true, characterBase);
                            }
                            break;
                        case CharacterBase.UnitGroups.Cobra:
                            enemyObject.PoisonStatus(true, characterBase);
                            if ( EnforceManager.Instance.cobra2StunToChance && Chance(characterBase.effectChance))
                            {
                                enemyObject.StunStatus(true, characterBase);
                            }
                            break;
                    }
                    break;
                case CharacterBase.UnitEffects.Burn:
                    switch (characterBase.unitGroup)
                    {
                        case CharacterBase.UnitGroups.Phoenix:
                            enemyObject.BurnStatus(true, characterBase);
                            break;
                        case CharacterBase.UnitGroups.Beholder:
                            if (EnforceManager.Instance.beholderBurnPerAttackEffect)
                            {
                                enemyObject.BurnStatus(true, characterBase);
                            }
                            break;
                    }
                    break;
                case CharacterBase.UnitEffects.Bleed:
                    switch (characterBase.unitGroup)
                    {
                        case CharacterBase.UnitGroups.Orc:
                            if (EnforceManager.Instance.orcBindBleed && enemyObject.isBind)
                            {
                                enemyObject.BleedStatus(true, characterBase);
                            }
                            break;
                        case CharacterBase.UnitGroups.Berserker:
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
            if (enemyBase.IsFreeze.ContainsKey(characterBase) && enemyBase.IsFreeze[characterBase] == CharacterBase.UnitGroups.DeathChiller)
            {
                damage *= 1.15f;
            }

            if (enemyBase.isFreeze || EnforceManager.Instance.fishmanFreezeDamageBoost)
            {
                damage *= 1.15f;
            }

            switch (characterBase.unitGroup)
            {
                case CharacterBase.UnitGroups.Octopus:
                    if (EnforceManager.Instance.octopusBleedDamageBoost && enemyBase.isBleed)
                    {
                        damage *= 1.5f;
                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Darkness)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.Ogre:
                    if (enemyBase.isFreeze ||enemyBase.isBind || enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn || enemyBase.isPoison)
                    {
                        if (EnforceManager.Instance.ogreStatusAilmentDamageBoost)
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
                case CharacterBase.UnitGroups.DeathChiller:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Water)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
               case CharacterBase.UnitGroups.Orc:
                   if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Physics)
                   {
                       damage *= 0.8f;
                   }
                   return damage;
               case CharacterBase.UnitGroups.Fishman:
                   if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Water)
                   {
                       damage *= 0.8f;
                   }
                   return damage;
                case CharacterBase.UnitGroups.Skeleton:
                   if (EnforceManager.Instance.skeletonBleedingEnemyDamageBoost && enemyBase.isBleed )
                   {
                       damage *= 1.8f;
                   }
                   if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Poison)
                   {
                       damage *= 0.8f;
                   }
                   return damage;
                case CharacterBase.UnitGroups.Phoenix:
                    if (EnforceManager.Instance.phoenixFreezeDamageBoost && enemyBase.isFreeze)
                    {
                        damage *= 2f;
                    }
                    if (EnforceManager.Instance.phoenixBossDamageBoost && enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
                    {
                        damage += 1.3f;
                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Burn)
                    {
                        damage *= EnforceManager.Instance.phoenixChangeProperty ? 1f : 0.8f;
                    }
                    return damage; 
                case CharacterBase.UnitGroups.Beholder:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Burn)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.Cobra:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Poison)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.Berserker:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Physics)
                    {
                        damage *= 0.8f;
                    }

                    if (enemyBase.isPoison && EnforceManager.Instance.berserkerPoisonDamageBoost)
                    {
                        damage *= 1.6f;
                    }

                    if (EnforceManager.Instance.berserkerBossBoost && enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
                    {
                        damage *= 1.3f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.DarkElf:
                    if (EnforceManager.Instance.darkElfSameEnemyBoost)
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

                    if (!EnforceManager.Instance.darkElfStatusDamageBoost) return damage;
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
            hitEnemy.GetComponentInChildren<SpriteRenderer>().DOColor(Color.white, 0.2f);
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
                characterBase.unitGroup == CharacterBase.UnitGroups.Berserker && 
                EnforceManager.Instance.berserkerBossBoost)
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

