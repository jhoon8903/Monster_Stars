using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_C : EnemyBase
    { 
        protected internal override void EnemyProperty()
        {
            healthPoint = 280;
            CrushDamage = 130;
            MoveSpeed = 0.8f;
            EnemyType = EnemyTypes.Fast;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Poison;
        }
    }
}