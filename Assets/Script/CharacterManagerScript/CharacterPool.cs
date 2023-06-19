using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public sealed class CharacterPool : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager; // Reference to the character manager
        [SerializeField] private int poolSize; // Size of the character pool
        public List<GameObject> pooledCharacters; // List of character objects in the pool

        public void Awake()
        {
            pooledCharacters = new List<GameObject>();
            foreach (var character in characterManager.characterList)
            {
                for (var i = 0; i < poolSize; i++)
                {
                    var obj = Instantiate(character.gameObject, transform, true);
                    obj.SetActive(false);
                    pooledCharacters.Add(obj);
                }
            }
        }

        public List<GameObject> NotUsePoolCharacterList()
        {
            return pooledCharacters.Where(t => !t.activeSelf).ToList();
        }

        public List<GameObject> UsePoolCharacterList()
        {
            return pooledCharacters.Where(t => t.activeSelf).ToList();
        }

        public static void ReturnToPool(GameObject obj)
        {
            if (obj == null) return;
            obj.GetComponent<CharacterBase>().CharacterReset();
            obj.transform.localScale = Vector3.one;
            obj.SetActive(false);
        }
    }
}