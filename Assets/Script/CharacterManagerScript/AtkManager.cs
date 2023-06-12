using System.Collections;
using System.Collections.Generic;
using Script.UIManager;
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
        [SerializeField] private WaveManager waveManager;
        public float atkRate = 3;


        public void CheckForAttack()
        {
            var characters = characterPool.UsePoolCharacterList();

            foreach (var character in characters)
            {
                var characterBase = character.GetComponent<CharacterBase>();

                if (characterBase != null && characterBase.Type == CharacterBase.Types.Character && characterBase.UnitLevel >= 2)
                {
                    characterBase.EnemyDetected += OnEnemyDetected;
                    
                }
            }
            StartCoroutine(TestAttack(characters));
        }

        IEnumerator TestAttack(List<GameObject> characters)
        {
            while (gameObject.GetComponent<GameManager>().isBattle)
            {
                foreach (var character in characters)
                {
                    yield return null;
                    character.GetComponent<CharacterBase>().DetectEnemies();
                    Debug.Log("적 감지");
                }   
            }
        }
        private void OnEnemyDetected(GameObject enemy)
        {
            var characterBase = enemy.GetComponent<CharacterBase>();

            if (characterBase != null)
            {
                AtkMotion(characterBase);
            }
        }
        private IEnumerator Attack(AttackData attackData)
        {
            while (waveManager.enemyTotalCount != 0)
            {
                var unit = attackData.Unit;
                var unitAtkRate = unit.GetComponent<CharacterBase>().defaultAtkRate;
                var weaponType = attackData.WeaponType;
                var _enemyList = unit.GetComponent<CharacterBase>().DetectEnemies();
                if (_enemyList.Count != 0)
                {
                    var weaponObject = weaponsPool.SpawnFromPool(weaponType, unit.transform.position, unit.transform.rotation);
                    var weaponBase = weaponObject.GetComponentInChildren<WeaponBase>();
                    weaponBase.InitializeWeapon(unit.GetComponent<CharacterBase>());
                    var useWeapon = weaponBase.UseWeapon();
                    weaponsPool.SetSprite(weaponType, attackData.Unit.GetComponent<CharacterBase>().UnitLevel, weaponObject);
                    StartCoroutine(useWeapon);
                    yield return new WaitForSeconds(unitAtkRate * atkRate);
                }
                else
                {
                    yield return null;
                }
            }
        }
        private void AtkMotion(CharacterBase unit)
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
                default:
                    Debug.Log($"unitGroup: {unitGroup} / uniAtkType: {atkUnit}");
                    break;
            }
        }
        private void ProjectileAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.A:
                    StartCoroutine(Attack(new AttackData(unit, WeaponsPool.WeaponType.Spear))); // Perform attack with a spear
                    break;
                case CharacterBase.UnitGroups.E:
                    StartCoroutine(Attack(new AttackData(unit, WeaponsPool.WeaponType.IceCrystal))); // Perform attack with an ice crystal
                    break;
            }
        }
        private void GasAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.F:
                    StartCoroutine(Attack(new AttackData(unit, WeaponsPool.WeaponType.VenomSac)));
                    break;
            }
        }
        private void CircleAttack(GameObject unit, CharacterBase.UnitGroups unitGroup)
        {
            switch (unitGroup)
            {
                case CharacterBase.UnitGroups.D:
                    StartCoroutine(Attack(new AttackData(unit, WeaponsPool.WeaponType.Sword))); // Perform attack with a sword for group D
                    break;
            }
        }
    }
}
