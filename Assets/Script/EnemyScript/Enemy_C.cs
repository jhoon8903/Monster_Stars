using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_C : EnemyBase
    { 
        public override void Initialize()
        {
            healthPoint = 280;
            CrushDamage = 130;
            MoveSpeed = 0.8f;
            EnemyType = EnemyTypes.Fast;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Physics;
            base.Initialize();
        }
    }
}