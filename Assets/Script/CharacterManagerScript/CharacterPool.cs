using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
        public int callCount = 0;
        public List<GameObject> UsePoolCharacterList()
        {
            Debug.Log("callCount = " + ++callCount);
            return pooledCharacters.Where(t => t.activeSelf).ToList();
        }

        public List<GameObject> SortPoolCharacterList()
        {
            List<GameObject> sortedList = pooledCharacters
                .OrderByDescending(t => t.transform.position.y)
                .ThenBy(t => t.transform.position.x)
                .ToList();

            return sortedList;
        }

        public void ListInspector()
        {
            foreach (GameObject item in SortPoolCharacterList())
            {
                Debug.Log("item : " + item + " (" + item.transform.position.x + ", " + item.transform.position.y + ")");
            }
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