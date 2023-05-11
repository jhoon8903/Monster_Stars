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
        foreach (GameObject character in pooledCharacters)
        {
            if (!character.activeInHierarchy)
            {
                return character;
            }
        }
        return null;
    }
}
