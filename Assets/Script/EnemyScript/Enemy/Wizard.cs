using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Wizard : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Burn;
            enemyClass = EnemyClasses.Wizard;
            enemyDesc = "Due to their long lifespan, wizards have access to many different types of magic.";
            base.Initialize();
        }
    }
}