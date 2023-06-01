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
        CharacterName = "Unit_E_00";
        Type = Types.Character;
        defaultDamage = 0;
        defaultAtkRate = 0;
        defaultAtkDistance = 0;
        _spriteRenderer.sprite = level1Sprite;

    }

    private void Level2()
    {
        CharacterName = "Unit_E_01";
        Type = Types.Character;
        UnitGroup = UnitGroups.E;
        defaultDamage = 75;
        defaultAtkRate = 0.1f;
        defaultAtkDistance = 12f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level2Sprite;
        UnitAtkType = UnitAtkTypes.Projectile;
        UnitProperty = UnitProperties.Water;
        UnitEffect = UnitEffects.Slow;
    }

    private void Level3()
    {
        CharacterName = "Unit_E_02";
        Type = Types.Character;
        UnitGroup = UnitGroups.E;
        defaultDamage *= 1.7f;
        defaultAtkRate = 0.3f;
        defaultAtkDistance = 12f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level3Sprite;
        UnitAtkType = UnitAtkTypes.Projectile;
        UnitProperty = UnitProperties.Water;
        UnitEffect = UnitEffects.Slow;
    }

    private void Level4()
    {
        CharacterName = "Unit_E_03";
        Type = Types.Character;
        UnitGroup = UnitGroups.E;
        defaultDamage *= 2;
        defaultAtkRate = 0.9f;
        defaultAtkDistance = 12f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level4Sprite;
        UnitAtkType = UnitAtkTypes.Projectile;
        UnitProperty = UnitProperties.Water;
        UnitEffect = UnitEffects.Slow;
    }

    private void Level5()
    {
        CharacterName = "Unit_E_04";
        Type = Types.Character;
        UnitGroup = UnitGroups.E;
        defaultDamage = 27;
        defaultDamage *= 2.3f;
        defaultAtkDistance = 12f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level5Sprite;
        UnitAtkType = UnitAtkTypes.Projectile;
        UnitProperty = UnitProperties.Water;
        UnitEffect = UnitEffects.Slow;
    }
}
