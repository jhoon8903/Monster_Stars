using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Dryad : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.Dryad;
            enemyDesc = "The Druids have remained neutral, not participating in the war, until their home was demolished.";
            base.Initialize();
        }
    }
}