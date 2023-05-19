using Script.CharacterManagerScript;
using UnityEngine;

public class Unit_D : CharacterBase
{


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

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Level1();
    }

    protected override void LevelUp()
    {
        base.LevelUp();  // increment the level
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
            case 5:
                Level5();
                break;
            default:
                return;
        }
    }

    private void Level1()
    {
        _characterName = "Unit_D_00";
        _damage = 0;
        _atkSpeed = 0;
        _range = 0;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level1Sprite;

    }

    private void Level2()
    {
        _characterName ="Unit_D_01";
        _damage = 1;
        _atkSpeed = 0.1f;
        _range = 0.1f;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level2Sprite;
    }

    private void Level3()
    {
        _characterName = "Unit_D_02";
        _damage = 3;
        _atkSpeed = 0.3f;
        _range = 0.2f;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level3Sprite;
    }

    private void Level4()
    {
        _characterName = "Unit_D_03";
        _damage = 9;
        _atkSpeed = 0.9f;
        _range = 0.3f;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level4Sprite;
    }

    private void Level5()
    {
        _characterName = "Unit_D_04";
        _damage = 27;
        _atkSpeed = 2.7f;
        _range = 0.4f;
        _spearForce = 0;
        _splashRange = 0f;
        _spriteRenderer.sprite = level5Sprite;
    }
}
