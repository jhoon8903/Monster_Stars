using System.Collections;
using Script.CharacterManagerScript;
using UnityEngine;
using UnityEngine.Serialization;

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
        private float waitTime = 1.5f;
        
        private void Start()
        {
            countManager.Initialize(moveCount);
            gridManager.GenerateInitialGrid();
            spawnManager.SpawnCharacters();
            StartCoroutine(StartMatchesThenCheck());
        }
        private IEnumerator StartMatchesThenCheck()
        {
            yield return StartCoroutine(StartMatches());
            yield return StartCoroutine(matchManager.CheckMatchesAndMoveCharacters());
        }

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