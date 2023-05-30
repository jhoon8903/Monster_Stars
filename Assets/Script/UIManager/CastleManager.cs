using Script.EnemyManagerScript;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Script.UIManager
{
      public class CastleManager : MonoBehaviour
      {
          [SerializeField] private int hpPoint = 1000;
          [SerializeField] private int maxHpPoint = 1000;
          [SerializeField] private Slider hpBar;
          [SerializeField] private TextMeshProUGUI hpText;
          [SerializeField] private EnemyPool enemyPool;
          [SerializeField] private GameManager gameManager;
      
          // Start is called before the first frame update
          private void Start()
          {
              // Set the max value of the Slider to the max HP
              hpBar.maxValue = maxHpPoint;
              // Set the current value of the Slider to the current HP
              hpBar.value = hpPoint;
              // Set the initial HP Text
              UpdateHpText();
          }
      
          private void OnTriggerEnter2D(Collider2D collision)
          {
              if (!collision.gameObject.CompareTag("Enemy")) return;
              var enemyBase = collision.gameObject.GetComponent<EnemyBase>();
              if (enemyBase == null) return;
              hpPoint -= enemyBase.CrushDamage;
              // Animate the change in value
              hpBar.DOValue(hpPoint, 1.0f); // Change 0.5f to whatever duration you want for the animation
              UpdateHpText();
              enemyPool.ReturnToPool(enemyBase.gameObject);
              if (hpPoint <= 0)
              {
                  DOTween.KillAll();
                  hpPoint = 0;
                  hpBar.value = hpPoint;
                  UpdateHpText();
                  gameManager.LoseGame();
              }
          }
      
      
          // Method to increase HP and MaxHP
          public void IncreaseHp(int increaseAmount)
          {
              hpPoint += increaseAmount;
              maxHpPoint += increaseAmount;
              hpBar.maxValue = maxHpPoint;
              // Animate the change in value
              hpBar.DOValue(hpPoint, 1.0f); // Change 0.5f to whatever duration you want for the animation
              UpdateHpText();
          }
      
      
          // Method to update the HP text
          private void UpdateHpText()
          {
              hpText.text = $"{hpPoint} / {maxHpPoint}";
          }
      }
}
