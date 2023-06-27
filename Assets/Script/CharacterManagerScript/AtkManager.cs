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
        public GameObject Unit { get; } // Reference to the attacking unit
        public WeaponsPool.WeaponType WeaponType { get;} // Type of the weapon used for the attack

        public AttackData(GameObject unit, WeaponsPool.WeaponType weaponType)
        {
            Unit = unit;
            WeaponType = weaponType;
        }
    }

    public class AtkManager : MonoBehaviour
    {
        [SerializeField] private CharacterPool characterPool; // Reference to the character pool
        [SerializeField] private WeaponsPool weaponsPool; // Reference to the weapon pool
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
                         .Where(atkUnit => atkUnit.UnitLevel >= 2))
            {
                StartCoroutine(AtkMotion(atkUnit));
            }
            yield return null;
        }
        private IEnumerator AtkMotion(CharacterBase unit)
        {
            var atkRate = unit.GetComponent<CharacterBase>().defaultAtkRate * (AttackRate - EnforceManager.Instance.increaseAtkRate / 100f);
            while (gameManager.IsBattle)
            {
                yield return StartCoroutine(gameManager.WaitForPanelToClose());
                enemyList = unit.DetectEnemies();

                if (enemyList.Count > 0)
                {
                    var atkUnit = unit.gameObject; // Attacking unit
                    var unitAtkType = unit.UnitAtkType; // Attack type of the unit
                    var unitGroup = unit.unitGroup; // Group of the unit
                    switch (unitAtkType)
                    {
                        case CharacterBase.UnitAtkTypes.Projectile:
                           ProjectileAttack(atkUnit, unitGroup); // Perform projectile attack
                            break;
                        case CharacterBase.UnitAtkTypes.Gas:
                           GasAttack(atkUnit, unitGroup); // Perform gas attack
                            break;
                        case CharacterBase.UnitAtkTypes.Circle:
                            CircleAttack(atkUnit, unitGroup); // Perform circle attack
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
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Spear)); // Perform attack with a spear
                    break;
                case CharacterBase.UnitGroups.B:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Dark));
                    break;
                case CharacterBase.UnitGroups.C:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.IceCrystal));
                    break;
                case CharacterBase.UnitGroups.E:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.IceCrystal)); // Perform attack with an ice crystal
                    break;
                case CharacterBase.UnitGroups.H:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Dart));
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
                    if (enforceManager.poisonDoubleAtk )
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

        private GameObject Attack(AttackData attackData)
        {
            var unit = attackData.Unit; // Attacking unit
            var weaponType = attackData.WeaponType; // Type of weapon used for the attack
            var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.transform.position, unit.transform.rotation); // Get the weapon object from the weapon pool
            var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>(); // Get the weapon base component
            weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>()); // Initialize the weapon with the character's information
            weaponsList.Add(weaponBase.gameObject);
            StartCoroutine(weaponBase.UseWeapon()); // Perform the weapon's attack logic
            weaponsPool.SetSprite(weaponType, attackData.Unit.GetComponent<CharacterBase>().UnitLevel, weaponObject); // Set the weapon's sprite based on the character's level
            
            return weaponObject;
        }

        private IEnumerator GasDoubleAtk(GameObject unit,float atkDuration)
        {
            Attack(new AttackData(unit, WeaponsPool.WeaponType.VenomSac));
            yield return new WaitForSeconds(atkDuration);
            Attack(new AttackData(unit, WeaponsPool.WeaponType.VenomSac));
        }

        public void ClearWeapons()
        {
            if (weaponsList.Count <= 0) return;
            var weaponsListCopy = weaponsList.ToList();
            foreach (var weaponBase in weaponsListCopy
                         .Select(weapon => weapon.GetComponent<WeaponBase>())
                         .Where(weaponBase => weaponBase.isInUse))
            {
                weaponBase.StopUseWeapon(weaponBase.gameObject);
            }
        }
    }
}