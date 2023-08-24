using Script.EnemyManagerScript;

namespace Script.EnemyScript.Boss
{
    public class BossMarauder : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 10000;
            originSpeed = 0.7f;
            EnemyType = EnemyTypes.Boss;
            enemyClass = EnemyClasses.Marauder;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}