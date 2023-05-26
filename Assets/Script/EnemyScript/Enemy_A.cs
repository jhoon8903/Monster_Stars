using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.EnemyScript
{
    public class Enemy_A : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "A";
            HealthPoint = 100;
            CrushDamage = 100;
            MoveSpeed = Random.Range(15f, 20f);  // 높을 수록 느려짐
            EnemyType = EnemyTypes.Normal;
            SpawnZone = EnemyZone.A;
        }
    }
}

