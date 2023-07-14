using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyB2 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 550;
            CrushDamage = 200;
            moveSpeed = 1f;
            EnemyType = EnemyTypes.Group2;
            SpawnZone = SpawnZones.B;
            RegistryType = RegistryTypes.Water;
            base.Initialize();
        }
    }
}