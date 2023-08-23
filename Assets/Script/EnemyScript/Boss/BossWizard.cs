using Script.EnemyManagerScript;

namespace Script.EnemyScript.Boss
{
    public class BossWizard : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 10000;
            originSpeed = 0.7f;
            EnemyType = EnemyTypes.Boss;
            enemyClass = EnemyClasses.Wizard;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}