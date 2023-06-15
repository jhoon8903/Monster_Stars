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
        public bool PhysicAdditionalWeapon { get; set; } = false;
        private Transform _mainWeapon;
        private Transform _secondWeapon;
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

            // Find the main weapon and the second weapon in the hierarchy
            _mainWeapon = objectToSpawn.transform.Find("FirstSword"); // Replace with the actual name of your main weapon
            _secondWeapon = objectToSpawn.transform.Find("SecondSword"); // Replace with the actual name of your second weapon

            if (_mainWeapon != null)
            {
                _mainWeapon.gameObject.SetActive(true);
            }

            // Only try to activate the second weapon if the weapon type is Sword
            if (weaponType == WeaponType.Sword && _secondWeapon != null)
            {
                _secondWeapon.gameObject.SetActive(PhysicAdditionalWeapon);
            }

            _poolDictionary[weaponType].Enqueue(objectToSpawn);
            objectToSpawn.transform.localScale = InitLocalScale;
            objectToSpawn.SetActive(true);

            return objectToSpawn;
        }

        public void SetSprite(WeaponType weaponType, int level, GameObject weaponObject)
        {
            var weapon = weapons.Find(w => w.weaponType == weaponType);

            if (level - 1 < weapon.weaponSprite.Count)
            {
                var spriteRenderer = weaponObject.GetComponentInChildren<SpriteRenderer>();
                // ReSharper disable once Unity.NoNullPropagation
                var spriteRendererSecond = _secondWeapon?.GetComponentInChildren<SpriteRenderer>();

                spriteRenderer.sprite = weapon.weaponSprite[level - 1];

                if (spriteRendererSecond != null && PhysicAdditionalWeapon)
                {
                    spriteRendererSecond.sprite = weapon.weaponSprite[level - 1];
                }
            }
        }

        public static void ReturnToPool(GameObject weapon)
        {
            weapon.SetActive(false);
        }
    }  
}

