using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyF1 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400f;
            CrushDamage = 160;
            MoveSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.F;
            RegistryType = RegistryTypes.Burn;
            base.Initialize();
        }
    }
}