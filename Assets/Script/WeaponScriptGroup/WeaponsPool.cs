using System.Collections.Generic;
using Script.RewardScript;
using UnityEngine;

namespace Script.WeaponScriptGroup
{
    public class WeaponsPool : MonoBehaviour
    {
        public enum WeaponType { None ,Spear, Sword, Dart, IceCrystal, VenomSac }
        [System.Serializable]
        public class Weapon
        {
            public WeaponType weaponType;
            public GameObject weaponPrefab;
            public List<Sprite> weaponSprite;
        }
        public List<Weapon> weapons;
        [SerializeField] private int weaponPoolCapacity = 20;
        private Dictionary<WeaponType, Queue<GameObject>> _poolDictionary;
        private static readonly Vector3 InitLocalScale = new Vector3(1f, 1f , 1f);
        private GameObject _pivotSword;
        private void Start()
        {
            _poolDictionary = new Dictionary<WeaponType, Queue<GameObject>>();
            foreach (var weapon in weapons)
            {
                var objectPool = new Queue<GameObject>();
                for (var i = 0; i < weaponPoolCapacity; i++)
                {
                    var obj = Instantiate(weapon.weaponPrefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                _poolDictionary.Add(weapon.weaponType, objectPool);
            }
        }

        public GameObject SpawnFromPool(WeaponType weaponType, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(weaponType))
            {
                return null;
            }
            var objectToSpawn = _poolDictionary[weaponType].Dequeue();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            _pivotSword = FindInChildren(objectToSpawn, "Sword(Clone)");
            if (EnforceManager.Instance.physicIncreaseWeaponScale)
            {
                if (_pivotSword != null)
                {
                    _pivotSword.transform.localScale = new Vector3(2f,1.7f,0);
                }
            }
            _poolDictionary[weaponType].Enqueue(objectToSpawn);
            objectToSpawn.SetActive(true);
            return objectToSpawn;
        }

        public void SetSprite(WeaponType weaponType, int level, GameObject weaponObject)
        {
            var weapon = weapons.Find(w => w.weaponType == weaponType);
            if (level - 1 >= weapon.weaponSprite.Count) return;
            var spriteRenderer = weaponObject.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = weapon.weaponSprite[level - 1];
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

