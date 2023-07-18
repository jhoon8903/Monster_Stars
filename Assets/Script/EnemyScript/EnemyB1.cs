using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyB1 : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 200;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.B;
            RegistryType = RegistryTypes.Water;
            base.Initialize();
        }
    }
}