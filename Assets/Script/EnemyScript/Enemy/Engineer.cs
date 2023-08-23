using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Engineer : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Burn;
            enemyClass = EnemyClasses.Engineer;
            enemyDesc = "When engineers see a huge object, they want to blow it up.";
            base.Initialize();
        }
    }
}