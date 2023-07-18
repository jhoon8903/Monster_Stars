using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyBoss : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 10000;
            originSpeed = 0.35f;
            EnemyType = EnemyTypes.Boss;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}

