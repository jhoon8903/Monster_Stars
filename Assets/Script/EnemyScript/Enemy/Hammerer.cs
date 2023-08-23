using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Hammerer : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.Hammerer;
            enemyDesc = "Hammerers have proven their skill and valor on countless battlefields.";
            base.Initialize();
        }
    }
}