using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_E : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400;
            CrushDamage = 160;
            MoveSpeed = 1f;
            EnemyType = EnemyTypes.Slow;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Physics;
            base.Initialize();
        }
    }
}