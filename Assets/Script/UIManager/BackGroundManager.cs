using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script.PuzzleManagerGroup;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace Script.UIManager
{
    public class BackGroundManager : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private GameObject backGround;
        [SerializeField] private GameObject top;
        [SerializeField] private GameObject castle;
        [SerializeField] private Image leftSide;
        [SerializeField] private Image rightSide;

        [Serializable]
        public class BattleSpriteSet
        {
            public Sprite back;
            public Sprite topSprite;
            public Sprite castleSprite;
            public Sprite left;
            public Sprite right;
            public Sprite grid1;
            public Sprite grid2;
        }

        public List<BattleSpriteSet> spriteSet = new List<BattleSpriteSet>();
        private Vector3 _originalBackGroundPosition;
        private Vector2 _originalCastleSize;
        private Vector3 _originalCastlePosition;
        public static BackGroundManager Instance;

        private void Awake()
        {
            Instance = this;
            _originalBackGroundPosition = top.GetComponent<RectTransform>().anchoredPosition3D;
            _originalCastleSize = castle.GetComponent<RectTransform>().sizeDelta;
            _originalCastlePosition = castle.GetComponent<RectTransform>().anchoredPosition3D;
        }

        public void ChangedBackGround()
        {
            var stage = StageManager.Instance.selectStage;
            backGround.GetComponent<Image>().sprite = spriteSet[stage - 1].back;
            top.GetComponent<Image>().sprite = spriteSet[stage - 1].topSprite;
            castle.GetComponent<Image>().sprite = spriteSet[stage - 1].castleSprite;
            leftSide.sprite = spriteSet[stage - 1].left;
            rightSide.sprite = spriteSet[stage - 1].right;
            gridManager.grid1Sprite.GetComponent<SpriteRenderer>().sprite = spriteSet[stage - 1].grid1;
            gridManager.grid2Sprite.GetComponent<SpriteRenderer>().sprite = spriteSet[stage - 1].grid2;
        }

        public IEnumerator ChangeBattleSize()
        {
            top.GetComponent<RectTransform>().DOAnchorPos3D(Vector3.zero, 1f);
            castle.GetComponent<RectTransform>().DOSizeDelta(new Vector2(2950f, 1000f), 1f);
            castle.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 600f, 0f), 1f);
            yield return null;
        }

        public IEnumerator ChangePuzzleSize()
        {
            top.GetComponent<RectTransform>().DOAnchorPos3D(_originalBackGroundPosition, 1f);
            castle.GetComponent<RectTransform>().DOSizeDelta(_originalCastleSize, 1f);
            castle.GetComponent<RectTransform>().DOAnchorPos3D(_originalCastlePosition, 1f);
            yield return null;
        }


    }
}

