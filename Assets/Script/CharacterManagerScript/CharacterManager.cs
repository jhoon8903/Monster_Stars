using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public List<CharacterBase> characterGroup;

    public GameObject GetRandomCharacterPrefab()
    {
        if (characterGroup.Count > 0)
        {
            int randomIndex = Random.Range(0, characterGroup.Count);
            return characterGroup[randomIndex].gameObject;
        }
        return null;
    }
}
