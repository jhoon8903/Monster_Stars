using System;
using Script.UIManager;
using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyBase : MonoBehaviour
    {
        [SerializeField] private EnemySpawnManager enemySpawnManager;
        [SerializeField] private ExpManager expManager;
        public enum KillReasons { ByPlayer, ByCastle }
        public event Action<KillReasons> OnEnemyKilled;
        protected string EnemyName;  // 적 고유 이름

        public int number = 0;

        protected internal float HealthPoint; // 적 오브젝트의 체력
        
        protected internal int CrushDamage;   // 충돌시 데미지
        protected internal float MoveSpeed;   // 적 오브젝트의 이동속도, 1f 는 1초에 1Grid를 가는 속도 숫자가 커질수록 느려져야 함
        
        public enum EnemyTypes { Boss, Basic, Slow, Fast }
        protected internal EnemyTypes EnemyType; // 적 타입 빠른적, 느린적, 보통적, 보스
        
        public enum RegistryTypes { Physics, Divine, Poison, None }
        protected internal RegistryTypes RegistryType; // 저항타입, 만약 공격하는 적이 해당 타입과 일치하면 20%의 데미지를 덜 입게 됨

        public enum SpawnZones { A, B, C, D, E }
        protected internal SpawnZones SpawnZone;

        protected internal virtual void EnemyProperty() { }

        public void ReceiveDamage(float damage, KillReasons reason = KillReasons.ByPlayer)
        {
            HealthPoint -= damage;
            if (!(HealthPoint <= 0)) return;
            FindObjectOfType<EnemyPool>().ReturnToPool(gameObject);
            OnEnemyKilled?.Invoke(reason);
            if (ExpManager.Instance != null)
            {
                ExpManager.Instance.HandleEnemyKilled(reason);
            }
        }

        public void DecreaseMoveSpeed(int decreaseAmount)
        {
            var percentageIncrease = (float)decreaseAmount / 100;
            MoveSpeed *= decreaseAmount * percentageIncrease;
        }
    }
}