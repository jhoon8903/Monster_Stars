using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Druid : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Water;
            enemyClass = EnemyClasses.Druid;
            enemyDesc = "The Druids have remained neutral, not participating in the war, until their home was demolished.";
            base.Initialize();
        }
    }
}