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
        [SerializeField] SwipeManager swipeManager;

        private void Start()
        {
            swipeManager.isBusy = true;
            countManager.Initialize(moveCount);
            gridManager.GenerateInitialGrid();
            StartCoroutine(StartMatchesThenCheck());
            swipeManager.isBusy = false;
        }

        private IEnumerator StartMatchesThenCheck()
        {
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject());
            yield return null;
        }
    }
}