using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPool : MonoBehaviour
{
    [SerializeField]
    private CharacterManager characterManager;
    [SerializeField]
    private int _poolSize;
    [SerializeField]
    private SpawnManager spawnManager;
    private List<GameObject> pooledCharacters;

    void Awake()
    {
        pooledCharacters = new List<GameObject>();
        foreach (CharacterBase character in characterManager.characterList)
        {
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject obj = Instantiate(character.gameObject);
                obj.SetActive(false);
                pooledCharacters.Add(obj);
                obj.transform.SetParent(transform);
            }
        }
    }

    public GameObject GetRandomCharacterPrefab()
    {
        if (characterManager.characterList.Count > 0)
        {
            int randomIndex = Random.Range(0, characterManager.characterList.Count);
            return characterManager.characterList[randomIndex].gameObject;
        }
        return null;
    }

    public GameObject GetPooledCharacter()
    {
        List<GameObject> inactiveCharacters = new List<GameObject>();

        for (int i = 0; i < pooledCharacters.Count; i++)
        {
            if (!pooledCharacters[i].activeInHierarchy)
            {
                inactiveCharacters.Add(pooledCharacters[i]);
            }
        }

        if (inactiveCharacters.Count == 0)
        {
            return CreateNewCharacter();
        }
        else
        {
            int randomIndex = Random.Range(0, inactiveCharacters.Count);
            return inactiveCharacters[randomIndex];
        }
    }

    public List<GameObject> GetPooledCharacters()
    {
        return pooledCharacters;
    }

    public GameObject CreateNewCharacter()
    {
        GameObject newCharacter = Instantiate(GetRandomCharacterPrefab());
        newCharacter.SetActive(false);
        pooledCharacters.Add(newCharacter);
        newCharacter.transform.SetParent(transform);
        return newCharacter;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        spawnManager.RespawnCharacters();
    }

    public int GetActiveCharacterCount()
    {
        return pooledCharacters.Count(character => character.activeInHierarchy);
    }

    public GameObject GetRandomInactiveCharacter()
    {
        List<GameObject> inactiveCharacters = pooledCharacters.Where(character => !character.activeInHierarchy).ToList();

        if (inactiveCharacters.Count > 0)
        {
            int randomIndex = Random.Range(0, inactiveCharacters.Count);
            return inactiveCharacters[randomIndex];
        }

        return null;
    }

}
