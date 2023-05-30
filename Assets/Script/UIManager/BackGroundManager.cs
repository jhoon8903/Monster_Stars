using DG.Tweening;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Script.UIManager
{
    public class BackGroundManager : MonoBehaviour
    {
        [SerializeField] private RectTransform backGround;
        [SerializeField] private RectTransform castle;

        public void ChangeSize()
        {
            backGround.DOAnchorPos3D(Vector3.zero, 1f);
            castle.DOSizeDelta(new Vector2(1100f, 300f), 1f);
            castle.DOAnchorPos3D(new Vector3(0f, 250f, 0f), 1f);
        }
    }
}

