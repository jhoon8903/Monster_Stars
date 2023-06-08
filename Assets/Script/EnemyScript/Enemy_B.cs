using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_B : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            HealthPoint = 550;
            CrushDamage = 200;
            MoveSpeed = 1.2f;
            EnemyType = EnemyTypes.Slow;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Divine;
        }
    }
}