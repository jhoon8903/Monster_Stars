using System;
using Script.EnemyManagerScript;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Script.RewardScript;

namespace Script.UIManager
{
      public class CastleManager : MonoBehaviour
      {
          [SerializeField] protected internal Slider hpBar;
          [SerializeField] private TextMeshProUGUI hpText;
          [SerializeField] private EnemyPool enemyPool;
          [SerializeField] private GameManager gameManager;
          [SerializeField] private WaveManager waveManager;
          [SerializeField] private EnforceManager enforceManager;
          public int hpPoint = 1000;
          public int maxHpPoint = 1000;

          public bool Damaged => hpPoint < PreviousHpPoint;
          private int PreviousHpPoint { get; set; }
          
          private void Start()
          {
              hpBar.maxValue = maxHpPoint;
              hpBar.value = hpPoint;
              UpdateHpText();
          }
      
          private void OnTriggerEnter2D(Collider2D collision)
          {
              collision.DOKill();
              if (!collision.gameObject.CompareTag("Enemy")) return;
              var enemyBase = collision.gameObject.GetComponent<EnemyBase>();
              if (enemyBase == null) return;
              hpPoint -= enemyBase.CrushDamage;
              hpBar.DOValue(hpPoint, 1.0f);
              UpdateHpText();
              waveManager.EnemyDestroyInvoke();
              enemyPool.ReturnToPool(enemyBase.gameObject);
              if (hpPoint > 0) return;
              hpPoint = 0;
              hpBar.value = hpPoint;
              UpdateHpText();
              StartCoroutine(gameManager.ContinueOrLose());
          }
          
          public void UpdatePreviousHp()
          {
              PreviousHpPoint = hpPoint;
          }
          
          private void UpdateHpText()
          {
              hpText.text = $"{hpPoint} / {maxHpPoint}";
          }

          public void IncreaseMaxHp(int increaseAmount)
          {
              maxHpPoint += increaseAmount;
              hpPoint += increaseAmount;
              if (hpPoint > maxHpPoint)
              {
                  hpPoint = maxHpPoint;
              }
              hpBar.maxValue = maxHpPoint;
              hpBar.value = hpPoint;
              UpdateHpText();
          }

          public void RecoveryCastle()
          {
              if (enforceManager.recoveryCastle && !Damaged)
              {
                  hpPoint += 200;
                  if (hpPoint > maxHpPoint)
                  {
                      hpPoint = maxHpPoint;
                  }
              }
              UpdatePreviousHp();
          }
      }
}
