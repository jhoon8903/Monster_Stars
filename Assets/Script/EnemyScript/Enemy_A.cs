using Script.EnemyManagerScript;
using UnityEngine;

namespace Script.EnemyScript
{
    public class Enemy_A : EnemyBase
    {
        protected internal override void EnemyProperty()
        {
            EnemyName = "A";
            HealthPoint = 400;
            CrushDamage = 160;
            MoveSpeed = Random.Range(13f, 17f);  // 높을 수록 느려짐
            EnemyType = EnemyTypes.Normal;
            SpawnZone = EnemyZone.A;

        }
    }
}

