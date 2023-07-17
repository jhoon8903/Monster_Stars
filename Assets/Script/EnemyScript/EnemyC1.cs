using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyC1 : EnemyBase
    { 
        public override void Initialize()
        {
            healthPoint = 280;
            CrushDamage = 130;
            moveSpeed = 1f;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.C;
            RegistryType = RegistryTypes.Physics;
            base.Initialize();
        }
    }
}