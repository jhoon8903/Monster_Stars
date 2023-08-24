using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Marauder : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Physics;
            enemyClass = EnemyClasses.Marauder;
            enemyDesc = "The Marauders came here in the employ of the nobles, but it is doubtful whether they will return the castle to them.";
            base.Initialize();
        }
    }
}