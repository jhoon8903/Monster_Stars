using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        public GridManager gridManager;
        public CountManager countManager;
        public int moveCount;
        public SpawnManager spawnManager;
        [SerializeField] private MatchManager matchManager;

        private void Start()
        {
            countManager.Initialize(moveCount);
            spawnManager.SpawnCharacters();
            CheckAndHandleMatches();
        }
        
        private void CheckAndHandleMatches()
        {
            bool matchFound;

            do
            {
                matchFound = false;

                for (var x = 0; x < gridManager.gridWidth; x++)
                {
                    for (var y = 0; y < gridManager.gridHeight; y++)
                    {
                        var position = new Vector3(x, y, 0);

                        if (!matchManager.IsMatched(position)) continue;
                        // Handle the match, e.g., remove the matched pieces and spawn new ones
                        matchManager.IsMatched(position);
                        matchFound = true;
                    }
                }
            } while (matchFound);
        }
    }
}
