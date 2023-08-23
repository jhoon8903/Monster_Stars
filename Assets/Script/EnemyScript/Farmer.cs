using Script.EnemyManagerScript;
using UnityEngine;
using UnityEngine.UI;

namespace Script.EnemyScript
{
    public class Farmer : EnemyBase
    {
        public override void Initialize()
        {
            CrushDamage = 160;
            originSpeed = 1f;
            EnemyType = EnemyTypes.Group1;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Burn;
            enemyClass = EnemyClasses.Farmer;
            base.Initialize();
        }
    }
}

