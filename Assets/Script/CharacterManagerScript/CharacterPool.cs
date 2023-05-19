using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.CharacterManagerScript
{
    public sealed class CharacterPool : MonoBehaviour
    {
        [SerializeField]
        private CharacterManager characterManager;
        [FormerlySerializedAs("_poolSize")] [SerializeField]
        private int poolSize;
        [SerializeField]
        private SpawnManager spawnManager;
        private List<GameObject> _pooledCharacters;
        
        // 풀 초기 설정 활성화
        public void CreateCharacterPool()
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
        
        // 비활성화 된 CharacterObject를 반환하거나, 생성 
        public GameObject GetPooledCharacter()
        {
            var inactiveCharacters = _pooledCharacters.Where(t => !t.activeInHierarchy).ToList();
    
            if (inactiveCharacters.Count <= 0) return null;
            var randomIndex = Random.Range(0, inactiveCharacters.Count);
            return inactiveCharacters[randomIndex];
        }
    
        // Pool에서 모든 케릭터를 반환함
        public List<GameObject> GetPooledCharacters(bool onlyInactive = false)
        {
            return onlyInactive 
                ? _pooledCharacters.Where(t => !t.activeInHierarchy).ToList() 
                : new List<GameObject>(_pooledCharacters);
        }

        // 반환되는 CharacterObject를 Pool에 반환함
        internal static IEnumerator ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            yield return null;
        }
        

    }
}
