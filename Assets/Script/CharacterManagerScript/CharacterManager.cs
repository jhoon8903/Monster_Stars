using System.Collections.Generic;
using UnityEngine;

namespace Script.CharacterManagerScript
{
    public class CharacterManager : MonoBehaviour
    {
        public List<CharacterBase> characterList = new List<CharacterBase>();

        public List<CharacterBase> GetCharacterList()
        {
            return characterList;
        }

    }
}
