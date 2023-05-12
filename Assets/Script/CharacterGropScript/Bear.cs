using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AtkElementProperty;
using static SpecialAtkProperty;

public class Bear : CharacterBase
{
    //AtkElementProperty.AtkElement _atkElement = AtkElementProperty.AtkElement.Physical;
    //SpecialAtkProperty.SpecialElment _specialElment = SpecialAtkProperty.SpecialElment.Strike;
    //bool _atkElementBool = false;

    [SerializeField]
    private Sprite level1Sprite;
    [SerializeField]
    private Sprite level2Sprite;
    [SerializeField]
    private Sprite level3Sprite;
    [SerializeField]
    private Sprite level4Sprite;
    [SerializeField]
    private Sprite level5Sprite;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Level1();
    }

    public void Level1()
    {
        _characterName = "bear1";
        _damage = 0;
        _atkSpeed = 0;
        _Range = 0;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level1Sprite;

    }

    public void Level2()
    {
        _characterName = "bear2";
        _damage = 1;
        _atkSpeed = 0.1f;
        _Range = 0.1f;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level2Sprite;
    }

    public void Level3()
    {
        _characterName = "bear3";
        _damage = 3;
        _atkSpeed = 0.3f;
        _Range = 0.2f;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level3Sprite;
    }

    public void Level4()
    {
        _characterName = "bear4";
        _damage = 9;
        _atkSpeed = 0.9f;
        _Range = 0.3f;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level4Sprite;
    }

    public void Level5()
    {
        _characterName = "bear5";
        _damage = 27;
        _atkSpeed = 2.7f;
        _Range = 0.4f;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level5Sprite;
    }
}
