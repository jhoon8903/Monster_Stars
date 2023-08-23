using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Quarreller : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Physics;
            enemyClass = EnemyClasses.Quarreller;
            enemyDesc =
                "Quarrellers claim that their weapons are better than bows, but it is questionable whether they are better at capturing castles.";
            base.Initialize();
        }
    }
}