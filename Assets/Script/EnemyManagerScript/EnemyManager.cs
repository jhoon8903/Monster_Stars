using System.Collections.Generic;
using UnityEngine;

namespace Script.EnemyManagerScript
{
     public class EnemyManager : MonoBehaviour
     {
          [SerializeField] internal List<EnemyBase> bossList = new List<EnemyBase>();
          [SerializeField] internal List<EnemyBase> phase1EnemyList = new List<EnemyBase>();
          [SerializeField] internal List<EnemyBase> phase2EnemyList = new List<EnemyBase>();
          [SerializeField] internal List<EnemyBase> phase3EnemyList = new List<EnemyBase>();
          [SerializeField] internal List<EnemyBase> phase4EnemyList = new List<EnemyBase>();
          [SerializeField] internal List<EnemyBase> phase5EnemyList = new List<EnemyBase>();

          public List<EnemyBase> GetBossList()
          {
               return bossList;
          }

          public List<EnemyBase> GetPhase1EnemyList()
          {
               return phase1EnemyList;
          }

          public List<EnemyBase> GetPhase2EnemyList()
          {
               return phase2EnemyList;
          }

          public List<EnemyBase> GetPhase3EnemyList()
          {
               return phase3EnemyList;
          }

          public List<EnemyBase> GetPhase4EnemyList()
          {
               return phase4EnemyList;
          }

          public List<EnemyBase> GetPhase5EnemyList()
          {
               return phase5EnemyList;
          }
     }
}

