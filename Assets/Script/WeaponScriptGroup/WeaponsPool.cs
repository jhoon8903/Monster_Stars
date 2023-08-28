using System.Collections.Generic;
using Script.CharacterManagerScript;
using Script.EnemyScript.Enemy;
using Script.RewardScript;
using UnityEngine;
using Berserker = Script.CharacterGroupScript.Berserker;

namespace Script.WeaponScriptGroup
{
    public class WeaponsPool : MonoBehaviour
    {
        public enum WeaponType { None, Octopus, Ogre, DeathChiller, Orc, Fishman, Skeleton, Phoenix, Beholder, Cobra, Berserker, Darkelf }
        [System.Serializable]
        public class Weapon
        {
            public WeaponType weaponType;
            public List<GameObject> weaponPrefabs;
        }
        public List<Weapon> weapons;
        [SerializeField] private int weaponPoolCapacity = 20;
        [SerializeField] private GameObject berserkerWeapon;
        private Dictionary<WeaponType, List<Queue<GameObject>>> _poolDictionary;
        private static readonly Vector3 InitLocalScale = new Vector3(1f, 1f , 1f);
        private GameObject _pivotDSword;
        private GameObject _berserkerWeapon;
        private void Start()
        {
            _poolDictionary = new Dictionary<WeaponType, List<Queue<GameObject>>>();
            foreach (var weapon in weapons)
            {
                var weaponPools = new List<Queue<GameObject>>();

                foreach (var weaponPrefab in weapon.weaponPrefabs)
                {
                    var objectPool = new Queue<GameObject>();
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

            for (var i = 0; i < weaponPoolCapacity; i++)
            {
                _berserkerWeapon = Instantiate(berserkerWeapon, transform);
                _berserkerWeapon.SetActive(false);
            }
        }

        public GameObject SpawnFromPool(WeaponType weaponType, CharacterBase unit, Vector3 position, Quaternion rotation)
        {
            var puzzleLevel = unit.unitPuzzleLevel;
            if (!_poolDictionary.ContainsKey(weaponType))
            {
                return null;
            }
            var objectToSpawn = _poolDictionary[weaponType][puzzleLevel-2].Dequeue();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

           
            if (EnforceManager.Instance.orcSwordScaleIncrease)
            {
                Debug.Log(puzzleLevel);
                _pivotDSword = FindInChildren(objectToSpawn, $"Orc{puzzleLevel-1}(Clone)");
                if (_pivotDSword != null)
                {
                    _pivotDSword.transform.localScale = new Vector3(1.5f,2f,1f);
                }
            }

            if (EnforceManager.Instance.berserkerThirdBoost)
            {
                if (unit.GetComponent<Berserker>().atkCount == 3 && unit.GetComponent<Berserker>())
                {
                    _berserkerWeapon.transform.position = position;
                    _berserkerWeapon.transform.rotation = rotation;
                    _berserkerWeapon.SetActive(true);
                    return _berserkerWeapon;
                }
            }
            _poolDictionary[weaponType][puzzleLevel-2].Enqueue(objectToSpawn);
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

