using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using Script.EnemyScript.Enemy;
using Script.RewardScript;
using UnityEngine;
using Berserker = Script.CharacterGroupScript.Berserker;

namespace Script.WeaponScriptGroup
{
    public class WeaponsPool : MonoBehaviour
    {
        public enum WeaponType { None, Octopus, Ogre, DeathChiller, Orc, Fishman, Skeleton, Phoenix, Beholder, Cobra, Berserker, DarkElf }
        [System.Serializable]
        public class Weapon
        {
            public WeaponType weaponType;
            public List<GameObject> weaponPrefabs;
        }
        public List<Weapon> weapons;
        [SerializeField] private int weaponPoolCapacity = 20;
        [SerializeField] private BerserkerWeapon berserkerWeapon;
        private Dictionary<WeaponType, List<Queue<GameObject>>> _poolDictionary;
        private static readonly Vector3 InitLocalScale = new Vector3(1f, 1f , 1f);
        private GameObject _pivotDSword;
        private BerserkerWeapon _berserkerWeapon;
        private readonly List<BerserkerWeapon> _berserkerList = new List<BerserkerWeapon>();
        private void Start()
        {
            _poolDictionary = new Dictionary<WeaponType, List<Queue<GameObject>>>();
            foreach (var weapon in weapons)
            {
                var weaponPools = new List<Queue<GameObject>>();

                foreach (var weaponPrefab in weapon.weaponPrefabs)
                {
                    var objectPool = new Queue<GameObject>();
                    if (EnforceManager.Instance.characterList.All(unit => unit.unitGroup.ToString() != weapon.weaponType.ToString())) continue;
                    for (var i = 0; i < weaponPoolCapacity; i++)
                    {
                        var obj = Instantiate(weaponPrefab, transform);
                        obj.SetActive(false);
                        objectPool.Enqueue(obj);
                    }
                    weaponPools.Add(objectPool);
                }
                _poolDictionary.Add(weapon.weaponType, weaponPools);
            }

            foreach (var unit in EnforceManager.Instance.characterList.Where(unit => unit.unitGroup == CharacterBase.UnitGroups.Berserker))
            {
                for (var i = 0; i < weaponPoolCapacity; i++)
                {
                    _berserkerWeapon = Instantiate(berserkerWeapon, transform);
                    _berserkerWeapon.gameObject.SetActive(false);
                    _berserkerList.Add(_berserkerWeapon);
                }
            }
        }

        public GameObject SpawnFromPool(WeaponType weaponType, GameObject atkUnit ,CharacterBase unit, Vector3 position, Quaternion rotation)
        {
            var puzzleLevel = unit.unitPuzzleLevel;
            if (!_poolDictionary.ContainsKey(weaponType))
            {
                return null;
            }
            var objectToSpawn = _poolDictionary[weaponType][puzzleLevel-2].Dequeue();
         

           
            if (EnforceManager.Instance.orcSwordScaleIncrease)
            {
                _pivotDSword = FindInChildren(objectToSpawn, $"Orc{puzzleLevel-1}(Clone)");
                if (_pivotDSword != null)
                {
                    _pivotDSword.transform.localScale = new Vector3(1.5f,2f,1f);
                }
            }

            if (EnforceManager.Instance.berserkerThirdBoost && unit.unitGroup == CharacterBase.UnitGroups.Berserker)
            {
                if (atkUnit.GetComponent<Berserker>().atkCount == 3)
                {
                    if (_berserkerList.Any(weaponObject => !weaponObject.gameObject.activeInHierarchy))
                    {
                       _berserkerWeapon.gameObject.SetActive(true);
                    }
                }
            }
            _poolDictionary[weaponType][puzzleLevel-2].Enqueue(objectToSpawn);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);
            return objectToSpawn;
        }

        public static void ReturnToPool(GameObject weapon)
        {
            weapon.transform.localScale = InitLocalScale;
            weapon.SetActive(false);
        }

        private static GameObject FindInChildren(GameObject parent, string name)
        {
            if (parent.name == name)
                return parent;
            foreach (Transform child in parent.transform)
            {
                if (child.name == name)
                    return child.gameObject;
        
                var result = FindInChildren(child.gameObject, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }  
}

