using UnityEngine;

namespace Script.EnemyManagerScript
{
    public class EnemyBase : MonoBehaviour
    {
        protected string EnemyName;
        protected float HealthPoint;
        protected internal int CrushDamage;
        protected internal float MoveSpeed;
        public enum EnemyZone { A,B,C,D,E }
        protected internal EnemyZone SpawnZone;
        public enum EnemyTypes { Boss, Normal }
        protected internal EnemyTypes EnemyType;

        public enum RegistryTypes { Physics, Divine, Poison }
        protected internal RegistryTypes RegistryType;

        protected internal virtual void EnemyProperty() { }
    }
}

