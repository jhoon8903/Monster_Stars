using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyC1 : EnemyBase
    { 
        public override void Initialize()
        {
            CrushDamage = 130;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.C;
            RegistryType = RegistryTypes.Physics;
            base.Initialize();
        }
    }
}