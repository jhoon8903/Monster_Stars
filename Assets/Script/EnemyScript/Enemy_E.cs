using System;
using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_E : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "E";
            HealthPoint = 100;
            MoveSpeed = 1.0f;
            EnemyType = EnemyTypes.Normal;
            SpawnZone = EnemyZone.E;
        }
    }
}