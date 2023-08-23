using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class WarDancer : EnemyBase
    { 
        public override void Initialize()
        {
            CrushDamage = 130;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Poison;
            enemyClass = EnemyClasses.WarDancer;
            enemyDesc = "The dance of the WarDancers is said to be very beautiful, but no one who saw it survived.";
            base.Initialize();
        }
    }
}