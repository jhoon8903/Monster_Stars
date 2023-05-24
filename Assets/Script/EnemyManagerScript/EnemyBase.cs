using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyBase : MonoBehaviour
    {
        protected string EnemyName;
        protected int HealthPoint;
        protected float MoveSpeed;
        public enum EnemyZone { A,B,C,D,E }
        protected internal EnemyZone SpawnZone;
        public enum EnemyTypes { Boss, Normal }
        protected internal EnemyTypes EnemyType;

        protected internal virtual void EnemyProperty() { }
    }
}

