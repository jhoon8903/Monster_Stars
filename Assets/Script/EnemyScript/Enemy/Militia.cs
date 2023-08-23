using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Militia : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.Militia;
            enemyDesc = "Militias were simply mobilized at the behest of the nobility.";
            base.Initialize();
        }
    }
}

