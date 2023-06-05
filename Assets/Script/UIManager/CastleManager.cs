using System;
using Script.EnemyManagerScript;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Script.UIManager
{
      public class CastleManager : MonoBehaviour
      {

          [SerializeField] private Slider hpBar;
          [SerializeField] private TextMeshProUGUI hpText;
          [SerializeField] private EnemyPool enemyPool;
          [SerializeField] private GameManager gameManager;
          [SerializeField] private EnemySpawnManager enemySpawnManager;
          public int hpPoint = 1000;
          public int maxHpPoint = 1000;
          public event Action OnEnemyKilled;
          private int PreviousHpPoint { get; set; }
      
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
              enemyBase.ReceiveDamage(enemyBase.HealthPoint, EnemyBase.KillReasons.ByCastle); 
              hpPoint -= enemyBase.CrushDamage;
              // Animate the change in value
              hpBar.DOValue(hpPoint, 1.0f); // Change 0.5f to whatever duration you want for the animation
              UpdateHpText();
              OnEnemyKilled += () => enemySpawnManager.fieldList.Remove(enemyBase.gameObject);
              enemyPool.ReturnToPool(enemyBase.gameObject);
              OnEnemyKilled?.Invoke();
              if (hpPoint <= 0)
              {
                  hpPoint = 0;
                  hpBar.value = hpPoint;
                  UpdateHpText();
                  StartCoroutine(gameManager.ContinueOrLose());
              }
          }
          
          public bool Damaged => hpPoint < PreviousHpPoint;

          public void UpdatePreviousHp()
          {
              PreviousHpPoint = hpPoint;
          }
          
          // Method to update the HP text
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
      }
}
