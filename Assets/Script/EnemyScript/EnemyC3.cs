using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyC3 : EnemyBase
    { 
        public override void Initialize()
        {
            healthPoint = 280;
            CrushDamage = 130;
            moveSpeed = 1f;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group3;
            SpawnZone = SpawnZones.C;
            RegistryType = RegistryTypes.Physics;
            base.Initialize();
        }
    }
}