using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;

public class Unit_D : CharacterBase
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
            Debug.Log($"DetectEnemies_Unit_D: " +
                      $"Detected enemy {enemy.name} " +
                      $"at position {enemy.transform.position}");
        }
        return detectedEnemies;
    }

    public void OnDrawGizmos()
    {
        var detectionCenter = transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(detectionCenter, detectionSize);
    }

    private void Level1()
    {
        CharacterName = "Unit_D_00";
        Type = Types.Character;
        defaultDamage = 0;
        defaultAtkRate = 0;
        defaultAtkDistance = 0;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level1Sprite;

    }

    private void Level2()
    {
        CharacterName ="Unit_D_01";
        Type = Types.Character;
        UnitGroup = UnitGroups.D;
        defaultDamage = 200;
        defaultAtkRate = 1f;
        defaultAtkDistance = 1f;
        swingSpeed = 1f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level2Sprite;
        UnitAtkType = UnitAtkTypes.Circle;
        UnitProperty = UnitProperties.Physics;
        UnitEffect = UnitEffects.None;
    }

    private void Level3()
    {
        CharacterName = "Unit_D_02";
        Type = Types.Character;
        UnitGroup = UnitGroups.D;
        defaultDamage *= 1.7f;
        defaultAtkRate = 1f;
        defaultAtkDistance = 1f;
        swingSpeed = 1f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level3Sprite;
        UnitAtkType = UnitAtkTypes.Circle;
        UnitProperty = UnitProperties.Physics;
        UnitEffect = UnitEffects.None;
    }

    private void Level4()
    {
        CharacterName = "Unit_D_03";
        Type = Types.Character;
        UnitGroup = UnitGroups.D;
        defaultDamage *= 2.0f;
        defaultAtkRate = 1f;
        defaultAtkDistance = 1f;
        swingSpeed = 1f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level4Sprite;
        UnitAtkType = UnitAtkTypes.Circle;
        UnitProperty = UnitProperties.Physics;
        UnitEffect = UnitEffects.None;
    }

    private void Level5()
    {
        CharacterName = "Unit_D_04";
        Type = Types.Character;
        UnitGroup = UnitGroups.D;
        defaultDamage *= 2.3f;
        defaultAtkRate = 1f;
        defaultAtkDistance = 1f;
        swingSpeed = 1f;
        defaultAtkRange = Vector3.zero;
        _spriteRenderer.sprite = level5Sprite;
        UnitAtkType = UnitAtkTypes.Circle;
        UnitProperty = UnitProperties.Physics;
        UnitEffect = UnitEffects.None;
    }
}
