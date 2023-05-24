using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_A : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "A";
            HealthPoint = 100;
            MoveSpeed = 1.0f;
            EnemyType = EnemyTypes.Normal;
            SpawnZone = EnemyZone.A;
        }
    }
}

