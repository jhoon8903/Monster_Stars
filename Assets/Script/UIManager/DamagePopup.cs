using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.EnemyManagerScript;
using TMPro;
using UnityEngine;

namespace Script.UIManager
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private GameObject damageText;
        public static DamagePopup Instance;
        private bool _pooling;
        private GameObject _damagePopup;
        private readonly List<GameObject> _damagePopupList = new List<GameObject>();
        private readonly Dictionary<EnemyBase, GameObject> _lastPopupByEnemy = new Dictionary<EnemyBase, GameObject>();
        private void Awake()
        {
            Instance = this;

            if (_pooling) return;
            for (var i = 0; i < 300; i++)
            {
                _damagePopup = Instantiate(damageText, gameObject.transform, false);
                _damagePopupList.Add(_damagePopup);
                _damagePopup.SetActive(false);
                _pooling = true;
            }
        }

        public IEnumerator DamageTextPopup(EnemyBase enemyBase, int damage)
        {
            GameObject? popupToUse = _damagePopupList.FirstOrDefault(popup => !popup.activeInHierarchy);
            if (popupToUse == null || !enemyBase.gameObject.activeInHierarchy) yield break;

            if (_lastPopupByEnemy.ContainsKey(enemyBase) && _lastPopupByEnemy[enemyBase] != null)
            {
                _lastPopupByEnemy[enemyBase].SetActive(false);
            }

            _lastPopupByEnemy[enemyBase] = popupToUse;

            var pos = enemyBase.transform.position;
            popupToUse.transform.position = new Vector3(pos.x, pos.y + 0.5f, 0f);
    
            if (enemyBase.EnemyType == EnemyBase.EnemyTypes.Boss)
            {
                popupToUse.transform.position = new Vector3(pos.x, pos.y + 1.7f, 0f);
                popupToUse.transform.localScale = new Vector3(1.5f, 1.5f, 0);
            }

            Vector2 startPosition = popupToUse.transform.position;
            var endPosition = new Vector2(startPosition.x, startPosition.y + 0.2f);

            if (damage == 0) yield break;
            popupToUse.SetActive(true);
            popupToUse.GetComponent<TextMeshPro>().text = damage.ToString();
            Color damageColor;
            if (enemyBase.isBleed)
            {
                damageColor = new Color(0.82f,0.089f, 0.089f);
            }
            else if (enemyBase.isPoison)
            {
                damageColor = new Color(0.0264f, 0.8018f, 0.1808f);
            }
            else if (enemyBase.isBurn)
            {
                damageColor = Color.red;
            }
            else
            {
                damageColor = Color.white;
            }

            popupToUse.GetComponent<TextMeshPro>().color = damageColor;

            float t = 0;
            const float speed = 1f;
            while (t < 1)
            {
                t += Time.deltaTime * speed;
                popupToUse.transform.position = Vector2.Lerp(startPosition, endPosition, t);
                yield return null;
                if (enemyBase.gameObject.activeInHierarchy) continue;
                // if (!_lastPopupByEnemy.TryGetValue(enemyBase, out var value)) continue;
                // value.SetActive(false);
                // yield break;
            }

            yield return new WaitForSecondsRealtime(0.2f);
            popupToUse.SetActive(false);
        }
    }
}