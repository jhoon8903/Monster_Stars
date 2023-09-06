using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Script.UIManager
{
    public class CameraManager : MonoBehaviour
    {

        public Camera mainCamera;
        [SerializeField] private float targetSize = 10f;
        [SerializeField] private float duration = 1.0f;

        public void CameraBattleSizeChange()
        {
            mainCamera.transform.DOMove(new Vector3(2.5f, 3.5f, -100f), duration);
            mainCamera.DOOrthoSize(targetSize, duration);
        }

        public IEnumerator CameraPuzzleSizeChange()
        {
            mainCamera.transform.DOMove(mainCamera.transform.position, duration);
            mainCamera.DOOrthoSize(8f, duration);
            yield return null;
        }
    }
}

