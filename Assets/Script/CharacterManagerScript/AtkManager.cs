using System.Linq;
using Script.WeaponScriptGroup;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class AttackData
    {
        public GameObject Unit { get; private set; } // Reference to the attacking unit
        public WeaponsPool.WeaponType WeaponType { get; private set; } // Type of the weapon used for the attack

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

        public void CheckForAttack()
        {
            var characters = characterPool.UsePoolCharacterList(); // Get the list of characters from the character pool
            foreach (var characterBase in from character in characters select character
                .GetComponent<CharacterBase>() into characterBase where characterBase.UnitLevel >= 2 let enemies = characterBase
                .DetectEnemies() where enemies.Count > 0 select characterBase)
            {
                AtkMotion(characterBase); // Trigger attack motion for the character
            }
        }

        // Perform attack motion for the given unit
        private void AtkMotion(CharacterBase unit)
        {
            var atkUnit = unit.gameObject; // Attacking unit
            var unitAtkType = unit.UnitAtkType; // Attack type of the unit
            var unitGroup = unit.unitGroup; // Group of the unit

            // Choose the attack method based on the unit's attack type
            switch (unitAtkType)
            {
                case CharacterBase.UnitAtkTypes.Projectile:
                    ProjectileAttack(atkUnit, unitGroup); // Perform projectile attack
                    break;
                case CharacterBase.UnitAtkTypes.GuideProjectile:
                    GuideProjectileAttack(atkUnit, unitGroup); // Perform guided projectile attack
                    break;
                case CharacterBase.UnitAtkTypes.Gas:
                    GasAttack(atkUnit, unitGroup); // Perform gas attack
                    break;
                case CharacterBase.UnitAtkTypes.Circle:
                    CircleAttack(atkUnit, unitGroup); // Perform circle attack
                    break;
                default:
                    Debug.Log($"unitGroup: {unitGroup} / uniAtkType: {atkUnit}");
                    break;
            }
        }

        // Perform attack using the specified weapon type for projectile attack
        private void ProjectileAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.A:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Spear)); // Perform attack with a spear
                    break;
                case CharacterBase.UnitGroups.E:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.IceCrystal)); // Perform attack with an ice crystal
                    break;
            }
        }

        // Perform guided projectile attack
        private void GuideProjectileAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            // Add code for GuideProjectileAttack
        }

        // Perform gas attack
        private void GasAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.F:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.VenomSac));
                    break;
            }
        }

        // Perform circle attack
        private void CircleAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.D:
                    Attack(new AttackData(unit, WeaponsPool.WeaponType.Sword)); // Perform attack with a sword for group D
                    break;
            }
        }

        // Perform the attack using the specified attack data
        private void Attack(AttackData attackData)
        {
            var unit = attackData.Unit; // Attacking unit
            var weaponType = attackData.WeaponType; // Type of weapon used for the attack
            var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.transform.position, unit.transform.rotation); // Get the weapon object from the weapon pool
            var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>(); // Get the weapon base component
            weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>()); // Initialize the weapon with the character's information
            var useWeapon = weaponBase.UseWeapon(); // Perform the weapon's attack logic
    
            weaponsPool.SetSprite(weaponType, attackData.Unit.GetComponent<CharacterBase>().UnitLevel, weaponObject); // Set the weapon's sprite based on the character's level
            StartCoroutine(useWeapon); // Start the attack coroutine
        }
    }
}
