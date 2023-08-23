using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Scout : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.Scout;
            enemyDesc = "Scouts perform dangerous missions but are paid less for it.";
            base.Initialize();
        }
    }
}

