using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class RoyalGuard : EnemyBase
    { 
        public override void Initialize()
        {
            CrushDamage = 130;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.RoyalGuard;
            enemyDesc = "The Royal Guard has the best skills and equipment.";
            base.Initialize();
        }
    }
}