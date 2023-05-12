using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        public GameObject gameObject;
        public int prefabIndex;
    }

    public List<CharacterData> characterGroup;

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
