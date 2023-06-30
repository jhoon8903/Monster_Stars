using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_D : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400;
            CrushDamage = 160;
            MoveSpeed = 1f;
            EnemyType = EnemyTypes.BasicD;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Poison;
            base.Initialize();
        }
    }
}