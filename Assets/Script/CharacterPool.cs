using System.Collections.Generic;
using UnityEngine;

public class CharacterPool : MonoBehaviour
{
    public List<GameObject> characterPrefabs;
    public int initialPoolSize = 10;
    private List<GameObject> pooledObjects;

    void Awake()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            for (int j = 0; j < initialPoolSize; j++)
            {
                AddObjectToPool(characterPrefabs[i]);
            }
        }
    }

    private GameObject AddObjectToPool(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy && obj.name.StartsWith(prefab.name))
            {
                return obj;
            }
        }

        return AddObjectToPool(prefab);
    }
}
