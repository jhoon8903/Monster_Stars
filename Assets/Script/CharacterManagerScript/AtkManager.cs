using System.Linq;
using Script.WeaponScriptGroup;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class AttackData
    {
        public GameObject Unit { get; private set; }
        public WeaponsPool.WeaponType WeaponType { get; private set; }

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

        public void CheckForAttack()
        {
            var characters = characterPool.UsePoolCharacterList();
            foreach (var characterBase in from character in characters select character
                         .GetComponent<CharacterBase>() into characterBase where characterBase.Level >= 2 let enemies = characterBase
                         .DetectEnemies() where enemies.Count > 0 select characterBase)
            {
                AtkMotion(characterBase);
            }
        }

        private void AtkMotion(CharacterBase unit)
        {
            var atkUnit = unit.gameObject;
            var unitAtkType = unit.UnitAtkType;
            var unitGroup = unit.UnitGroup;
            switch (unitAtkType)
            {
                case CharacterBase.UnitAtkTypes.Projectile:
                    ProjectileAttack(atkUnit, unitGroup);
                    break;
                case CharacterBase.UnitAtkTypes.GuideProjectile:
                    GuideProjectileAttack(atkUnit, unitGroup);
                    break;
                case CharacterBase.UnitAtkTypes.Gas:
                    GasAttack(atkUnit,unitGroup);
                    break;
                case CharacterBase.UnitAtkTypes.Circle:
                    CircleAttack(atkUnit,unitGroup);
                    break;
                case CharacterBase.UnitAtkTypes.Vibrate:
                    VibrateAttack(atkUnit,unitGroup);
                    break;
                case CharacterBase.UnitAtkTypes.Boomerang:
                    BoomerangAttack(atkUnit,unitGroup);
                    break;
                case CharacterBase.UnitAtkTypes.None:
                    break;
                default:
                    Debug.Log($"unitGroup: {unitGroup} / uniAtkType: {atkUnit}");
                    break;
            }
        }

        private void Attack(AttackData attackData)
        {
            var unit = attackData.Unit;
            var weaponType = attackData.WeaponType;
            var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.transform.position, unit.transform.rotation);
            var useWeapon = weaponObject.GetComponent<WeaponBase>().UseWeapon();
            weaponsPool.SetSprite(weaponType, attackData.Unit.GetComponent<CharacterBase>().Level, weaponObject);
            StartCoroutine(useWeapon);
        }

        private void ProjectileAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.A:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Spear));
                    break;
                case CharacterBase.UnitGroups.E:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.IceCrystal));
                    break;
                default:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.None));
                    break;
            }
        }

        private void GuideProjectileAttack(GameObject unit,CharacterBase.UnitGroups unitGroup)
        {
            // Add code for GuideProjectileAttack
        }

        private void GasAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            var weaponType = unitGroup switch
            {
                CharacterBase.UnitGroups.F => WeaponsPool.WeaponType.VenomSac,
                _ => WeaponsPool.WeaponType.None
            };
            Attack(new AttackData(unit, weaponType));
        }

        private void CircleAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.D:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Sword));
                    break;
                default:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.None));
                    break;
            }
        }

        private void VibrateAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            // Add code for VibrateAttack
        }

        private void BoomerangAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            // Add code for BoomerangAttack
        }
    }
}
