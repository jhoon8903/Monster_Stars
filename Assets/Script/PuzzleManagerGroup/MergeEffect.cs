using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.PuzzleManagerGroup
{
    public class MergeEffect : MonoBehaviour
    {
        [SerializeField] private GameObject mergeCircleEffect;
        [SerializeField] private GameObject sparkle1;
        [SerializeField] private GameObject sparkle2;
        [SerializeField] private GameObject sparkle3;
        [SerializeField] private GameObject sparkle4;
        [SerializeField] private GameObject sparkle5;
        [SerializeField] private GameObject sparkle6;
        [SerializeField] private GameObject sparkle7;
        [SerializeField] private GameObject sparkle8;
        [SerializeField] private GameObject tileEffect;


        public void MergeAction()
        {

            GameObject[] sparkles = { sparkle1, sparkle2, sparkle3, sparkle4, sparkle5, sparkle6, sparkle7, sparkle8 };
            mergeCircleEffect.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            mergeCircleEffect.SetActive(true);
            mergeCircleEffect.transform.DOScale(1f, 0.55f)
                .OnComplete(() => mergeCircleEffect.transform.DOScale(0.3f, 0.45f));
            
            tileEffect.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            tileEffect.SetActive(true);
            tileEffect.GetComponent<SpriteRenderer>().DOFade(1f, 0.6f)
                .OnComplete(() => tileEffect.GetComponent<SpriteRenderer>().DOFade(0f, 0.4f));

            var delay = 0f;
            foreach (var sparkle in sparkles)
            {
                var randomTime = Random.Range(0.2f, 0.3f);
                DOVirtual.DelayedCall(delay, () => {
                    sparkle.SetActive(true);
                    DOVirtual.DelayedCall(randomTime, () => sparkle.SetActive(false));
                });
                delay += randomTime;
            }
        }

        public void MergeActionClose()
        {
            DOVirtual.DelayedCall(0.9f, () => {
                mergeCircleEffect.SetActive(false);
                tileEffect.SetActive(false);
                gameObject.SetActive(false);
            });
        }

        public void ReturnAction()
        {
            GameObject[] sparkles = { sparkle1, sparkle2, sparkle3, sparkle4, sparkle5, sparkle6, sparkle7, sparkle8 };
            tileEffect.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            tileEffect.SetActive(true);
            tileEffect.GetComponent<SpriteRenderer>().DOFade(1f, 0.12f)
                .OnComplete(() => tileEffect.GetComponent<SpriteRenderer>().DOFade(0f, 0.13f));
            var delay = 0f;
            foreach (var sparkle in sparkles)
            {
                var randomTime = Random.Range(0.1f, 0.2f);
                DOVirtual.DelayedCall(delay, () => {
                    sparkle.SetActive(true);
                    DOVirtual.DelayedCall(randomTime, () => sparkle.SetActive(false));
                });
                delay += randomTime;
            }
        }

        public void ReturnActionClose()
        {
            DOVirtual.DelayedCall(0.3f, () => {
                mergeCircleEffect.SetActive(false);
                tileEffect.SetActive(false);
                gameObject.SetActive(false);
            });
        }
    }
}
