using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_Boss_A : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 10000;
            CrushDamage = 10000;
            MoveSpeed = 1.5f;
            EnemyType = EnemyTypes.Boss;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}

