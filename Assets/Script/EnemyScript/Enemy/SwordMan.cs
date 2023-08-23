using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class SwordMan : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 200;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.SwordMan;
            enemyDesc = "Swordsmen make up the largest portion of a noble's standing army.";
            base.Initialize();
        }
    }
}