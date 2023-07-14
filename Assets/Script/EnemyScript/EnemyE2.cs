using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyE2 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400;
            CrushDamage = 160;
            MoveSpeed = 1f;
            EnemyType = EnemyTypes.Group2;
            SpawnZone = SpawnZones.E;
            RegistryType = RegistryTypes.Physics;
            base.Initialize();
        }
    }
}