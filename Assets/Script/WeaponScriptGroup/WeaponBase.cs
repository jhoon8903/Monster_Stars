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
        protected float Damage { get; private set; }
        protected float Distance { get; private set; }
        private CharacterBase.UnitProperties UnitProperty { get; set; }
        private CharacterBase.UnitEffects UnitEffect { get; set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;
        protected readonly System.Random Random = new System.Random();
        private EnemyBase _poisonedEnemy;
        protected readonly List<EnemyBase> HitEnemy = new List<EnemyBase>();
        protected EnforceManager EnforceManager;
        private WaveManager _waveManager;

        public void InitializeWeapon(CharacterBase characterBase)
        {
            CharacterBase = characterBase;
            EnforceManager = FindObjectOfType<EnforceManager>();
            _waveManager = FindObjectOfType<WaveManager>();
        }
        public virtual IEnumerator UseWeapon()
        {
            isInUse = true;
            UnitProperty = CharacterBase.UnitProperty;
            UnitEffect = CharacterBase.UnitEffect;
            StartingPosition = transform.position;
            Distance = CharacterBase.defaultAtkDistance;
            Damage = CharacterBase.defaultDamage;
            Speed = CharacterBase.projectileSpeed;
            yield return null;
        }
        protected void StopUseWeapon(GameObject weapon)
        {
            isInUse = false;
            WeaponsPool.ReturnToPool(weapon);
        }
        protected void AtkEffect(EnemyBase enemyObject)
        {
            switch (UnitEffect)
            {
                case CharacterBase.UnitEffects.Restraint:
                    RestraintAttribution(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Poison:
                    PoisonAttribution(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Slow:
                    SlowAttribution(enemyObject);
                    break;
                case CharacterBase.UnitEffects.None:
                    PhysicsAttribution(enemyObject);
                    break;
                default:
                    Debug.Log("UnKnown AtkEffect");
                    return;
            }
        }
        private void RestraintAttribution(EnemyBase enemyStatus)
        {
            switch (enemyStatus.RegistryType)
            {
                case EnemyBase.RegistryTypes.Physics:
                case EnemyBase.RegistryTypes.Poison:
                case EnemyBase.RegistryTypes.None:
                    IsRestraint(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.Divine:
                    break;
                default:
                    Debug.Log("UnKnown Registries");
                    break;
            }
        }
        private static void PoisonAttribution(EnemyBase enemyStatus)
        {
            switch (enemyStatus.RegistryType)
            {
                case EnemyBase.RegistryTypes.Divine:
                case EnemyBase.RegistryTypes.Physics:
                case EnemyBase.RegistryTypes.None:
                    IsPoison(enemyStatus);
                    break;
                case EnemyBase.RegistryTypes.Poison:
                    break;
                default:
                    Debug.Log("UnKnown Registries");
                    break;
            }
        }
        private static void SlowAttribution(EnemyBase enemyStatus)
        {
            switch (enemyStatus.RegistryType)
            {
                case EnemyBase.RegistryTypes.Divine:
                case EnemyBase.RegistryTypes.Physics:
                case EnemyBase.RegistryTypes.Poison:
                case EnemyBase.RegistryTypes.None:
                    IsSlow(enemyStatus);
                    break;
                default:
                    Debug.Log("UnKnown Registries");
                    break;
            }
        }
        private static void PhysicsAttribution(EnemyBase enemyStatus)
        {
            switch (enemyStatus.RegistryType)
            {
                case EnemyBase.RegistryTypes.Divine:
                case EnemyBase.RegistryTypes.Physics:
                case EnemyBase.RegistryTypes.Poison:
                case EnemyBase.RegistryTypes.None:
                    break;
                default:
                    Debug.Log("UnKnown Registries");
                    break;
            }
        }
        private void IsRestraint(EnemyBase enemyStatus)
        {
            if (!EnforceManager.activeRestraint) return;
            if (Random.Next(100) < 20)
            {
                enemyStatus.IsRestraint = true;
            }
        }
        private static void IsSlow(EnemyBase enemyStatus)
        {
            enemyStatus.IsSlow = true;
        }
        private static void IsPoison(EnemyBase enemyStatus)
        {
            enemyStatus.IsPoison = true;
        }
        protected float DamageCalculator(float damage,EnemyBase enemyBase)
        {
            switch (UnitProperty)
            {
                case CharacterBase.UnitProperties.Divine:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Divine)
                    {
                        if (EnforceManager.divinePoisonAdditionalDamage && enemyBase.IsPoison)
                        {
                            var increaseDamage = 1f + (0.3f * EnforceManager.divinePoisonAdditionalDamageCount );
                            damage *= increaseDamage;
                        }
                        return damage;
                    } 
                    
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Divine)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Physics:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Physics)
                    {
                        if (EnforceManager.physicSlowAdditionalDamage && enemyBase.IsSlow)
                        {
                            damage *= 2.0f;
                        }

                        if (EnforceManager.physicIncreaseDamage)
                        {
                            damage += damage * EnforceManager.increasePhysicsDamage;
                        }
                        return damage;
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Physics)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Poison:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Poison)
                    {
                        if (EnforceManager.poisonRestraintAdditionalDamage && enemyBase.IsRestraint)
                        {
                            damage *= 2.0f;
                        }
                        return damage;
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Poison)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Water:

                        damage += damage * EnforceManager.IncreaseWaterDamage;
                        return damage;
            }
            return damage;
        }
        protected void InstantKill(EnemyBase enemy)
        {
            WeaponsPool.ReturnToPool(gameObject);
            _waveManager.EnemyDestroyInvoke();
            if (ExpManager.Instance == null) return;
            const EnemyBase.KillReasons reason = EnemyBase.KillReasons.ByPlayer;
            ExpManager.Instance.HandleEnemyKilled(reason);
        }
    }
}

