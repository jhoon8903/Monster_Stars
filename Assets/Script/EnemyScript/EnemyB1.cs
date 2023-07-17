using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyB1 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 550;
            CrushDamage = 200;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.B;
            RegistryType = RegistryTypes.Water;
            base.Initialize();
        }
    }
}