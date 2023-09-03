using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.PuzzleManagerGroup;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public sealed class CharacterPool : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private int poolSize;
        [SerializeField] private SpawnManager spawnManager;
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

        public static void ReturnToPool(GameObject obj)
        {
            if (obj == null) return;
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            spriteRenderer.DOFade(0, 0.6f).OnComplete(() =>
            {
                obj.GetComponent<CharacterBase>().CharacterReset();
                obj.transform.localScale = Vector3.one;
                var color = spriteRenderer.color;
                color = new Color(color.r, color.g, color.b, 1);
                spriteRenderer.color = color;
                obj.SetActive(false);
            });
        }
    }
}