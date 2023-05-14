using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        public GridManager gridManager;
        public CountManager countManager;
        [FormerlySerializedAs("_moveCount")] public int moveCount;
        public SpawnManager spawnManager;

        private void Start()
        {
            countManager.Initialize(moveCount);
            spawnManager.SpawnCharacters();
        }
    }
}
