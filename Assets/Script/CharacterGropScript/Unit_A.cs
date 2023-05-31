using Script.CharacterManagerScript;
using UnityEngine;

public class Unit_A : CharacterBase
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
    private const float DefaultDamage = 125f;
    private float _arrowSpeed = 0.33f;
    private float _increaseSpeed;
    private float _resistValue;
    private float _increaseDamage;

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
        _characterName = "Unit_A_00";
        _type = Type.character;
        _defaultDamage = 0;
        _defaultAtkSpeed = 0;
        _defaultAtkDistance = 0;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level1Sprite;
    }

    private void Level2()
    {
        _characterName = "Unit_A_01";
        _type = Type.character;
        _defaultDamage = (DefaultDamage * (1 + _increaseDamage)) * (1 - _resistValue);
        _defaultAtkSpeed = 1;
        _defaultAtkDistance = 12;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level2Sprite;
        _unitAtkType = Unit_AtkType.Arrow;
        _arrowSpeed *= _increaseSpeed;
        _unitProperty = Unit_Property.Divine;
        _unitEffect = Unit_Effect.Restraint;
    }

    private void Level3()
    {
        _characterName = "Unit_A_02";
        _type = Type.character;
        _defaultDamage *= 1.7f;
        _defaultAtkSpeed = 0.3f;
        _defaultAtkDistance = 12;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level3Sprite;
        _unitAtkType = Unit_AtkType.Arrow;
        _arrowSpeed = 0.33f;
        _unitProperty = Unit_Property.Divine;
        _unitEffect = Unit_Effect.Restraint;
    }

    private void Level4()
    {
        _characterName = "Unit_A_03";
        _type = Type.character;
        _defaultDamage *= 2;
        _defaultAtkSpeed = 0.9f;
        _defaultAtkDistance = 12;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level4Sprite;
        _unitAtkType = Unit_AtkType.Arrow;
        _arrowSpeed = 0.33f;
        _unitProperty = Unit_Property.Divine;
        _unitEffect = Unit_Effect.Restraint;
    }

    private void Level5()
    {
        _characterName = "Unit_A_04";
        _type = Type.character;
        _defaultDamage *= 2.3f;
        _defaultAtkSpeed = 2.7f;
        _defaultAtkDistance = 12;
        _defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level5Sprite;
        _unitAtkType = Unit_AtkType.Arrow;
        _arrowSpeed = 0.33f;
        _unitProperty = Unit_Property.Divine;
        _unitEffect = Unit_Effect.Restraint;
    }
}
