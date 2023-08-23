using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Archer : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Water;
            enemyClass = EnemyClasses.Archer;
            enemyDesc = "Elven archers are very good with bows and are said to be able to shoot with their feet.";
            base.Initialize();
        }
    }
}