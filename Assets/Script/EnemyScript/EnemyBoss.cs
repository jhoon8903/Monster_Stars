using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyBoss : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 100000f;
            CrushDamage = 100000;
            originSpeed = 0.5f;
            EnemyType = EnemyTypes.Boss;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}

