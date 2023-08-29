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
        [SerializeField] private  GameObject castle;

        [Serializable]
        public class BattleSpriteSet
        {
            public Sprite back;
            public Sprite topSprite;
            public Sprite castleSprite;
            public Sprite grid1;
            public Sprite grid2;
            public Sprite destroySprite;
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

        public void ChangedBackGround(int stage)
        {
            // 0 : Forest  1 : Desert 2 : Snow 3 : Swam 4 : Volcanic
            var index = 0;
            if (StageManager.Instance == null) return;
            index = stage switch
            {
                1 or 8 or 14 or 19 => 0,
                2 or 6 or 12 or 17 => 1,
                3 or 10 or 13 or 16 => 2,
                4 or 7 or 11 or 18 => 3,
                5 or 9 or 15 or 20 => 4,
                _ => index
            };
            BackSprite(index);
        }

        private void BackSprite(int index)
        {
            backGround.GetComponent<Image>().sprite = spriteSet[index].back;
            top.GetComponent<Image>().sprite = spriteSet[index].topSprite;
            castle.GetComponent<Image>().sprite = spriteSet[index].castleSprite;
            gridManager.grid1Sprite.GetComponent<SpriteRenderer>().sprite = spriteSet[index].grid1;
            gridManager.grid2Sprite.GetComponent<SpriteRenderer>().sprite = spriteSet[index].grid2;
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

        public void Lose()
        {
            if (StageManager.Instance == null) return;
            var stage = StageManager.Instance.selectStage;
            castle.GetComponent<Image>().sprite = spriteSet[stage - 1].destroySprite;
        }
    }
}

