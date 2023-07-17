using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class EnemyA1 : EnemyBase
    {
        public override void Initialize()
        {
            healthPoint = 400f;
            CrushDamage = 160;
            moveSpeed = 1f;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Burn;
            base.Initialize();
        }
    }
}

