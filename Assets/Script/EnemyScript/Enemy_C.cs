using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_C : EnemyBase
    { 
        protected internal override void EnemyProperty()
        {
            EnemyName = "C";
            HealthPoint = 100;
            MoveSpeed = 1.0f;
            EnemyType = EnemyTypes.Normal;
            SpawnZone = EnemyZone.C;
        }
    }
}