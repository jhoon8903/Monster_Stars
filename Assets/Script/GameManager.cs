using System.Collections;
using Script.EnemyManagerScript;
using UnityEngine;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private CountManager countManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private int moveCount;
        [SerializeField] private SwipeManager swipeManager;
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private BackGroundManager backgroundManager;
        [SerializeField] private EnemySpawnManager enemySpawnManager;

        private void Start()
        {
            swipeManager.isBusy = true;
            countManager.Initialize(moveCount); // 이동 횟수 초기화
            gridManager.GenerateInitialGrid(); // 초기 그리드 생성
            StartCoroutine(StartMatchesThenCheck()); // 매치 시작 후 확인
            swipeManager.isBusy = false;
        }

        private IEnumerator StartMatchesThenCheck()
        {
            yield return StartCoroutine(spawnManager.PositionUpCharacterObject()); // 캐릭터 객체를 생성하고 위치 조정
            yield return null;
        }

        public void Count0Call()
        {
            cameraManager.CameraSizeChange();
            backgroundManager.ChangeSize();
            // 중간 대기시간을 주는 방법이 필요
            StartCoroutine(enemySpawnManager.SpawnEnemies());
            if (enemySpawnManager.fieldList.Count <= 0)
            {
                // 적 리스트.Count = 0 일때 다음 게임 시작 메소드 필요
            }
        }
    }
}