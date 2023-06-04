using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_D : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "D";
            HealthPoint = 400;
            CrushDamage = 160;
            MoveSpeed = 1f;
            EnemyType = EnemyTypes.Basic;
            SpawnZone = SpawnZones.B;
            RegistryType = RegistryTypes.Physics;
        }
    }
}