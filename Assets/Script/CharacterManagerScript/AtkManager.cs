using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private EnforceManager enforceManager;
        [SerializeField] private GameManager gameManager;
        private const float AttackRate = 2f;
        public List<GameObject> enemyList = new List<GameObject>();
        public List<GameObject> weaponsList = new List<GameObject>();
        public static AtkManager Instance { get; private set; }

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
            var atkRate = unit.defaultAtkRate * (AttackRate - EnforceManager.Instance.increaseAtkRate / 100f);
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                enemyList = unit.DetectEnemies();

                if (enemyList.Count > 0)
                {
                    var atkUnit = unit.gameObject; 
                    var unitAtkType = unit.UnitAtkType;
                    var unitGroup = unit.unitGroup; 
                    switch (unitAtkType)
                    {
                        case CharacterBase.UnitAtkTypes.Projectile:
                           ProjectileAttack(atkUnit, unitGroup);
                            break;
                        case CharacterBase.UnitAtkTypes.Gas:
                           GasAttack(atkUnit, unitGroup);
                            break;
                        case CharacterBase.UnitAtkTypes.Circle:
                            CircleAttack(atkUnit, unitGroup);
                            break;
                        case CharacterBase.UnitAtkTypes.GuideProjectile:
                            GuideProjectileAttack(atkUnit, unitGroup);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Spear));
                    break;
                case CharacterBase.UnitGroups.C:
                    if (EnforceManager.Instance.water2AdditionalProjectile)
                    {
                        StartCoroutine(TripleAttack(new AttackData(unit, WeaponsPool.WeaponType.IceCrystal)));
                    }
                    else
                    {
                        Attack(new AttackData(unit, WeaponsPool.WeaponType.IceCrystal));
                    }
                    break;
                case CharacterBase.UnitGroups.E:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.IceCrystal)); 
                    break;
                case CharacterBase.UnitGroups.H:
                    if (EnforceManager.Instance.physics2AdditionalProjectile)
                    {
                        StartCoroutine(TripleAttack(new AttackData(unit, WeaponsPool.WeaponType.Dart)));
                    }
                    else
                    {
                        Attack(new AttackData(unit, WeaponsPool.WeaponType.Dart));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitGroup), unitGroup, null);
            }
        }
        private void GasAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.F:
                    if (enforceManager.poisonDoubleAtk)
                    {
                        StartCoroutine(GasDoubleAtk(unit, 0.3f));
                    }
                    else
                    {
                       Attack(new AttackData(unit, WeaponsPool.WeaponType.VenomSac));
                    }
                    break;
                case CharacterBase.UnitGroups.G:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.FireBall));
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
                        unit.GetComponent<CharacterBase>().CurrentWeapon = Attack(new AttackData(unit, WeaponsPool.WeaponType.Sword)); // Perform attack with a sword for group D
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
                case CharacterBase.UnitGroups.B:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Dark));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitGroup), unitGroup, null);
            }
        }

        private GameObject Attack(AttackData attackData)
        {
            var unit = attackData.Unit;
            var weaponType = attackData.WeaponType; 
            var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.transform.position, unit.transform.rotation);
            var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
            if (weaponBase == null) return null;
            weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
            weaponsList.Add(weaponBase.gameObject);
            StartCoroutine(weaponBase.UseWeapon());
            weaponsPool.SetSprite(weaponType, attackData.Unit.GetComponent<CharacterBase>().unitPuzzleLevel, weaponObject);
            return weaponObject;
        }

        private IEnumerator GasDoubleAtk(GameObject unit,float atkDuration)
        {
            Attack(new AttackData(unit, WeaponsPool.WeaponType.VenomSac));
            yield return new WaitForSeconds(atkDuration);
            Attack(new AttackData(unit, WeaponsPool.WeaponType.VenomSac));
        }

        private IEnumerator TripleAttack(AttackData attackData)
        {
            var unit = attackData.Unit;
            var weaponType = attackData.WeaponType; 
            const float offset = 0.7f;
            var unitPosition = unit.transform.position;
            var weaponPositions = new []
            {
                new Vector3(unitPosition.x - offset, unitPosition.y, 0),
                unitPosition,
                new Vector3(unitPosition.x + offset, unitPosition.y, 0)
            };
            foreach (var position in weaponPositions)
            {
                var weaponObject = weaponsPool.SpawnFromPool(weaponType, position, unit.transform.rotation);
                var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
                weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
                weaponsList.Add(weaponBase.gameObject);
                StartCoroutine(weaponBase.UseWeapon());
                weaponsPool.SetSprite(weaponType, unit.GetComponent<CharacterBase>().unitPuzzleLevel, weaponObject);
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