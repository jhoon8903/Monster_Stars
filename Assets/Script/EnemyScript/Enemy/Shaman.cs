using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Shaman: EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.None;
            enemyClass = EnemyClasses.Shaman;
            enemyDesc = "Shamans strive to capture the castle according to the prophecy.";
            base.Initialize();
        }
    }
}