using System.Collections.Generic;
using UnityEngine;

namespace Script.RewardScript
{
    public class ExpData
    {
        private static readonly System.Random Random = new System.Random();
        public int? ChosenProperty; 
        public enum Types
        {
            // Common Property
            GroupDamage,
            GroupAtkSpeed,
            Step,
            StepLimit,
            StepDirection,
            Exp,
            CastleRecovery,
            CastleMaxHp,
            Slow,
            NextStage,
            Gold,
           
            // Divine Property
            DivineActiveRestraint,
            DivineRestraintTime,
            DivinePenetrate,
            DivineAtkRange,
            DivinePoisonAdditionalDamage,
            
            // Physic Property
            PhysicAdditionalWeapon,
            PhysicIncreaseWeaponScale,
            PhysicSlowAdditionalDamage,
            PhysicAtkSpeed,
            PhysicIncreaseDamage,
            Physics2AdditionalBleedingLayer,
            Physics2ActivateBleed,
            Physics2AdditionalAtkSpeed,
            Physics2AdditionalProjectile,
            Physics2ProjectilePenetration,
            
            // Poison Property
            PoisonDoubleAtk,
            PoisonRestraintAdditionalDamage,
            PoisonActivate,
            PoisonInstantKill,
            PoisonIncreaseAtkRange,
            PoisonOverlapping,
            
            // Water Property
            WaterBurnAdditionalDamage,
            WaterSideAttack,
            WaterIncreaseSlowPower,
            WaterRestraintIncreaseDamage,
            WaterIncreaseDamage,
            Water2IncreaseDamage,
            Water2BleedAdditionalRestraint,
            Water2IncreaseSlowTime,
            Water2BackAttack,
            Water2AdditionalProjectile,
            
            //Darkness
            DarkSlowAdditionalDamage,
            DarkBleedAdditionalDamage,
            DarkIncreaseAtkSpeed,
            DarkProjectilePenetration,
            DarkAdditionalFrontAttack,

            // Fire Property
            FireBleedingAdditionalDamage,
            FireIncreaseDamage,
            FirePoisonAdditionalStun,
            FireIncreaseAtkRange,
            FireDeleteBurnIncreaseDamage,
        }
        public Types Type { get; set; }
        private readonly int[] _property;
        public int[] Property 
        { 
            get
            {
                ChosenProperty ??= RandomChanceMethod(_property);
                return new[] { ChosenProperty.Value };
            }
        }
        public int Code { get; set; }
        public Sprite BtnColor { get; private set; }
        protected ExpData(Sprite btnColor, int code, Types type, int[] property)
        {
            BtnColor = btnColor;
            Code = code;
            Type = type;
            _property = property;
        }
        private static int RandomChanceMethod(IReadOnlyList<int> array)
        {
            var randomIndex = Random.Next(0, array.Count);
            return array[randomIndex];
        }
    }

    public class ExpGreenData : ExpData
    {
        public ExpGreenData(Sprite btnColor, int code, Types type, int[] property)
            : base(btnColor, code, type, property)
        {
        }
    }
    public class ExpBlueData : ExpData
    {
        public ExpBlueData(Sprite btnColor, int code, Types type, int[] property)
            : base(btnColor, code, type, property)
        {
        }
    }
    public class ExpPurpleData : ExpData
    {
        public ExpPurpleData(Sprite btnColor, int code, Types type, int[] property)
            : base(btnColor, code, type, property)
        {
        }
    }
    public class Exp : MonoBehaviour
    {
        public List<ExpData> ExpGreenList { get; private set; } = new List<ExpData>();
        public List<ExpData> ExpBlueList { get; private set; } = new List<ExpData>();
        public List<ExpData> ExpPurpleList { get; private set; } = new List<ExpData>();
        [SerializeField] private LevelUpRewardManager levelUpRewardManager;

        public void Start()
        {
            var g = levelUpRewardManager.greenSprite;
            var b = levelUpRewardManager.blueSprite;
            var p = levelUpRewardManager.purpleSprite;

            ExpGreenList = new List<ExpData>
            {
                new ExpGreenData(g,1, ExpData.Types.GroupDamage, new[]{4}),
                new ExpGreenData(g,2,ExpData.Types.GroupAtkSpeed,new[]{3,4,5}), 
                new ExpGreenData(g,3,ExpData.Types.Step, new [] {3,4,5}),
                new ExpGreenData(g,6, ExpData.Types.Exp, new []{5}),
                new ExpGreenData(g,100,ExpData.Types.DivinePoisonAdditionalDamage, new []{100}),
                new ExpGreenData(g,200,ExpData.Types.PhysicAdditionalWeapon, new []{1}),
                new ExpGreenData(g,300,ExpData.Types.PoisonRestraintAdditionalDamage, new []{200}),
                new ExpGreenData(g,400,ExpData.Types.WaterBurnAdditionalDamage,new []{200}),
                new ExpGreenData(g,500,ExpData.Types.FireBleedingAdditionalDamage, new []{150}),
                new ExpGreenData(g,600,ExpData.Types.Physics2AdditionalBleedingLayer, new []{1}),
                new ExpGreenData(g,700,ExpData.Types.DarkSlowAdditionalDamage, new []{50}),
                new ExpGreenData(g,800,ExpData.Types.Water2IncreaseDamage, new []{9})
            };

            ExpBlueList = new List<ExpData>
            {
                new ExpBlueData(b,1,ExpData.Types.GroupDamage, new []{6,7,8,9,10}),
                new ExpBlueData(b,2,ExpData.Types.GroupAtkSpeed, new []{10,11,12}),
                new ExpBlueData(b,3,ExpData.Types.Step, new []{9,10,11}),
                new ExpBlueData(b,7,ExpData.Types.Gold, new []{1}),
                new ExpBlueData(b,8,ExpData.Types.CastleMaxHp, new []{200}),
                new ExpBlueData(b,9,ExpData.Types.CastleRecovery, new []{200}),
                new ExpBlueData(b,11, ExpData.Types.Slow, new []{15}),
                new ExpBlueData(b,100,ExpData.Types.DivineAtkRange, new []{1}),
                new ExpBlueData(b,200,ExpData.Types.PhysicSlowAdditionalDamage,new []{100}),
                new ExpBlueData(b,300,ExpData.Types.PoisonDoubleAtk, new []{1}),
                new ExpBlueData(b,300,ExpData.Types.PoisonOverlapping, new[]{1}),
                new ExpBlueData(b,400,ExpData.Types.WaterSideAttack, new []{1}),
                new ExpBlueData(b,400,ExpData.Types.WaterIncreaseDamage, new []{20}),
                new ExpBlueData(b,500, ExpData.Types.FireIncreaseDamage, new []{15}),
                new ExpBlueData(b,500,ExpData.Types.FirePoisonAdditionalStun, new []{1}),
                new ExpBlueData(b,600,ExpData.Types.Physics2ActivateBleed ,new []{1}),
                new ExpBlueData(b,600, ExpData.Types.Physics2AdditionalAtkSpeed, new []{17}),
                new ExpBlueData(b,700, ExpData.Types.DarkBleedAdditionalDamage, new []{200}),
                new ExpBlueData(b,700, ExpData.Types.DarkIncreaseAtkSpeed, new []{17}),
                new ExpBlueData(b,800, ExpData.Types.Water2BleedAdditionalRestraint, new []{1}),
                new ExpBlueData(b,800,ExpData.Types.Water2IncreaseSlowTime, new []{1})
            };

            ExpPurpleList = new List<ExpData>
            {
                new ExpPurpleData(p,1,ExpData.Types.GroupDamage,new []{18}),
                new ExpPurpleData(p,2,ExpData.Types.GroupAtkSpeed, new []{19}),
                new ExpPurpleData(p,3,ExpData.Types.Step, new []{14,15,16,17}),
                // new ExpPurpleData(p,3,ExpData.Types.StepLimit, new []{1} ),
                new ExpPurpleData(p,13,ExpData.Types.StepDirection, new []{1}),
                new ExpPurpleData(p,14,ExpData.Types.NextStage, new []{1,2}),
                new ExpPurpleData(p,100,ExpData.Types.DivineActiveRestraint, new []{1}),
                new ExpPurpleData(p,100,ExpData.Types.DivineRestraintTime, new []{1}),
                new ExpPurpleData(p,100,ExpData.Types.DivinePenetrate, new []{1}),
                new ExpPurpleData(p,200,ExpData.Types.PhysicAtkSpeed, new []{50}),
                new ExpPurpleData(p,200,ExpData.Types.PhysicIncreaseDamage, new []{5}),
                new ExpPurpleData(p,200,ExpData.Types.PhysicIncreaseWeaponScale, new []{5}),
                new ExpPurpleData(p,300,ExpData.Types.PoisonInstantKill, new []{15}),
                new ExpPurpleData(p,300,ExpData.Types.PoisonIncreaseAtkRange, new []{1}),
                new ExpPurpleData(p,300,ExpData.Types.PoisonActivate, new []{1}),
                new ExpPurpleData(p,400,ExpData.Types.WaterIncreaseSlowPower, new []{50}),
                new ExpPurpleData(p,400,ExpData.Types.WaterRestraintIncreaseDamage, new []{1}),
                new ExpPurpleData(p,500,ExpData.Types.FireIncreaseAtkRange, new []{1}),
                new ExpPurpleData(p,500,ExpData.Types.FireDeleteBurnIncreaseDamage, new []{200}),
                new ExpPurpleData(p,600,ExpData.Types.Physics2AdditionalProjectile, new []{1}),
                new ExpPurpleData(p,600,ExpData.Types.Physics2ProjectilePenetration, new []{1}),
                new ExpPurpleData(p,700,ExpData.Types.DarkProjectilePenetration, new []{1}),
                new ExpPurpleData(p,700,ExpData.Types.DarkAdditionalFrontAttack, new []{1}),
                new ExpPurpleData(p,800,ExpData.Types.Water2BackAttack, new []{1}),
                new ExpPurpleData(p,800,ExpData.Types.Water2AdditionalProjectile, new []{1})
            };
        }
    }
}