using System.Collections;
using UnityEngine;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private CountManager countManager;
        [SerializeField]
        private GridManager gridManager;
        [SerializeField]
        private SpawnManager spawnManager;
        [SerializeField]
        private int moveCount;


        /**
         * If GameStart calling Method
         * 1. Count Init
         * 2. Create Grid
         * 3. Create CharacterObject
         * 4. Checking Start Match
         */
        private void Start()
        {
            countManager.Initialize(moveCount);
            gridManager.GenerateInitialGrid();
            StartCoroutine(StartMatchesThenCheck());
        }

        /**
         * Coroutine Matches
         */
        private IEnumerator StartMatchesThenCheck()
        {
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
        }
    }
}