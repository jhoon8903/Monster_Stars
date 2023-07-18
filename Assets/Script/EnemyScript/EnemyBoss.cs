using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyBoss : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 10000f;
            CrushDamage = 10000;
            originSpeed = 0.5f;
            EnemyType = EnemyTypes.Boss;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}

