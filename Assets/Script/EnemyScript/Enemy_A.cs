using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_A : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "A";
            HealthPoint = 400;
            CrushDamage = 160;
            MoveSpeed = 1f;
            EnemyType = EnemyTypes.Basic;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Physics;
            
        }
    }
}

