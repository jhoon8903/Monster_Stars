using DG.Tweening;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class BackGroundManager : MonoBehaviour
{
    [SerializeField] private RectTransform backGround;
    [SerializeField] private RectTransform castle;

    public void ChangeSize()
    {
        backGround.DOAnchorPos3D(Vector3.zero, 1f);
        castle.DOSizeDelta(new Vector2(1440f, 400f), 1f);
        castle.DOAnchorPos3D(new Vector3(0f, 400f, 0f), 1f);
    }
}
