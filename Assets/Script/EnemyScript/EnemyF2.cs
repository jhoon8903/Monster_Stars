using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyF2 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400f;
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group2;
            SpawnZone = SpawnZones.F;
            RegistryType = RegistryTypes.Burn;
            base.Initialize();
        }
    }
}