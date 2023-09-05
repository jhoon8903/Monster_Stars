using Script.EnemyManagerScript;

namespace Script.EnemyScript.Boss
{
    public class RoyalGuard : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 10000;
            originSpeed = 0.7f;
            EnemyType = EnemyTypes.Boss;
            enemyClass = EnemyClasses.RoyalGuard;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}