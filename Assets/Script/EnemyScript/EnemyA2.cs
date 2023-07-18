using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyA2 : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group2;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Burn;
            base.Initialize();
        }
    }
}

