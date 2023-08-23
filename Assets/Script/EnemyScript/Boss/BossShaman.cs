using Script.EnemyManagerScript;

namespace Script.EnemyScript.Boss
{
    public class BossShaman : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 10000;
            originSpeed = 0.7f;
            EnemyType = EnemyTypes.Boss;
            enemyClass = EnemyClasses.Shaman;
            RegistryType = RegistryTypes.None;
            base.Initialize();
        }
    }
}

