using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.PowerUpScript
{
    public class ExpData
    {
        public enum Types
        {
            // Common Property
            GroupDamage,
            GroupAtkSpeed,
            StepLimit,
            StepDirection,
            Exp,
            CastleRecovery,
            CastleMaxHp,
            Board,
            Slow,
            NextStage,
            Gold,
            // Divine Property
            DivineRestraint,
            DivinePenetrate,
            DivineRestraintDamage,
            DivineAtkRange,
            DivinePoisonAdditionalDamage,
            // Physic Property
            PhysicAdditionalWeapon,
            PhysicIncreaseWeaponScale,
            PhysicSlowAdditionalDamage,
            PhysicAtkSpeed,
            PhysicIncreaseDamage,
            // Poison Property
            PoisonDoubleAtk,
            PoisonRestraintAdditionalDamage,
            PoisonIncreaseTime,
            PoisonInstantKill,
            PoisonIncreaseAtkRange,
            // Water Property
            WaterStun,
            WaterIncreaseSlowTime,
            WaterIncreaseSlowPower,
            WaterRestraintKnockBack,
            WaterIncreaseDamage
        }

        public Types Type { get; set; }
        public int[] Property { get; set; }
        public int Code { get; set; }
        public Sprite Image { get; private set; }
        public int Count { get; set; }

        public enum ExpRepeatTypes
        {
            Repeat,
            NoneRepeat
        }

        public ExpRepeatTypes ExpRepeat { get; set; }

        protected ExpData(Sprite image, int code, ExpRepeatTypes commonRepeat, int count, Types type, int[] property)
        {
            Image = image;
            Code = code;
            ExpRepeat = commonRepeat;
            Count = count;
            Type = type;
            Property = property;
        }
    }

    public class ExpGreenData : ExpData
    {
        public ExpGreenData(Sprite image, int code, ExpRepeatTypes expRepeat, int count, Types type, int[] property)
            : base(image, code, expRepeat, count, type, property)
        {
        }
    }

    public class ExpBlueData : ExpData
    {
        public ExpBlueData(Sprite image, int code, ExpRepeatTypes expRepeat, int count, Types type, int[] property)
            : base(image, code, expRepeat, count, type, property)
        {
        }
    }

    public class ExpPurpleData : ExpData
    {
        public ExpPurpleData(Sprite image, int code, ExpRepeatTypes expRepeat, int count, Types type, int[] property)
            : base(image, code, expRepeat, count, type, property)
        {
        }
    }

    public class Exp : MonoBehaviour
    {
        public List<ExpData> ExpGreenList { get; private set; } = new List<ExpData>();
        public List<ExpData> ExpBlueList { get; private set; } = new List<ExpData>();
        public List<ExpData> ExpPurpleList { get; private set; } = new List<ExpData>();
        [SerializeField] private TreasureManager treasureManager;

        public void Start()
        {
            var g = treasureManager.greenBtn;
            var b = treasureManager.blueBtn;
            var p = treasureManager.purpleBtn;
            const ExpData.ExpRepeatTypes r = ExpData.ExpRepeatTypes.Repeat;
            const ExpData.ExpRepeatTypes n = ExpData.ExpRepeatTypes.NoneRepeat;

            ExpGreenList = new List<ExpData>
            {
                ExpGreen(g,1, r,0, ExpData.Types.GroupDamage, new[]{4}),
                ExpGreen(g,2,r,0,ExpData.Types.GroupAtkSpeed,new[]{4}),
                ExpGreen(g,7,r,6, ExpData.Types.Exp, new []{5}),
                ExpGreen(g,100,n,1,ExpData.Types.DivinePoisonAdditionalDamage, new []{50}),
                ExpGreen(g,200,n,1,ExpData.Types.PhysicAdditionalWeapon, new []{1}),
                ExpGreen(g,300,n,1,ExpData.Types.PoisonRestraintAdditionalDamage, new []{200}),
                ExpGreen(g,400,n,1,ExpData.Types.WaterStun,new []{15})
            };

            ExpBlueList = new List<ExpData>
            {
                ExpBlue(b,1,r,0,ExpData.Types.GroupDamage, new []{8}),
                ExpBlue(b,2,r,0,ExpData.Types.GroupAtkSpeed, new []{6,7,8,9,10}),
                ExpBlue(b,5,n,1,ExpData.Types.Gold, new []{1}),
                ExpBlue(b,6,r,5,ExpData.Types.CastleMaxHp, new []{200}),
                ExpBlue(b,8,n,1,ExpData.Types.CastleRecovery, new []{200}),
                ExpBlue(b,9,n,1,ExpData.Types.Board, new []{1}),
                ExpBlue(b,11, r,3,ExpData.Types.Slow, new []{1}),
                ExpBlue(b,100,n,1,ExpData.Types.DivineRestraintDamage, new []{100}),
                ExpBlue(b,100,n,1,ExpData.Types.DivineAtkRange, new []{1}),
                ExpBlue(b,200,n,1,ExpData.Types.PhysicSlowAdditionalDamage,new []{100}),
                ExpBlue(b,300,n,1,ExpData.Types.PoisonDoubleAtk, new []{2}),
                ExpBlue(b,400,n,1,ExpData.Types.WaterIncreaseSlowTime, new []{1}),
                ExpBlue(b,400,n,1,ExpData.Types.WaterIncreaseDamage, new []{50})
            };

            ExpPurpleList = new List<ExpData>
            {
                ExpPurple(p,1,r,0,ExpData.Types.GroupDamage,new []{18}),
                ExpPurple(p,2,r,0,ExpData.Types.GroupAtkSpeed, new []{19}),
                ExpPurple(p,3,n,1,ExpData.Types.StepLimit, new []{1} ),
                ExpPurple(p,12,n,1,ExpData.Types.StepDirection, new []{1}),
                ExpPurple(p,10,r,3,ExpData.Types.NextStage, new []{1,2}),
                ExpPurple(p,100,n,1,ExpData.Types.DivineRestraint, new []{1}),
                ExpPurple(p,100,n,1,ExpData.Types.DivinePenetrate, new []{1}),
                ExpPurple(p,200,n,1,ExpData.Types.PhysicAtkSpeed, new []{50}),
                ExpPurple(p,200,n,1,ExpData.Types.PhysicIncreaseDamage, new []{5}),
                ExpPurple(p,200,n,1,ExpData.Types.PhysicIncreaseWeaponScale, new []{5}),
                ExpPurple(p,300,n,1,ExpData.Types.PoisonIncreaseTime, new []{2}),
                ExpPurple(p,300,n,1,ExpData.Types.PoisonInstantKill, new []{15}),
                ExpPurple(p,300,n,1,ExpData.Types.PoisonIncreaseAtkRange, new []{1}),
                ExpPurple(p,400,n,1,ExpData.Types.WaterIncreaseSlowPower, new []{50}),
                ExpPurple(p,400,n,1,ExpData.Types.WaterRestraintKnockBack, new []{1}),
            };
        }
        private ExpGreenData ExpGreen(Sprite image, int code, ExpData.ExpRepeatTypes expRepeatType, int count, ExpData.Types type, IReadOnlyList<int> expProperty)
        {
            var randomProperty = TreasureManager.IncreaseRange(expProperty);
            var powerUp = new ExpGreenData(image, code, expRepeatType, count, type, new int[] { randomProperty });
            ExpGreenList.Add(powerUp);
            return powerUp;
        }
        private ExpBlueData ExpBlue(Sprite image, int code, ExpData.ExpRepeatTypes expRepeatType, int count, ExpData.Types type, IReadOnlyList<int> expProperty)
        {
            var randomProperty = TreasureManager.IncreaseRange(expProperty);
            var powerUp = new ExpBlueData(image, code, expRepeatType, count, type, new int[] { randomProperty });
            ExpBlueList.Add(powerUp);
            return powerUp;
        }
        private ExpPurpleData ExpPurple(Sprite image, int code, ExpData.ExpRepeatTypes expRepeatType, int count, ExpData.Types type, IReadOnlyList<int> expProperty)
        {
            var randomProperty = TreasureManager.IncreaseRange(expProperty);
            var powerUp = new ExpPurpleData(image, code, expRepeatType, count, type, new int[] {randomProperty });
            ExpPurpleList.Add(powerUp);
            return powerUp;
        }
    }
}