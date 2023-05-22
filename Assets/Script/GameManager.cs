using System.Collections;
using Script.CharacterManagerScript;
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
        private MatchManager matchManager;
        [SerializeField]
        private SwipeManager swipeManager;
        [SerializeField] 
        private CharacterPool characterPool;
        [SerializeField]
        private int moveCount;
        [SerializeField] 
        private float waitTime = 2.0f;
        private bool isBusy;
        

        /**
         * If GameStart calling Method
         * 1. Count Init
         * 2. Create Grid
         * 3. Create CharacterObject
         * 4. Checking Start Match
         */
        private void Start()
        {
            if (isBusy) return;
            countManager.Initialize(moveCount);
            gridManager.GenerateInitialGrid();
            spawnManager.SpawnCharacters();
            StartCoroutine(StartMatchesThenCheck());
        }

        /**
         * Coroutine Matches
         */
        private IEnumerator StartMatchesThenCheck()
        {
            isBusy = true;
            yield return StartCoroutine(StartMatches());
            yield return StartCoroutine(matchManager.CheckMatchesAndMoveCharacters());
            isBusy = false;
            yield return null;
        }

        /**
         * All Grid Searching to Matches
         */
        private IEnumerator StartMatches()
        {
            yield return new WaitForSecondsRealtime(waitTime);
            foreach (var character in characterPool.UsePoolCharacterList())
            {
                matchManager.IsMatched(character);
            }
            yield return null;
            StartCoroutine(spawnManager.PositionUpCharacterObject());

        }

    }
}