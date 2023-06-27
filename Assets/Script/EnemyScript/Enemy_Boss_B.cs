using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_Boss_B : EnemyBase
    {
        public override void Initialize()
        {
            base.Initialize();
            healthPoint = 100000;
            CrushDamage = 10000;
            MoveSpeed = 1.5f;
            EnemyType = EnemyTypes.Boss;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.None;
        }
    }
}