using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyB3 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 550;
            CrushDamage = 200;
            moveSpeed = 1f;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group3;
            SpawnZone = SpawnZones.B;
            RegistryType = RegistryTypes.Water;
            base.Initialize();
        }
    }
}