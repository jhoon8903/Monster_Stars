using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Pirate : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.Pirate;
            enemyDesc = "The pirate got tired of being on the ship and started a new *business* on land.";
            base.Initialize();
        }
    }
}