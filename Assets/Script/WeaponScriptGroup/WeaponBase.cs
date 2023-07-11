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
        protected float Damage { get; private set; }
        protected float Distance { get; set; }
        private CharacterBase.UnitProperties UnitProperty { get; set; }
        private CharacterBase.UnitEffects UnitEffect { get; set; }
        private CharacterBase.UnitGroups UnitGroup { get; set; }
        protected Vector3 StartingPosition;
        protected CharacterBase CharacterBase;
        private readonly System.Random _random = new System.Random();
        private EnemyBase _poisonedEnemy;
        protected readonly List<EnemyBase> HitEnemy = new List<EnemyBase>();
        public Vector2 Direction { get; set; } = Vector2.up;
        public void InitializeWeapon(CharacterBase characterBase)
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
                switch (UnitEffect)
            {
                case CharacterBase.UnitEffects.Restraint:
                    IsRestraint(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Poison:
                    IsPoison(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Slow:
                    IsSlow(enemyObject);
                    break;
                case CharacterBase.UnitEffects.None:
                    break;
                case CharacterBase.UnitEffects.Burn:
                    IsBurn(enemyObject);
                    break;
                case CharacterBase.UnitEffects.Bleed:
                    IsBleed(enemyObject);
                     break;
                case CharacterBase.UnitEffects.Stun:
                case CharacterBase.UnitEffects.Strike:
                case CharacterBase.UnitEffects.Darkness:
                default:
                    Debug.Log("UnKnown AtkEffect");
                    return;
            }
        }

        private void IsRestraint(EnemyBase enemyStatus)
        {
            if (!EnforceManager.Instance.activeRestraint) return;
            if (_random.Next(100) < 20)
            {
                enemyStatus.isRestraint = true;
            }
        }
        private static void IsSlow(EnemyBase enemyStatus)
        {
            enemyStatus.isSlow = true;
        }
        private static void IsPoison(EnemyBase enemyStatus)
        {
            if (!EnforceManager.Instance.activatePoison) return;
            enemyStatus.IsPoison = true;
        }

        private static void IsBurn(EnemyBase enemyStatus)
        {
            if (EnforceManager.Instance.fireDeleteBurnIncreaseDamage) return;
            enemyStatus.IsBurn = true;
        }

        private static void IsBleed(EnemyBase enemyStatus)
        {
            if (!EnforceManager.Instance.physics2ActivateBleed) return;
            enemyStatus.IsBleed = true;
        }
        protected float DamageCalculator(float damage,EnemyBase enemyBase)
        {
            switch (UnitProperty)
            {
                case CharacterBase.UnitProperties.Divine:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Divine)
                    {
                        if (EnforceManager.Instance.divinePoisonAdditionalDamage && enemyBase.isPoison)
                        {
                            var increaseDamage = 1f + (1f * EnforceManager.Instance.divinePoisonAdditionalDamageCount );
                            damage *= increaseDamage;
                        }
                    } 
                    
                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Divine)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Physics:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Physics)
                    {
                        if (EnforceManager.Instance.physicSlowAdditionalDamage && enemyBase.isSlow)
                        {
                            damage *= 2.0f;
                        }

                        if (EnforceManager.Instance.physicIncreaseDamage)
                        {
                            damage *= damage * (1f + EnforceManager.Instance.increasePhysicsDamage);
                        }
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Physics)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Poison:
                    if (enemyBase.RegistryType != EnemyBase.RegistryTypes.Poison)
                    {
                        if (EnforceManager.Instance.poisonRestraintAdditionalDamage && enemyBase.isRestraint)
                        {
                            damage *= 3.0f;
                        }
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Poison)
                    {
                        damage *= 0.8f;
                    }
                    return damage;

                case CharacterBase.UnitProperties.Water:
                    switch (UnitGroup)
                    {
                        case CharacterBase.UnitGroups.C:
                        {
                            if (EnforceManager.Instance.water2IncreaseDamage >= 1)
                            {
                                damage *= 1f + (0.09f * EnforceManager.Instance.water2IncreaseDamage);
                            }

                            if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Water)
                            {
                                damage *= 0.8f;
                            }
                            return damage;
                        }

                        case CharacterBase.UnitGroups.E:
                        {
                            if (enemyBase.isRestraint && EnforceManager.Instance.waterRestraintIncreaseDamage)
                            {
                                damage *= 2.0f;
                            }

                            if (enemyBase.isBurn && EnforceManager.Instance.waterBurnAdditionalDamage)
                            {
                                damage *= 3.0f;
                            }

                            if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Water)
                            {
                                damage *= 0.8f;
                            }

                            return damage * EnforceManager.Instance.increaseWaterDamage;
                        }
                    }
                    break;
                case CharacterBase.UnitProperties.Fire:
                    if (enemyBase.isBleed && EnforceManager.Instance.fireBleedingAdditionalDamage)
                    {
                        damage *= 2.5f ;
                    }

                    if (EnforceManager.Instance.fireDeleteBurnIncreaseDamage)
                    {
                        damage *= 3.0f;
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Burn)
                    {
                        damage *= 0.8f;
                    }
                    return damage * (1f + 0.15f * EnforceManager.Instance.fireIncreaseDamage);
                
                case CharacterBase.UnitProperties.Darkness:
                    if (enemyBase.isSlow && EnforceManager.Instance.darkSlowAdditionalDamage)
                    {
                        damage *= 1.5f;
                    }
                    if (enemyBase.isBleed && EnforceManager.Instance.darkBleedAdditionalDamage)
                    {
                        damage *= 3.0f;
                    }

                    if (enemyBase.RegistryType == EnemyBase.RegistryTypes.Darkness)
                    {
                        damage *= 0.8f;
                    }
                    return damage;
                case CharacterBase.UnitProperties.None:
                    return damage;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return damage;
        }
        protected static void InstantKill(EnemyBase target)
        {
            const EnemyBase.KillReasons reason = EnemyBase.KillReasons.ByPlayer;
            ExpManager.Instance.HandleEnemyKilled(reason);
            FindObjectOfType<EnemyBase>().EnemyKilledEvents(target);
        }

        protected static IEnumerator FlickerEffect(SpriteRenderer renderer, Color targetColor, float duration)
        {
            var originalColor = renderer.color;
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                var lerpValue = (Mathf.Sin(elapsedTime / duration * (2f * Mathf.PI)) + 1f) / 2f;
                renderer.color = Color.Lerp(originalColor, targetColor, lerpValue);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            renderer.color = originalColor;
        }
    }
}

