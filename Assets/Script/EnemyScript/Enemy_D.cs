using System;
using Script.EnemyManagerScript;

namespace Script.EnemyScript
{
    public class Enemy_D : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "D";
            HealthPoint = 100;
            MoveSpeed = 1.0f;
            EnemyType = EnemyTypes.Normal;
            SpawnZone = EnemyZone.D;
        }
    }
}