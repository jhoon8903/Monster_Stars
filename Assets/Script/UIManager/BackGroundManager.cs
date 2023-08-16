using System.Collections;
using DG.Tweening;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Script.UIManager
{
    public class BackGroundManager : MonoBehaviour
    {
        [SerializeField] private RectTransform backGround;
        [SerializeField] private RectTransform castle;

        private Vector3 _originalBackGroundPosition;
        private Vector2 _originalCastleSize;
        private Vector3 _originalCastlePosition;

        private void Awake()
        {
            _originalBackGroundPosition = backGround.anchoredPosition3D;
            _originalCastleSize = castle.sizeDelta;
            _originalCastlePosition = castle.anchoredPosition3D;
        }


        public IEnumerator ChangeBattleSize()
        {
            backGround.DOAnchorPos3D(Vector3.zero, 1f);
            castle.DOSizeDelta(new Vector2(2950f, 1000f), 1f);
            castle.DOAnchorPos3D(new Vector3(0f, 600f, 0f), 1f);
            yield return null;
        }

        public IEnumerator ChangePuzzleSize()
        {
            backGround.DOAnchorPos3D(_originalBackGroundPosition, 1f);
            castle.DOSizeDelta(_originalCastleSize, 1f);
            castle.DOAnchorPos3D(_originalCastlePosition, 1f);
            yield return null;
        }
    }
}

