using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Script.PuzzleManagerGroup
{
    public class MergeEffect : MonoBehaviour
    {
        [SerializeField] private GameObject mergeCircle;
        [SerializeField] private GameObject sparkle1;
        [SerializeField] private GameObject sparkle2;
        [SerializeField] private GameObject sparkle3;
        [SerializeField] private GameObject sparkle4;
        [SerializeField] private GameObject sparkle5;
        [SerializeField] private GameObject sparkle6;
        [SerializeField] private GameObject sparkle7;
        [SerializeField] private GameObject sparkle8;
        [SerializeField] private GameObject tileImage;

        public delegate void EffectComplete(GameObject obj, bool isCenter);
        public EffectComplete onEffectComplete;

        public IEnumerator MergeEffectAction(GameObject matchedCharacter, bool isCenter)
        {
            float delay = 0f;
            GameObject[] sparkles = { sparkle1, sparkle2, sparkle3, sparkle4, sparkle5, sparkle6, sparkle7, sparkle8 };
        
            foreach (var sparkle in sparkles)
            {
                DOVirtual.DelayedCall(delay, () => sparkle.SetActive(true));
                DOVirtual.DelayedCall(delay + 0.8f, () => sparkle.SetActive(false));
                delay += Random.Range(0.2f, 0.3f);
            }

            if (isCenter)
            {
                mergeCircle.transform.localScale = Vector3.zero;
                var seq = DOTween.Sequence();
                seq.Append(mergeCircle.transform.DOScale(0.8f, 0.8f))
                    .Append(mergeCircle.transform.DOScale(1f, 0.2f).SetEase(Ease.OutSine))
                    .OnKill(() => mergeCircle.SetActive(false));
            }

            var spriteRenderer = tileImage.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.DOFade(1, 0.25f).OnComplete(() => 
                {
                    spriteRenderer.DOFade(0, 0.45f).OnComplete(() => 
                    {
                        onEffectComplete?.Invoke(matchedCharacter, isCenter);
                    });
                });
            }
            else
            {
                onEffectComplete?.Invoke(matchedCharacter, isCenter);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
