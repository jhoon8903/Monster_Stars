using System.Collections.Generic;
using UnityEngine;

public class CharacterPool : MonoBehaviour
{
    [System.Serializable]
    public class CharacterPoolItem
    {
        public GameObject characterPrefab;
        public int _count;
    }

    public List<CharacterPoolItem> poolItems;
    private List<GameObject> pooledCharacters;
    private int currentPrefabIndex = 0;

    void Awake()
    {
        pooledCharacters = new List<GameObject>();
        foreach (CharacterPoolItem item in poolItems)
        {
            for (int i = 0; i < item._count; i++)
            {
                GameObject obj = Instantiate(item.characterPrefab);
                obj.SetActive(false);
                pooledCharacters.Add(obj);
                obj.transform.SetParent(transform);
            }
        }
    }

    public GameObject GetPooledCharacter()
    {
        for (int i = 0; i < pooledCharacters.Count; i++)
        {
            if (!pooledCharacters[i].activeInHierarchy)
            {
                return pooledCharacters[i];
            }
        }

        // If no inactive characters are found, create a new one.
        return CreateNewCharacter();
    }


    public List<GameObject> GetPooledCharacters()
    {
        return pooledCharacters;
    }

    public GameObject CreateNewCharacter()
    {
        GameObject newCharacter = Instantiate(poolItems[currentPrefabIndex].characterPrefab);
        newCharacter.SetActive(false);
        pooledCharacters.Add(newCharacter);
        newCharacter.transform.SetParent(transform);
        currentPrefabIndex = (currentPrefabIndex + 1) % poolItems.Count;
        return newCharacter;
    }

}
