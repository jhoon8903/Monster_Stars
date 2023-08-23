using Script.EnemyManagerScript;

namespace Script.EnemyScript.Enemy
{
    public class Pilgrim : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 200;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Normal;
            RegistryType = RegistryTypes.Darkness;
            enemyClass = EnemyClasses.Pilgrim;
            enemyDesc = "Pilgrims get rid of everything that gets in the way of their pilgrimage.";
            base.Initialize();
        }
    }
}