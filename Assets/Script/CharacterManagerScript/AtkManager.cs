using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterGroupScript;
using Script.EnemyManagerScript;
using Script.RewardScript;
using Script.WeaponScriptGroup;
using UnityEngine;

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
        private const float AttackRate = 1f;
        public float atkRate;
        public List<GameObject> enemyList = new List<GameObject>();
        public List<GameObject> weaponsList = new List<GameObject>();
        public static AtkManager Instance { get; private set; }
        public int groupCAtkCount;
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
        public IEnumerator CheckForAttack()
        {
            var characters = characterPool.UsePoolCharacterList();
            foreach (var atkUnit in characters
                         .Select(character => character.GetComponent<CharacterBase>())
                         .Where(atkUnit => atkUnit.unitPuzzleLevel >= 2))
            {
                StartCoroutine(AtkMotion(atkUnit));
            }
            yield return null;
        }
        private IEnumerator AtkMotion(CharacterBase unit)
        {
            atkRate = unit.defaultAtkRate * (AttackRate - EnforceManager.Instance.increaseAtkRate / 100f);
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                enemyList = unit.DetectEnemies();

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
                case CharacterBase.UnitGroups.A:
                    if (EnforceManager.Instance.divineDualAttack)
                    {
                        StartCoroutine(DivineDualAttack(new AttackData(unit, WeaponsPool.WeaponType.A)));
                    }
                    else
                    {
                        Attack(new AttackData(unit, WeaponsPool.WeaponType.A));
                    }
                    break;
                case CharacterBase.UnitGroups.B:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.B));
                    break;
                case CharacterBase.UnitGroups.C:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.C));
                    break;
                case CharacterBase.UnitGroups.E:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.E));
                    break;
                case CharacterBase.UnitGroups.H:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.H));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitGroup), unitGroup, null);
            }
        }
        private void CircleAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.D:
                    if (unit.GetComponent<CharacterBase>().CurrentWeapon == null || unit.GetComponent<CharacterBase>().CurrentWeapon.activeSelf == false)
                    {
                        unit.GetComponent<CharacterBase>().CurrentWeapon = Attack(new AttackData(unit, WeaponsPool.WeaponType.D));
                    }
                    break;
                case CharacterBase.UnitGroups.G:
                    if (unit.GetComponent<CharacterBase>().CurrentWeapon == null || unit.GetComponent<CharacterBase>().CurrentWeapon.activeSelf == false)
                    {
                        unit.GetComponent<CharacterBase>().CurrentWeapon = Attack(new AttackData(unit, WeaponsPool.WeaponType.G));
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
                case CharacterBase.UnitGroups.F:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.F));
                            break;
            }
        }
        private GameObject Attack(AttackData attackData, GameObject target = null)
        {
            var unit = attackData.Unit;
            switch (unit.GetComponent<CharacterBase>().unitGroup)
            {
               case CharacterBase.UnitGroups.A:
                   if (EnforceManager.Instance.divineFifthAttackBoost)
                   {
                       unit.GetComponent<UnitA>().atkCount++;
                   }
                   break;
               case CharacterBase.UnitGroups.B:
                   if (EnforceManager.Instance.darkFifthAttackDamageBoost)
                   {
                       unit.GetComponent<UnitB>().atkCount++;
                   }
                   break;
               case CharacterBase.UnitGroups.C:
                   if (EnforceManager.Instance.waterGlobalFreeze)
                   {
                       groupCAtkCount++;
                       if (groupCAtkCount == 100)
                       {
                           StartCoroutine(enemyPatternManager.GlobalFreezeEffect());
                       }
                   }
                   break;
               case CharacterBase.UnitGroups.D:
                   if (EnforceManager.Instance.physicalRatePerAttack)
                   {
                       groupDAtkCount++;
                       if (groupDAtkCount % 3 == 0)
                       {
                           if (GetComponent<UnitD>().groupDAtkRate < 0.6f)
                           {
                               GetComponent<UnitD>().groupDAtkRate += 0.01f;
                           }
                       }
                   }
                   break;
               case CharacterBase.UnitGroups.F:
                   if (EnforceManager.Instance.poisonDamagePerBoost)
                   {
                       groupFCount++;
                       if (groupFCount % 5 == 0 && GetComponent<UnitF>().groupFDamage < 0.6f)
                       {
                           GetComponent<UnitF>().groupFDamage += 0.01f;
                       }
                   }
                   break;
            }
            var weaponType = attackData.WeaponType;
            var weaponObject = weaponsPool.SpawnFromPool(weaponType, attackData.Unit.GetComponent<CharacterBase>().unitPuzzleLevel, unit.transform.position, unit.transform.rotation);
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

        private IEnumerator DoubleAtk(AttackData attackData)
        {
            var unit = attackData.Unit;
            var weaponType = attackData.WeaponType;
            var enemies = unit.GetComponent<UnitF>().DetectEnemies();
            if (enemies.Count == 0) yield break;
            Attack(new AttackData(unit, weaponType), enemies[0]);
            Attack(new AttackData(unit, weaponType), enemies.Count > 1 ? enemies[1] : enemies[0]);
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
                var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.GetComponent<CharacterBase>().unitPuzzleLevel, position, unit.transform.rotation);
                var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
                weaponsList.Add(weaponBase.gameObject);
                StartCoroutine(weaponBase.UseWeapon());
            }
            yield return null;
        }
        public IEnumerator SplitAttack(AttackData attackData, Vector3 enemyPosition)
        {
            var unit = attackData.Unit;
            var weaponType = attackData.WeaponType;
            var weaponDirections = new[]
            {
                new {Direction = Vector2.left, Rotation = Quaternion.Euler(0, 0, 90)},
                new {Direction = Vector2.right, Rotation = Quaternion.Euler(0, 0, -90)}
            };
            foreach (var weapon in weaponDirections)
            {
                var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.GetComponent<CharacterBase>().unitPuzzleLevel, enemyPosition, weapon.Rotation);
                var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
                weaponsList.Add(weaponBase.gameObject);
                weaponBase.Direction = weapon.Direction;
                StartCoroutine(weaponBase.UseWeapon());
            }
            yield return null;
        }
        private IEnumerator DivineDualAttack(AttackData attackData)
        {
            var unit = attackData.Unit;
            var weaponType = attackData.WeaponType;
            var unitPosition = unit.transform.position;
            var weaponDirections = new[]
            {
                new {Direction = Vector2.up, Rotation = Quaternion.identity},
                new {Direction = Vector2.down, Rotation = Quaternion.Euler(0, 0, 180)}
            };
            foreach (var weapon in weaponDirections)
            {
                var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.GetComponent<CharacterBase>().unitPuzzleLevel, unitPosition, weapon.Rotation);
                var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
                weaponsList.Add(weaponBase.gameObject);
                weaponBase.Direction = weapon.Direction;
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
    }
}