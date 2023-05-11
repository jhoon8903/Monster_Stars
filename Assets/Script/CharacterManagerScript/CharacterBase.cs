using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected string _characterName;   // 케릭터 이름 
    protected int _damage;             // 케릭터 데미지
    protected float _atkSpeed;         // 케릭터 공격속도 
    protected float _Range;            // 케릭터 사거리 
    protected int _spearForce;         // 케릭터 관통력 
    protected float _splashRange;      // 케릭터 공격범위
    protected AtkElementProperty _atkElementProperty; // 케릭터 속성 { Fire, Ice, Physical, Posion, Holy, Dark }
    protected SpecialAtkProperty _specialAtkProperty; // 케릭터 특수효과 { Slow, Posion, Burn, Bleed, Stern, Strike }
}
