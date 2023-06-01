using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;


public class Unit_F : CharacterBase
{
    [SerializeField] private Sprite level1Sprite;
    [SerializeField] private Sprite level2Sprite;
    [SerializeField] private Sprite level3Sprite;
    [SerializeField] private Sprite level4Sprite;
    [SerializeField] private Sprite level5Sprite;
    private SpriteRenderer _spriteRenderer;
    private const float detectionSize = 1.5f;

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

    public override List<GameObject> DetectEnemies()
    {
        var detectionCenter = (Vector2)transform.position;
        var colliders = Physics2D.OverlapCircleAll(detectionCenter, detectionSize);
        var detectedEnemies = (from collider in colliders
            where collider.gameObject.CompareTag("Enemy")
            select collider.gameObject).ToList();
        foreach (var enemy in detectedEnemies)
        {
            Debug.Log($"DetectEnemies_Unit_F: " +
                      $"Detected enemy {enemy.name} " +
                      $"at position {enemy.transform.position}");
        }
        return detectedEnemies;
    }

    public void OnDrawGizmos()
    {
        var detectionCenter = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(detectionCenter, detectionSize);
    }

    private void Level1()
    {
        CharacterName = "Unit_F_00";
        Type = Types.Character;
        UnitGroup = UnitGroups.F;
        defaultDamage = 0f;
        defaultAtkRate = 0;
        defaultAtkDistance = 0;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level1Sprite;
        UnitAtkType = UnitAtkTypes.Gas;
        UnitProperty = UnitProperties.Poison;
        UnitEffect = UnitEffects.Poison;
    }

    private void Level2()
    {
        CharacterName = "Unit_F_01";
        Type = Types.Character;
        UnitGroup = UnitGroups.F;
        defaultDamage = 90;
        defaultAtkRate = 0.1f;
        defaultAtkDistance = 0.1f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level2Sprite;
        UnitAtkType = UnitAtkTypes.Gas;
        UnitProperty = UnitProperties.Poison;
        UnitEffect = UnitEffects.Poison;
    }

    private void Level3()
    {
        CharacterName = "Unit_F_02";
        Type = Types.Character;
        UnitGroup = UnitGroups.F;
        defaultDamage = 3;
        defaultAtkRate = 0.3f;
        defaultAtkDistance = 0.2f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level3Sprite;
        UnitAtkType = UnitAtkTypes.Gas;
        UnitProperty = UnitProperties.Poison;
        UnitEffect = UnitEffects.Poison;
    }

    private void Level4()
    {
        CharacterName = "Unit_F_03";
        Type = Types.Character;
        UnitGroup = UnitGroups.F;
        defaultDamage = 9;
        defaultAtkRate = 0.9f;
        defaultAtkDistance = 0.3f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level4Sprite;
        UnitAtkType = UnitAtkTypes.Gas;
        UnitProperty = UnitProperties.Poison;
        UnitEffect = UnitEffects.Poison;
    }

    private void Level5()
    {
        CharacterName = "Unit_F_04";
        Type = Types.Character;
        UnitGroup = UnitGroups.F;
        defaultDamage = 27;
        defaultAtkRate = 2.7f;
        defaultAtkDistance = 0.4f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level5Sprite;
        UnitAtkType = UnitAtkTypes.Gas;
        UnitProperty = UnitProperties.Poison;
        UnitEffect = UnitEffects.Poison;
    }
}
