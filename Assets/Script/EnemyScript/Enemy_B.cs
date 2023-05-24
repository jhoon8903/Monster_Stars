using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_B : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "B";
            HealthPoint = 100;
            MoveSpeed = 1.0f;
            EnemyType = EnemyTypes.Normal;
            SpawnZone = EnemyZone.B;
        }
    }
}