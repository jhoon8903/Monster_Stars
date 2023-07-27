using System;
using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using Script.UIManager;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class WeaponBase : MonoBehaviour
    {
        public bool isInUse;
        protected float Speed { get; set; }
        protected float Damage { get; set; }
        protected float Distance { get; private set; }
        protected Color Sprite { get; set; }
        private CharacterBase.UnitProperties UnitProperty { get; set; }
        private CharacterBase.UnitEffects UnitEffect { get; set; }
        private CharacterBase.UnitGroups UnitGroup { get; set; }
        protected CharacterBase CharacterBase;
        private readonly System.Random _random = new System.Random();
        private EnemyBase _poisonedEnemy;
        protected readonly List<EnemyBase> HitEnemy = new List<EnemyBase>();
        public Vector2 Direction { get; set; } = Vector2.up;
        public void InitializeWeapon(CharacterBase characterBase , GameObject target = null)
        {
            CharacterBase = characterBase;
            UnitProperty = CharacterBase.UnitProperty;
            UnitEffect = CharacterBase.UnitEffect;
            Distance = CharacterBase.defaultAtkDistance;
            Damage = CharacterBase.DefaultDamage;
            Speed = CharacterBase.projectileSpeed * 2f;
            Sprite = GetComponent<SpriteRenderer>().color;
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
        protected void AtkEffect(EnemyBase enemyObject)
        {
            if (enemyObject == null) return;
            switch (UnitEffect)
            {
                case CharacterBase.UnitEffects.Bind:
                    IsBind(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Poison:
                    IsPoison(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Slow:
                    if (EnforceManager.Instance.waterFreeze)
                    {
                         IsFreeze(enemyObject);
                    }
                    else if (EnforceManager.Instance.water2Freeze && Chance(10))
                    {
                        IsFreezeE(enemyObject);
                    }
                    else
                    {
                        IsSlow(enemyObject);
                    }
                    break;
                case CharacterBase.UnitEffects.Burn:
                    IsBurn(enemyObject);
                    if (EnforceManager.Instance.fire2StunChance)
                    {
                        IsBurningPoison(enemyObject);
                    }
                    break;
                case CharacterBase.UnitEffects.Bleed:
                    if (EnforceManager.Instance.physicalBindBleed && enemyObject.isBind)
                    {
                        IsBleed(enemyObject);
                    }
                    break;
                case CharacterBase.UnitEffects.None:
                     IsKnockBack(enemyObject);
                     break;
                default:
                    return;
            }
        }

        private bool Chance(int percent)
        {
            return _random.Next(100) < percent;
        }

        private void IsFreeze(EnemyBase enemyStatus)
        {
            var chance = EnforceManager.Instance.waterFreezeChance ? 25 : 15;
            if (EnforceManager.Instance.waterFreeze && Chance(chance))
            {
                enemyStatus.isFreeze = true;
            }
        }
        private void IsFreezeE(EnemyBase enemyStatus)
        {
            var chance = EnforceManager.Instance.water2FreezeChanceBoost ? 20 : 10;
            if (EnforceManager.Instance.waterFreeze && Chance(chance))
            {
                enemyStatus.isFreezeE = true;
            }
        }
        private void IsKnockBack(EnemyBase enemyStatus)
        {
            if (EnforceManager.Instance.darkKnockBackChance && Chance(10))
            {
                enemyStatus.isKnockBack = true;
            }
        }
        private static IEnumerator IsDamageDebuff(EnemyBase enemyStatus)
        {
            enemyStatus.isReceiveDamageDebuff = true;
            yield return new WaitForSeconds(5f);
            enemyStatus.isReceiveDamageDebuff = false;
        }
        private void IsBind(EnemyBase enemyStatus)
        {
            var bindChance = EnforceManager.Instance.divineBindChanceBoost ? 50 : 30;
            if (Chance(bindChance))
            {
                enemyStatus.isBind = true;
            }
        }
        private void IsSlow(EnemyBase enemyStatus)
        {
            switch (UnitGroup)
            {
                case CharacterBase.UnitGroups.C:
                    enemyStatus.isSlowC = true;
                    break;
                case CharacterBase.UnitGroups.E:
                    enemyStatus.isSlowE = true;
                    break;
            }
        }
        private static void IsPoison(EnemyBase enemyStatus)
        { 
            if (!EnforceManager.Instance.poisonPerHitEffect) return;
            enemyStatus.IsPoison = true;
        }
        private static void IsBurn(EnemyBase enemyStatus)
        {
            enemyStatus.IsBurn = true;
        }
        private void IsBleed(EnemyBase enemyStatus)
        {
            enemyStatus.IsBleed = true;
        }
        private void IsBurningPoison(EnemyBase enemyStatus)
        {
            if (_random.Next(100) < 20)
            {
                enemyStatus.isBurningPoison = true;
            }
        }

        protected internal float DamageCalculator(float damage,EnemyBase enemyBase, CharacterBase.UnitGroups unitGroup)
        {
            if (enemyBase.isFreezeE)
            {
                damage *= 1.5f;
            }

            if (enemyBase.isFreeze)
            {
                damage *= 1.15f;
            }

            switch (unitGroup)
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
                    if (enemyBase.isFreeze ||enemyBase.isBind || enemyBase.isSlowC || enemyBase.isSlowE || enemyBase.isBleed || enemyBase.isBurn || enemyBase.isPoison)
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
                       damage *= 1.5f;
                   }
                   if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Burn)
                   {
                       damage *= 0.8f;
                   }
                   return damage;
                case CharacterBase.UnitGroups.G:
                    if (EnforceManager.Instance.fire2PoisonDamageIncrease && enemyBase.isPoison)
                    {
            
                        damage *= 1.15f;
                    }
                    if (EnforceManager.Instance.fire2NoBurnDamageIncrease)
                    {
                        damage *= 1.5f;
                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Burn)
                    {
                        damage *= 0.8f;
                    }
                    return damage; 
                case CharacterBase.UnitGroups.H:
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Burn)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        protected static void InstantKill(EnemyBase target)
        {
            const EnemyBase.KillReasons reason = EnemyBase.KillReasons.ByPlayer;
            ExpManager.Instance.HandleEnemyKilled(reason);
            FindObjectOfType<EnemyBase>().EnemyKilledEvents(target);
        }
    }
}

