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
            backGround.DOAnchorPos3D(new Vector3(0f,45f,0f), 1f);
            castle.DOSizeDelta(new Vector2(1100f, 300f), 1f);
            castle.DOAnchorPos3D(new Vector3(0f, 280f, 0f), 1f);
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

