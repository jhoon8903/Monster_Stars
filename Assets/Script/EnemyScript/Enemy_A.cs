using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_A : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400f;
            CrushDamage = 160;
            MoveSpeed = 1f;
            EnemyType = EnemyTypes.BasicA;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Burn;
            base.Initialize();
        }
    }
}

