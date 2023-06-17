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
        private Transform _mainWeapon;
        private Transform _secondWeapon;
        private Transform _pivotSword;
        [SerializeField] private EnforceManager enforceManager;
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
            if (enforceManager.physicIncreaseWeaponScale)
            {
                _pivotSword = objectToSpawn.transform.Find("Sword");
                _pivotSword.transform.localScale = new Vector3(2f,1.7f,0);
            }
            _mainWeapon = objectToSpawn.transform.Find("FirstSword"); // Replace with the actual name of your main weapon
            _secondWeapon = objectToSpawn.transform.Find("SecondSword"); // Replace with the actual name of your second weapon
            if (_mainWeapon != null)
            {
                _mainWeapon.gameObject.SetActive(true);
            }
            if (weaponType == WeaponType.Sword && _secondWeapon != null)
            {
                _secondWeapon.gameObject.SetActive(enforceManager.physicAdditionalWeapon);
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
            // ReSharper disable once Unity.NoNullPropagation
            var spriteRendererSecond = _secondWeapon?.GetComponentInChildren<SpriteRenderer>();

            spriteRenderer.sprite = weapon.weaponSprite[level - 1];

            if (spriteRendererSecond != null && enforceManager.physicAdditionalWeapon)
            {
                spriteRendererSecond.sprite = weapon.weaponSprite[level - 1];
            }
        }

        public static void ReturnToPool(GameObject weapon)
        {
            weapon.transform.localScale = InitLocalScale;
            weapon.SetActive(false);
        }
    }  
}

