using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class SpearMan : EnemyBase
    { 
        public override void Initialize()
        {
            CrushDamage = 130;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.SpearMan;
            enemyDesc = "Elven Spearman trained for over a millennium could probably beat tanks..";
            base.Initialize();
        }
    }
}