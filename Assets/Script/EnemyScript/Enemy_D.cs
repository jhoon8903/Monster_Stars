using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.EnemyScript
{
    public class Enemy_D : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            healthPoint = 400;
            CrushDamage = 160;
            MoveSpeed = 1f;
            EnemyType = EnemyTypes.Basic;
            SpawnZone = SpawnZones.A;
            RegistryType = RegistryTypes.Physics;
        }
    }
}