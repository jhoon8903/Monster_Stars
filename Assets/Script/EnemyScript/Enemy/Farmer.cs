using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Farmer : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.Farmer;
            enemyDesc = "The peasant was mobilized by order of the nobleman, but failed to bring his weapon.";
            base.Initialize();
        }
    }
}

