using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_B : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 550;
            CrushDamage = 200;
            MoveSpeed = 1.2f;
            EnemyType = EnemyTypes.Slow;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Divine;
            base.Initialize();
        }
    }
}