using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public sealed class CharacterPool : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private int poolSize;
        
        private SpawnManager _spawnManager;
        private List<GameObject> _pooledCharacters;
        
        /**
         * Character Pool Create and SetActive(false)
         * StorageName is _pooledCharacters && Type is List<GameObject> 
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
            obj.GetComponent<CharacterBase>().LevelReset();
            obj.transform.localScale = new Vector3(0.6f,0.6f,0.6f);
            obj.SetActive(false);
        }
    }
}
