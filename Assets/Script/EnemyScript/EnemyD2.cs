using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyD2 : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group2;
            SpawnZone = SpawnZones.D;
            RegistryType = RegistryTypes.Poison;
            base.Initialize();
        }
    }
}