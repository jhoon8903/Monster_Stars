using System.Collections;
using DG.Tweening;
using Script.RobbyScript.MainMenuGroup;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class LoadingManager : MonoBehaviour 
    {
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private Image handle;
        public bool isFirstContact;

        public void Start()
        {
            if (!bool.Parse(PlayerPrefs.GetString(MainPanel.IsLoadingKey, "true")))
            {
                gameObject.SetActive(false);
            }
            else
            {
                Time.timeScale = 1f;
                if (PlayerPrefs.HasKey("TutorialKey"))
                {
                    isFirstContact = PlayerPrefs.GetInt("TutorialKey", 1) == 1;
                }
                else
                {
                    PlayerPrefs.SetInt("TutorialKey", 1);
                    isFirstContact = true;
                }
                StartCoroutine(UpdateLoadingBar());
                StartCoroutine(ShakeHandle());
            }
        }

        private IEnumerator UpdateLoadingBar()
        {
            const float duration = 2f;
            var progress = 0f;
            loadingSlider.value = 0f;

            while (progress < 1f)
            {
                progress += Time.deltaTime / duration;
                loadingSlider.value = Mathf.Clamp01(progress);
                yield return null;
            }

            loadingSlider.value = 1f;

            if (isFirstContact)
            {
                MainPanel.Instance.StartGame();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private IEnumerator ShakeHandle()
        {
            handle.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            const float shakeDuration = 0.5f;

            while (true) // 무한 루프로 계속 움직임
            {
                handle.rectTransform.DOLocalRotate(new Vector3(0, 0, 30), shakeDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(shakeDuration);

                handle.rectTransform.DOLocalRotate(new Vector3(0, 0, 0), shakeDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(shakeDuration);
            }
        }
    }
}
