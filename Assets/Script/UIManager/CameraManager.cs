using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Script.UIManager
{
    public class CameraManager : MonoBehaviour
    {

        public Camera mainCamera;
        [SerializeField] private float targetSize = 10f;
        [SerializeField] private float duration = 1.0f;

        public IEnumerator CameraBattleSizeChange()
        {
            mainCamera.transform.DOMove(new Vector3(2.5f, 3.5f, -100f), duration);
            mainCamera.DOOrthoSize(targetSize, duration);
            yield return null;
        }

        public IEnumerator CameraPuzzleSizeChange()
        {
            mainCamera.transform.DOMove(mainCamera.transform.position, duration);
            mainCamera.DOOrthoSize(8f, duration);
            yield return null;
        }
    }
}

