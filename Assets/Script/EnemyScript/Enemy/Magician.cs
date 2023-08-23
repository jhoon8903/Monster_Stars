using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Magician : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 200; 
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Water;
            enemyClass = EnemyClasses.Magician;
            enemyDesc = "Magicians never use teleport again after getting stuck in a wall.";
            base.Initialize();
        }
    }
}