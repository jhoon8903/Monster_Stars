using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public sealed class CharacterPool : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager; // Reference to the character manager
        [SerializeField] private int poolSize; // Size of the character pool
        public List<GameObject> _pooledCharacters; // List of character objects in the pool

        /**
         * Character Pool Create and SetActive(false)
         * StorageName is _pooledCharacters && Types is List<GameObject> 
         * Idling Pool wait for Calling.
         */
        public void Awake()
        {
            _pooledCharacters = new List<GameObject>();
            foreach (var character in characterManager.characterList)
            {
                for (var i = 0; i < poolSize; i++)
                {
                    var obj = Instantiate(character.gameObject, transform, true);
                    obj.SetActive(false);
                    _pooledCharacters.Add(obj);
                }
            }
        }

        /**
         * Return to SetActivate(false) CharacterObject_List 
         */
        public List<GameObject> NotUsePoolCharacterList()
        {
            return _pooledCharacters.Where(t => !t.activeSelf).ToList();
        }

        /**
         * Return to SetActivate(true) CharacterObject_List 
         */
        public List<GameObject> UsePoolCharacterList()
        {
            return _pooledCharacters.Where(t => t.activeSelf).ToList();
        }

        /**
         * Return CharacterObjectPool
         */
        public static void ReturnToPool(GameObject obj)
        {
            if (obj == null) return;
            obj.GetComponent<CharacterBase>().CharacterReset();
            obj.SetActive(false);
        }
    }
}