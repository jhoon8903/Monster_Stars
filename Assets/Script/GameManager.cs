using System.Collections;
using Script.CharacterManagerScript;
using Script.EnemyManagerScript;
using Script.UIManager;
using UnityEngine.SceneManagement;
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
        [SerializeField] private EnemyPatternManager enemyPatternManager;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private AtkManager atkManager;
        [SerializeField] private float checkInterval = 1.0f;

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

        public IEnumerator Count0Call()
        {
            cameraManager.CameraSizeChange();
            backgroundManager.ChangeSize();
            yield return StartCoroutine(enemySpawnManager.SpawnEnemies());
            // 중간 대기시간을 주는 방법이 필요
            yield return new WaitForSecondsRealtime(1.5f);
            StartCoroutine(enemyPatternManager.Zone_Move());
            while (enemySpawnManager.FieldList.Count > 0)
            {
                atkManager.CheckForAttack();
                yield return new WaitForSeconds(checkInterval); 
            }

            if (enemySpawnManager.FieldList.Count == null || enemySpawnManager.FieldList.Count == 0)
            {
                // enemySpawnManager.FieldList.Count가 0이 되면 다음 스테이지 진행 및 초기화 호출
                // 다음 로직을 호출하시면 됩니다.
            }
        }

        public void LoseGame()
        {
            Time.timeScale = 0;
            gamePanel.SetActive(true);
            Debug.Log("게임종료");
            

        }

        private void NextStage()
        {
            Debug.LogWarning("다음 스테이지 진행");
        }

        public void RetryGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}