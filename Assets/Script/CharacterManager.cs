using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public CharacterPool characterPool;

    public GameObject GetRandomCharacterPrefab()
    {
        int randomIndex = Random.Range(0, characterPool.characterPrefabs.Count);
        return characterPool.characterPrefabs[randomIndex];
    }
}
