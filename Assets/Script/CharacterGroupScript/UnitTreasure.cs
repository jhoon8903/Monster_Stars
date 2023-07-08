using Script.CharacterManagerScript;
using UnityEngine;

namespace Script.CharacterGroupScript
{
    public class UnitTreasure : CharacterBase
    {
        [SerializeField] private Sprite level1Sprite;
        [SerializeField] private Sprite level2Sprite;
        [SerializeField] private Sprite level3Sprite;
        [SerializeField] private Sprite level4Sprite;
        private SpriteRenderer _spriteRenderer;

        public void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            unitGroup = UnitGroups.None;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            SetLevel(1);
        }

        protected internal override Sprite GetSprite(int level)
        {
            return level switch
            {
                1 => level1Sprite,
                2 => level2Sprite,
                3 => level3Sprite,
                _ => level4Sprite
            };
        }
        private void SetLevel(int level)
        {
            unitPuzzleLevel = level;
            Type = Types.Treasure;
            _spriteRenderer.sprite = GetSprite(level);
        }
        protected override void LevelUp()
        {
            base.LevelUp();
            SetLevel(unitPuzzleLevel);
        }
        protected internal override void CharacterReset()
        {
            base.CharacterReset();
            SetLevel(unitPuzzleLevel);
        }
    }
}
