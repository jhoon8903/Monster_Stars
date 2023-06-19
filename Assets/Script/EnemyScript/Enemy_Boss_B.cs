using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_Boss_B : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            healthPoint = 100000;
            CrushDamage = 10000;
            MoveSpeed = 1.5f;
            EnemyType = EnemyTypes.Boss;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.None;
        }
    }
}