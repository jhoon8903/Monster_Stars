using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Camera mainCamera;
    [SerializeField] private float _targetSize = 10f;
    [SerializeField] private float duration = 1.0f;

    public void CameraSizeChange()
    {
        mainCamera.transform.DOMove(new Vector3(2.5f, 3.5f, -100f), duration);
        mainCamera.DOOrthoSize(_targetSize, duration);
    }
}
