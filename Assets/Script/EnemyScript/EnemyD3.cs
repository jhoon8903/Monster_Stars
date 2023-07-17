using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyD3 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400;
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group3;
            SpawnZone = SpawnZones.D;
            RegistryType = RegistryTypes.Poison;
            base.Initialize();
        }
    }
}