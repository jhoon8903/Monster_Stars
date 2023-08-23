using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Berserker  : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Water;
            enemyClass = EnemyClasses.Berserker;
            enemyDesc = "Berserker focus on one thing at a time, and right now that thing is attacking the castle.";
            base.Initialize();
        }
    }
}