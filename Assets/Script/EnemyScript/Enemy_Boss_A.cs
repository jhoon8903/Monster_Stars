using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_Boss_A : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "BossA";
            HealthPoint = 200000;
            CrushDamage = 10000;
            MoveSpeed = 1.5f;
            EnemyType = EnemyTypes.Boss;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.None;
        }
    }
}

