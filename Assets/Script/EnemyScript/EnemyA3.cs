using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyA3 : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group3;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Burn;
            base.Initialize();
        }
    }
}

