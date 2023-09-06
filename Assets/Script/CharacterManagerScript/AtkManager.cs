using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using Script.UIManager;
using Script.WeaponScriptGroup;
using UnityEngine;
using Ogre = Script.CharacterGroupScript.Ogre;

namespace Script.CharacterManagerScript
{
    public class AttackData
    {
        public GameObject Unit { get; }
        public WeaponsPool.WeaponType WeaponType { get;}

        public AttackData(GameObject unit, WeaponsPool.WeaponType weaponType)
        {
            Unit = unit;
            WeaponType = weaponType;
        }
    }

    public class AtkManager : MonoBehaviour
    {
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private WeaponsPool weaponsPool;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EnemyPatternManager enemyPatternManager;
        public float attackRate;
        public float atkRate;
        public List<GameObject> enemyList = new List<GameObject>();
        public List<GameObject> weaponsList = new List<GameObject>();
        public static AtkManager Instance { get; private set; }
        public int groupDAtkCount;
        public int groupFCount;
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void CheckForAttack()
        {
            var characters = characterPool.UsePoolCharacterList();
            foreach (var atkUnit in characters
                         .Select(character => character.GetComponent<CharacterBase>())
                         .Where(atkUnit => atkUnit.unitPuzzleLevel >= 2))
            {
                StartCoroutine(AtkMotion(atkUnit));
            }
        }
        private IEnumerator AtkMotion(CharacterBase unit)
        {
            atkRate = unit.defaultAtkRate * (attackRate - EnforceManager.Instance.increaseAtkRate / 100f);
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                enemyList = unit.DetectEnemies();
                unit.GetComponent<SpriteRenderer>().sortingOrder = enemyList.Count <= 0 ? 1 : 2;
                unit.cover.GetComponent<SpriteRenderer>().sortingOrder = enemyList.Count <= 0 ? 2 : 1;
                if (enemyList.Count <= 0) continue;
                var atkUnit = unit.gameObject; 
                var unitAtkType = unit.UnitAtkType;
                var unitGroup = unit.unitGroup; 
                switch (unitAtkType)
                {
                    case CharacterBase.UnitAtkTypes.Projectile:
                        ProjectileAttack(atkUnit, unitGroup);
                        break;
                    case CharacterBase.UnitAtkTypes.Circle:
                        CircleAttack(atkUnit, unitGroup);
                        break;
                    case CharacterBase.UnitAtkTypes.GuideProjectile:
                        GuideProjectileAttack(atkUnit, unitGroup);
                        break;
                    case CharacterBase.UnitAtkTypes.None:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                yield return new WaitForSeconds(atkRate);
            }
            // ReSharper disable once IteratorNeverReturns
        }
        private void ProjectileAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.Octopus:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Octopus));
                    break;
                case CharacterBase.UnitGroups.Ogre:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Ogre));
                    break;
                case CharacterBase.UnitGroups.DeathChiller:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.DeathChiller));
                    break;
                case CharacterBase.UnitGroups.Fishman:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Fishman));
                    break;
                case CharacterBase.UnitGroups.Beholder:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Beholder));
                    break;
                case CharacterBase.UnitGroups.Berserker: 
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Berserker));
                    break;
                case CharacterBase.UnitGroups.DarkElf:
                    switch (EnforceManager.Instance.darkElfDualAttack)
                    {
                        case true when EnforceManager.Instance.darkElfDoubleAttack:
                            StartCoroutine(CombinedAttack(new AttackData(unit, WeaponsPool.WeaponType.DarkElf)));
                            break;
                        case true:
                            StartCoroutine(DualAttack(new AttackData(unit, WeaponsPool.WeaponType.DarkElf)));
                            break;
                        default:
                        {
                            if (EnforceManager.Instance.darkElfDoubleAttack)
                            {
                                StartCoroutine(DoubleFire(new AttackData(unit, WeaponsPool.WeaponType.DarkElf)));
                            }
                            else
                            {
                                Attack(new AttackData(unit, WeaponsPool.WeaponType.DarkElf));
                            }
                            break;
                        }
                    }
                    break;
                case CharacterBase.UnitGroups.Phoenix:
                    if (unit.GetComponent<CharacterBase>().CurrentWeapon == null || unit.GetComponent<CharacterBase>().CurrentWeapon.activeSelf == false)
                    {
                        unit.GetComponent<CharacterBase>().CurrentWeapon = Attack(new AttackData(unit, WeaponsPool.WeaponType.Phoenix));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitGroup), unitGroup, null);
            }
        }
        private void CircleAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.Orc:
                    if (unit.GetComponent<CharacterBase>().CurrentWeapon == null || unit.GetComponent<CharacterBase>().CurrentWeapon.activeSelf == false)
                    {
                        unit.GetComponent<CharacterBase>().CurrentWeapon = Attack(new AttackData(unit, WeaponsPool.WeaponType.Orc));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(unitGroup), unitGroup, null);
            }
        }
        private void GuideProjectileAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.Skeleton:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Skeleton)); 
                    break;
                case CharacterBase.UnitGroups.Cobra:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Cobra));
                    break;
            }
        }
        private GameObject Attack(AttackData attackData, GameObject target = null)
        {
            var unit = attackData.Unit;
            switch (unit.GetComponent<CharacterBase>().unitGroup)
            {
               case CharacterBase.UnitGroups.Octopus:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.iagAttackMotion);
                   if (EnforceManager.Instance.octopusThirdAttackBoost)
                   {
                       unit.GetComponent<Octopus>().atkCount++;
                   }
                   break;
               case CharacterBase.UnitGroups.Ogre:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.bcjAttackMotion);
                   if (EnforceManager.Instance.ogreThirdAttackDamageBoost)
                   {
                       unit.GetComponent<Ogre>().atkCount++;
                   }
                   break;
               case CharacterBase.UnitGroups.DeathChiller:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.cAttackMotion);
                   // if (EnforceManager.Instance.deathChillerBackAttackBoost)
                   // {
                   //     groupCAtkCount++;
                   //     if (groupCAtkCount == 100)
                   //     {
                   //         StartCoroutine(enemyPatternManager.GlobalFreezeEffect());
                   //     }
                   // }
                   break;
               case CharacterBase.UnitGroups.Orc:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.dAttackMotion);
                   if (EnforceManager.Instance.orcRatePerAttack)
                   {
                       groupDAtkCount++;
                       if (groupDAtkCount % 3 == 0)
                       {
                           if (unit.GetComponent<Orc>().groupDAtkRate < 0.6f)
                           {
                               unit.GetComponent<Orc>().groupDAtkRate += 0.01f;
                           }
                       }
                   }
                   break;
               case CharacterBase.UnitGroups.Skeleton:
                   if (EnforceManager.Instance.skeletonDamagePerBoost)
                   {
                       groupFCount++;
                       if (groupFCount % 5 == 0 && unit.GetComponent<Skeleton>().groupFDamage < 0.6f)
                       {
                           unit.GetComponent<Skeleton>().groupFDamage += 0.01f;
                       }
                   }
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.kfeAttackMotion);
                   break;
               case CharacterBase.UnitGroups.Berserker:
                   if (EnforceManager.Instance.berserkerThirdBoost)
                   {
                       unit.GetComponent<Berserker>().atkCount++;
                   }
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.bcjAttackMotion);
                   break;
               case CharacterBase.UnitGroups.Fishman:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.kfeAttackMotion);
                   break;
               case CharacterBase.UnitGroups.Phoenix:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.iagAttackMotion);
                   break;
               case CharacterBase.UnitGroups.Beholder:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.bcjAttackMotion);
                   break;
               case CharacterBase.UnitGroups.Cobra:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.iagAttackMotion);
                   break;
               case CharacterBase.UnitGroups.DarkElf:
                   SoundManager.Instance.AttackSoundEffect(SoundManager.Instance.kfeAttackMotion);
                   break;
               case CharacterBase.UnitGroups.None:
                   break;
            }
            var weaponType = attackData.WeaponType;
            var weaponObject = weaponsPool.SpawnFromPool(weaponType, attackData.Unit,attackData.Unit.GetComponent<CharacterBase>(), unit.transform.position, unit.transform.rotation);
            if (weaponObject == null) return null;
            var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
            if (weaponBase == null) return null;
            if (target != null)
            {
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>(), target);
            }
            else
            {
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
            }
            weaponsList.Add(weaponBase.gameObject);
            StartCoroutine(weaponBase.UseWeapon());
            return weaponObject;
        }
        private IEnumerator DualAttack(AttackData attackData)
        {
            var unit = attackData.Unit;
            if (EnforceManager.Instance.octopusThirdAttackBoost)
            {
                unit.GetComponent<Octopus>().atkCount++;
            }
            var weaponType = attackData.WeaponType;
            var unitPosition = unit.transform.position;
            var weaponDirections = new[]
            {
                new {Direction = Vector2.up, Rotation = Quaternion.identity},
                new {Direction = Vector2.down, Rotation = Quaternion.Euler(0, 0, 180)}
            };
            foreach (var weapon in weaponDirections)
            {
                var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit, unit.GetComponent<CharacterBase>(), unitPosition, weapon.Rotation);
                var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
                weaponsList.Add(weaponBase.gameObject);
                weaponBase.direction = weapon.Direction;
                StartCoroutine(weaponBase.UseWeapon());
            }
            yield return null;
        }
        private IEnumerator DoubleFire(AttackData attackData)
        {
            var unit = attackData.Unit;
            var weaponType = attackData.WeaponType; 
            const float offset = 0.3f;
            var unitPosition = unit.transform.position;
            var weaponPositions = new []
            {
                new Vector3(unitPosition.x - offset, unitPosition.y, 0),
                new Vector3(unitPosition.x + offset, unitPosition.y, 0)
            };
            foreach (var position in weaponPositions)
            {
                var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit, unit.GetComponent<CharacterBase>(), position, unit.transform.rotation);
                var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
                weaponsList.Add(weaponBase.gameObject);
                StartCoroutine(weaponBase.UseWeapon());
            }
            yield return null;
        }

        private IEnumerator CombinedAttack(AttackData attackData)
        {
            var unit = attackData.Unit;
            var weaponType = attackData.WeaponType;
            var unitPosition = unit.transform.position;
            const float offset = 0.3f;
            
            var directionsAndPositions = new[]
            {
                new {Direction = Vector2.up, Rotation = Quaternion.identity, Position = new Vector3(unitPosition.x - offset, unitPosition.y, 0)},
                new {Direction = Vector2.up, Rotation = Quaternion.identity, Position = new Vector3(unitPosition.x + offset, unitPosition.y, 0)},
                new {Direction = Vector2.down, Rotation = Quaternion.Euler(0, 0, 180), Position = new Vector3(unitPosition.x - offset, unitPosition.y, 0)},
                new {Direction = Vector2.down, Rotation = Quaternion.Euler(0, 0, 180), Position = new Vector3(unitPosition.x + offset, unitPosition.y, 0)}
            };

            foreach (var item in directionsAndPositions)
            {
                var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit, unit.GetComponent<CharacterBase>(), item.Position, item.Rotation);
                var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
                weaponsList.Add(weaponBase.gameObject);
                weaponBase.direction = item.Direction;
                StartCoroutine(weaponBase.UseWeapon());
            }
    
            yield return null;
        }


        public void ClearWeapons()
        {
            for (var i = weaponsList.Count - 1; i >= 0; i--)
            {
                var weaponBase = weaponsList[i].GetComponent<WeaponBase>();
                if (weaponBase.isInUse)
                {
                    weaponBase.StopUseWeapon(weaponBase.gameObject);
                }
            }
        }
        // private IEnumerator DoubleAtk(AttackData attackData)
        // {
        //     var unit = attackData.Unit;
        //     var weaponType = attackData.WeaponType;
        //     var enemies = unit.GetComponent<Skeleton>().DetectEnemies();
        //     if (enemies.Count == 0) yield break;
        //     Attack(new AttackData(unit, weaponType), enemies[0]);
        //     Attack(new AttackData(unit, weaponType), enemies.Count > 1 ? enemies[1] : enemies[0]);
        // }

        // public IEnumerator SplitAttack(AttackData attackData, Vector3 enemyPosition)
        // {
        //     var unit = attackData.Unit;
        //     var weaponType = attackData.WeaponType;
        //     var weaponDirections = new[]
        //     {
        //         new {Direction = Vector2.left, Rotation = Quaternion.Euler(0, 0, 90)},
        //         new {Direction = Vector2.right, Rotation = Quaternion.Euler(0, 0, -90)}
        //     };
        //     foreach (var weapon in weaponDirections)
        //     {
        //         var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.GetComponent<CharacterBase>().unitPuzzleLevel, enemyPosition, weapon.Rotation);
        //         var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
        //         weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
        //         weaponsList.Add(weaponBase.gameObject);
        //         weaponBase.direction = weapon.Direction;
        //         StartCoroutine(weaponBase.UseWeapon());
        //     }
        //     yield return null;
        // }
    }
}