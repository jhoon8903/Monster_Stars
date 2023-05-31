using Script.CharacterManagerScript;
using UnityEngine;


public class Unit_E : CharacterBase
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

    protected internal override void LevelReset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Level1();
    }

    private void Level1()
    {
        _characterName = "Unit_E_00";
        _type = Type.character;
        _defaultDamage = 0;
        _defaultAtkSpeed = 0;
        _defaultAtkDistance = 0;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level1Sprite;

    }

    private void Level2()
    {
        _characterName = "Unit_E_01";
        _type = Type.character;
        _defaultDamage = 1;
        _defaultAtkSpeed = 0.1f;
        _defaultAtkDistance = 0.1f;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level2Sprite;
    }

    private void Level3()
    {
        _characterName = "Unit_E_02";
        _type = Type.character;
        _defaultDamage = 3;
        _defaultAtkSpeed = 0.3f;
        _defaultAtkDistance = 0.2f;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level3Sprite;
    }

    private void Level4()
    {
        _characterName = "Unit_E_03";
        _type = Type.character;
        _defaultDamage = 9;
        _defaultAtkSpeed = 0.9f;
        _defaultAtkDistance = 0.3f;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level4Sprite;
    }

    private void Level5()
    {
        _characterName = "Unit_E_04";
        _type = Type.character;
        _defaultDamage = 27;
        _defaultAtkSpeed = 2.7f;
        _defaultAtkDistance = 0.4f;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level5Sprite;
    }
}
