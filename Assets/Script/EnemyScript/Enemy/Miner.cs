using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Miner : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.Miner;
            enemyDesc = "With beer and pickaxes, there's nothing that Dwarves miners can't accomplish.";
            base.Initialize();
        }
    }
}