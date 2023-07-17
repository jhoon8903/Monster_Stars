using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyF3 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400f;
            CrushDamage = 160;
            moveSpeed = 1f;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group3;
            SpawnZone = SpawnZones.F;
            RegistryType = RegistryTypes.Burn;
            base.Initialize();
        }
    }
}