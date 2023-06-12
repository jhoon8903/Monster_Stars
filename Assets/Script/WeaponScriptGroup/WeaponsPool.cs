using System.Collections.Generic;
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
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            _poolDictionary[weaponType].Enqueue(objectToSpawn);
            objectToSpawn.transform.localScale = InitLocalScale;
            return objectToSpawn;
        }
        public void SetSprite(WeaponType weaponType, int level, GameObject weaponObject)
        {
            var spriteRenderer = weaponObject.GetComponentInChildren<SpriteRenderer>();
            var weapon = weapons.Find(w => w.weaponType == weaponType);
        
            if (level - 2 < weapon.weaponSprite.Count)
            {
                spriteRenderer.sprite = weapon.weaponSprite[level - 2];
            }
        }
        public void ReturnToPool(GameObject weapon)
        {
            weapon.SetActive(false);
        }
    }  
}

