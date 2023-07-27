using System.Collections.Generic;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class WeaponsPool : MonoBehaviour
    {
        public enum WeaponType { None, A, B, C, D, E, F, G, H }
        [System.Serializable]
        public class Weapon
        {
            public WeaponType weaponType;
            public List<GameObject> weaponPrefabs;
        }
        public List<Weapon> weapons;
        [SerializeField] private int weaponPoolCapacity = 20;
        private Dictionary<WeaponType, List<Queue<GameObject>>> _poolDictionary;
        private static readonly Vector3 InitLocalScale = new Vector3(1f, 1f , 1f);
        private GameObject _pivotDSword;
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
        }

        public GameObject SpawnFromPool(WeaponType weaponType, int puzzleLevel, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(weaponType))
            {
                return null;
            }
            var objectToSpawn = _poolDictionary[weaponType][puzzleLevel-2].Dequeue();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            _pivotDSword = FindInChildren(objectToSpawn, $"D{puzzleLevel-1}(Clone)");
            if (EnforceManager.Instance.physicalSwordScaleIncrease)
            {
                if (_pivotDSword != null)
                {
                    _pivotDSword.transform.localScale = new Vector3(1.5f,2f,1f);
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

