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
        private CharacterBase.UnitProperties UnitProperty { get; set; }
        private CharacterBase.UnitEffects UnitEffect { get; set; }
        private CharacterBase.UnitGroups UnitGroup { get; set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;
        private readonly System.Random _random = new System.Random();
        private EnemyBase _poisonedEnemy;
        protected readonly List<EnemyBase> HitEnemy = new List<EnemyBase>();
        public Vector2 Direction { get; set; } = Vector2.up;
        public void InitializeWeapon(CharacterBase characterBase , GameObject target = null)
        {
            CharacterBase = characterBase;
        }
        public virtual IEnumerator UseWeapon()
        {
            isInUse = true;
            UnitProperty = CharacterBase.UnitProperty;
            UnitEffect = CharacterBase.UnitEffect;
            StartingPosition = transform.position;
            Distance = CharacterBase.defaultAtkDistance;
            Damage = CharacterBase.DefaultDamage;
            Speed = CharacterBase.projectileSpeed * 2f;
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
            if (UnitGroup == CharacterBase.UnitGroups.C && EnforceManager.Instance.waterSlowEnemyStunChance)
            {
                IsSlowStun(enemyObject);
            }
            if (UnitGroup == CharacterBase.UnitGroups.D && EnforceManager.Instance.water2StunChanceAgainstBleeding)
            {
                IsSlowBleedStun(enemyObject);
            }
            switch (UnitEffect)
            {
                case CharacterBase.UnitEffects.Bind:
                    IsBind(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Poison:
                    IsPoison(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Slow:
                    IsSlow(enemyObject);
                    if (EnforceManager.Instance.waterDamageIncreaseDebuff)
                    {
                        StartCoroutine(IsDamageDebuff(enemyObject));
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
                    IsBleed(enemyObject);
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
        private void IsKnockBack(EnemyBase enemyStatus)
        {
            if (EnforceManager.Instance.darkKnockBackChance && Chance(99))
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
        private static void IsSlow(EnemyBase enemyStatus)
        {
            enemyStatus.isSlow = true;
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
            if (!EnforceManager.Instance.physicalBleedingChance) return;
            if (_random.Next(100) < 10)
            {
                enemyStatus.IsBleed = true;
            }
        }
        private void IsSlowStun(EnemyBase enemyStatus)
        {
            if (_random.Next(100) < 40)
            {
                enemyStatus.isSlowStun = true;
            }
        }
        private void IsSlowBleedStun(EnemyBase enemyStatus)
        {
            if (_random.Next(100) < 10)
            {
                enemyStatus.isSlowBleedStun = true;
            }
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
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.A:
                    if (EnforceManager.Instance.divinePoisonDamageBoost && enemyBase.isPoison)
                    {
                        const float increaseDamage = 1.25f;
                        damage *= increaseDamage;
                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Divine)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.B:
                    if (enemyBase.isBind || enemyBase.isSlow || enemyBase.isBleed || enemyBase.isBurn || enemyBase.isPoison)
                    {
                        if (EnforceManager.Instance.darkStatusAilmentDamageChance)
                        {
                            if (_random.Next(100) < 10)
                            {
                                damage *= 5f;
                            }
                        }

                        if (EnforceManager.Instance.darkStatusAilmentDamageBoost)
                        {
                            damage *= 1.15f;
                        }

                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Darkness)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitGroups.C:
                    if (!EnforceManager.Instance.waterAllyDamageBoost)
                    {
                        if (LevelUpRewardManager.Instance.HasUnitInGroup(CharacterBase.UnitGroups.E))
                        {
                            damage *= 2f;
                        }
                    }
                    if (enemyBase.isSlow && EnforceManager.Instance.waterSlowEnemyDamageBoost)
                    {
                        damage *= 1.2f;
                    }
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Water)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
               case CharacterBase.UnitGroups.D:
                   // if (EnforceManager.Instance.physicalDamage35Boost)
                   // {
                   //     if(LevelUpRewardManager.Instance.HasUnitInGroup(CharacterBase.UnitGroups.C))
                   //     {
                   //         damage *= 1.35f;
                   //     }
                   // }
                   if (EnforceManager.Instance.physicalSlowEnemyDamageBoost && enemyBase.isSlow)
                   {
                       damage *= 1.15f;
                   }
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
                    if (EnforceManager.Instance.fireSlowEnemyDamageBoost && enemyBase.isSlow)
                    {
                        damage *= 1.1f;
                    }
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

