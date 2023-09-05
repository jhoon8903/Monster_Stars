using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.UIManager
{
    public class FxManager : MonoBehaviour
    {
        [SerializeField] private GameObject bornEffect;
        public List<GameObject> bornPoolList = new List<GameObject>();
        public static FxManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            for (var i = 0; i < 30; i++)
            {
                var born = Instantiate(bornEffect, transform, true);
                born.SetActive(false);
                bornPoolList.Add(born);
            }
        }

        public void DeadEffect(Vector3 position)
        {
            foreach (var bornObject in bornPoolList.Where(bornObject => !bornObject.activeInHierarchy))
            {
                bornObject.transform.position = position;
                bornObject.SetActive(true);
                StartCoroutine(SetFalseEffect(bornObject));
                break;
            }
        }

        private static IEnumerator SetFalseEffect(GameObject bornEffects)
        {
            if (bornEffects != null)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                bornEffects.SetActive(false);
            }
            yield return null;
        }
    }
}