using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class RuneSmith : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.RuneSmith;
            enemyDesc = "RuneSmiths can blow up castles by carving runes instead of using hammers.";
            base.Initialize();
        }
    }
}