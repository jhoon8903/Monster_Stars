using Script.EnemyManagerScript;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Script.UIManager
{
      public class CastleManager : MonoBehaviour
      {
          [SerializeField] protected internal Slider hpBar;
          [SerializeField] private TextMeshProUGUI hpText;
          [SerializeField] private GameManager gameManager;
          public float hpPoint = 1000;
          public float maxHpPoint = 1000;

          private bool Damaged => hpPoint < PreviousHpPoint;
          private float PreviousHpPoint { get; set; }
          
          private void Start()
          {
              PreviousHpPoint = hpPoint;
              hpBar.maxValue = maxHpPoint;
              hpBar.value = hpPoint;
              UpdateHpText();
          }
      
          private void OnTriggerEnter2D(Collider2D collision)
          {
              DOTween.Kill(collision);
              if (!collision.gameObject.CompareTag("Enemy")) return;
              var enemyBase = collision.gameObject.GetComponent<EnemyBase>();
              if (enemyBase == null) return;
              hpPoint -= enemyBase.CrushDamage;
              hpBar.DOValue(hpPoint, 1.0f);
              UpdateHpText();
              FindObjectOfType<EnemyBase>().EnemyKilledEvents(enemyBase);
              if (hpPoint > 0) return;
              hpPoint = 0;
              hpBar.value = hpPoint;
              UpdateHpText();
              StartCoroutine(gameManager.ContinueOrLose());
          }

          private void UpdatePreviousHp()
          {
              PreviousHpPoint = hpPoint;
          }
          
          private void UpdateHpText()
          {
              hpText.text = $"{hpPoint} / {maxHpPoint}";
          }

          public void IncreaseMaxHp(float increaseAmount)
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
              if (!Damaged)
              {
                  hpPoint += 200;
                  if (hpPoint > maxHpPoint)
                  {
                      hpPoint = maxHpPoint;
                  }
              }
              else
              {
                  return;
              }
              UpdatePreviousHp();
          }
      }
}
