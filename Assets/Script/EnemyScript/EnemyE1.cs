using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyE1 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400;
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.E;
            RegistryType = RegistryTypes.Physics;
            base.Initialize();
        }
    }
}