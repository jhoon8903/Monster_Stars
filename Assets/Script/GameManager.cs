using System.Collections;
using UnityEngine;

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
            // StartCoroutine(PerformMatchHandling());
        }

        // private IEnumerator PerformMatchHandling()
        // {
        //     yield return null; 
        //
        //     bool matchFound;
        //
        //     do
        //     {
        //         matchFound = false;
        //
        //         for (var x = 0; x < gridManager.gridWidth; x++)
        //         {
        //             for (var y = 0; y < gridManager.gridHeight; y++)
        //             {
        //                 var characterPositions = new Vector3(x, y, 0);
        //                 var characterObject = spawnManager.GetCharacterAtPosition(characterPositions);
        //                 var position = characterObject.transform.position;
        //                 var characterObjectPositions = new Vector3(position.x, position.y, 0);
        //                 if (matchManager.IsMatched(characterObject, characterObjectPositions))
        //                 {
        //                     matchFound = true;
        //                 }
        //             }
        //         }
        //
        //         yield return null; // Wait for a frame before checking again
        //     } while (matchFound);
        // }
    }
}