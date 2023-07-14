using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyD1 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400;
            CrushDamage = 160;
            moveSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.D;
            RegistryType = RegistryTypes.Poison;
            base.Initialize();
        }
    }
}