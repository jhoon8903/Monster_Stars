using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public sealed class CharacterPool : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private int poolSize;
        public List<GameObject> pooledCharacters;
        public bool theFirst;
        public static CharacterPool Instance;

        public void Awake()
        {
            Instance = this;
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
            var notUsedPoolList = pooledCharacters.Where(t => !t.activeInHierarchy).ToList();
            if(theFirst)
            {
                var noneGroupList = notUsedPoolList.Where(t => t.GetComponent<CharacterBase>().unitGroup == CharacterBase.UnitGroups.None && Random.value < 0.08f);
                var nonNoneGroupList = notUsedPoolList.Where(t => t.GetComponent<CharacterBase>().unitGroup != CharacterBase.UnitGroups.None && Random.value < 0.23f);
                notUsedPoolList = noneGroupList.Concat(nonNoneGroupList).ToList();
            }
            theFirst = true;
            return notUsedPoolList;
        }

        public List<GameObject> UsePoolCharacterList()
        {
            return pooledCharacters.Where(t => t.activeInHierarchy).ToList();
        }

        public List<GameObject> SortPoolCharacterList()
        {
            var sortedList = UsePoolCharacterList()
                .OrderBy(t => t.transform.position.y)
                .ThenBy(t => t.transform.position.x)
                .ToList();
            return sortedList;
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