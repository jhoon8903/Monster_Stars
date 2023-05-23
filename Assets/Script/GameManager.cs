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
        [SerializeField] 
        private SwipeManager swipeManager;

        private void Start()
        {
            swipeManager.isBusy = true;
            countManager.Initialize(moveCount);  // 이동 횟수 초기화
            gridManager.GenerateInitialGrid();  // 초기 그리드 생성
            StartCoroutine(StartMatchesThenCheck());  // 매치 시작 후 확인
            swipeManager.isBusy = false;
        }

        private IEnumerator StartMatchesThenCheck()
        {
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject());  // 캐릭터 객체를 생성하고 위치 조정
            yield return null;
        }
    }
}