using Script.EnemyManagerScript;

namespace Script.EnemyScript.Boss
{
    public class BossFarmer : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 10000;
            originSpeed = 0.7f;
            EnemyType = EnemyTypes.Boss;
            enemyClass = EnemyClasses.Farmer;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}