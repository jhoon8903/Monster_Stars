using Script.EnemyManagerScript;

namespace Script.EnemyScript.Boss
{
    public class BossBerserker : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 10000;
            originSpeed = 0.7f;
            EnemyType = EnemyTypes.Boss;
            enemyClass = EnemyClasses.Berserker;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}