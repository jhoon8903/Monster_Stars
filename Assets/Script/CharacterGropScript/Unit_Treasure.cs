using System.Collections;
using System.Collections.Generic;
using Script.CharacterManagerScript;
using UnityEngine;

public class Unit_Treasure : CharacterBase
{
    [SerializeField]
    private Sprite level1Sprite;
    [SerializeField]
    private Sprite level2Sprite;
    [SerializeField]
    private Sprite level3Sprite;
    [SerializeField]
    private Sprite level4Sprite;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Level1();
    }

    protected override void LevelUp()
    {
        base.LevelUp();  
        switch (Level)
        {
            case 2:
                Level2();
                break;
            case 3:
                Level3();
                break;
            case 4:
                Level4();
                break;
            default:
                return;
        }
    }

    protected internal override void LevelReset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Level1();
    }

    private void Level1()
    {
        _characterName = "Unit_Treasure00";
        _type = Type.treasure;
        _spriteRenderer.sprite = level1Sprite;
    }

    private void Level2()
    {
        _characterName = "Unit_Treasure01";
        _type = Type.treasure;
        _spriteRenderer.sprite = level2Sprite;
    }

    private void Level3()
    {
        _characterName = "Unit_Treasure02";
        _type = Type.treasure;
        _spriteRenderer.sprite = level3Sprite;
    }

    private void Level4()
    {
        _characterName = "Unit_Treasure03";
        _type = Type.treasure;
        _spriteRenderer.sprite = level4Sprite;
    }
}
