using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RewardScript
{
    public class CommonData
    {
        public enum Types 
        { GroupDamage, GroupAtkSpeed, Step, StepLimit, StepDirection , RandomLevelUp, LevelUp, LevelUpPattern, Exp, CastleRecovery, CastleMaxHp, Board, Slow, NextStage, Gold, AddRow}
        public Types Type { get; private set; }
        public int[] Property { get; private set; }
        public int Code { get; set; }
        public Sprite BtnColor { get; private set; }
        public int Count { get; set; }
        public enum CommonRepeatTypes { Repeat, NoneRepeat }
        public CommonRepeatTypes CommonRepeat { get; set; }

        protected CommonData(Sprite btnColor, int code, CommonRepeatTypes commonRepeat, int count, Types type, int[] property)
        {
            BtnColor = btnColor;
            Code = code;
            CommonRepeat = commonRepeat;
            Count = count;
            Type = type;
            Property = property;
        }
    }
    public class CommonGreenData : CommonData
    {
        public CommonGreenData(Sprite btnColor, int code, CommonRepeatTypes commonRepeat, int count, Types type, int[] property)
            : base(btnColor, code, commonRepeat, count, type, property)
        {
        }
    }
    public class CommonBlueData : CommonData
    {
        public CommonBlueData(Sprite btnColor, int code, CommonRepeatTypes commonRepeat, int count, Types type, int[] property)
            : base(btnColor, code, commonRepeat, count, type, property)
        {
        }
    }
    public class CommonPurpleData : CommonData
    {
        public CommonPurpleData(Sprite btnColor, int code, CommonRepeatTypes commonRepeat, int count, Types type, int[] property)
            : base(btnColor, code, commonRepeat, count, type, property)
        {
        }
    }

    public class Common : MonoBehaviour
    {
        public List<CommonData> CommonGreenList { get; private set; } = new List<CommonData>();
        public List<CommonData> CommonBlueList { get; private set; } = new List<CommonData>();
        public List<CommonData> CommonPurpleList { get; private set; } = new List<CommonData>();
        [SerializeField] private CommonRewardManager commonRewardManager;
        public void Start()
        {
            var g = commonRewardManager.greenSprite;
            var b = commonRewardManager.blueSprite;
            var p = commonRewardManager.purpleSprite;
            const CommonData.CommonRepeatTypes r = CommonData.CommonRepeatTypes.Repeat;
            const CommonData.CommonRepeatTypes n = CommonData.CommonRepeatTypes.NoneRepeat;
            CommonGreenList = new List<CommonData>
            {
                CommonGreen(g,1, r,0, CommonData.Types.GroupDamage, new[]{4}),
                CommonGreen(g,2,r,0,CommonData.Types.GroupAtkSpeed,new[]{4}),
                CommonGreen(g,3,r,0,CommonData.Types.Step,new []{2,3,4}),
                CommonGreen(g,4,n,1,CommonData.Types.RandomLevelUp, new []{2,3}),
                CommonGreen(g,4,n,1,CommonData.Types.LevelUp, new []{1}),
                CommonGreen(g,4,r,0,CommonData.Types.LevelUpPattern, new []{1}),
                CommonGreen(g,7,r,6, CommonData.Types.Exp, new []{5}),
            };

            CommonBlueList = new List<CommonData>
            {
                CommonBlue(b,1,r,0,CommonData.Types.GroupDamage, new []{8}),
                CommonBlue(b,2,r,0,CommonData.Types.GroupAtkSpeed, new []{6,7,8,9,10}),
                CommonBlue(b,3,r,0,CommonData.Types.Step, new []{7,8,9,10}),
                CommonBlue(b,5,n,1,CommonData.Types.Gold, new []{1}),
                CommonBlue(b,6,r,5,CommonData.Types.CastleMaxHp, new []{200}),
                CommonBlue(b,8,n,1,CommonData.Types.CastleRecovery, new []{200}),
                CommonBlue(b,9,n,1,CommonData.Types.Board, new []{1}),
                CommonBlue(b,11, r,3,CommonData.Types.Slow, new []{15})
            };

            CommonPurpleList = new List<CommonData>
            {
                CommonPurple(p,1,r,0,CommonData.Types.GroupDamage,new []{18}),
                CommonPurple(p,2,r,0,CommonData.Types.GroupAtkSpeed, new []{19}),
                CommonPurple(p,3,r,0,CommonData.Types.Step, new []{14,15,16,17}),
                CommonPurple(p,3,n,1,CommonData.Types.StepLimit, new []{1} ),
                CommonPurple(p,4,n,1,CommonData.Types.LevelUpPattern, new []{1}),
                CommonPurple(p,12,n,1,CommonData.Types.StepDirection, new []{1}),
                CommonPurple(p,10,r,3,CommonData.Types.NextStage, new []{1,2}),
                CommonPurple(p,13, n, 1, CommonData.Types.AddRow, new []{1})
            };
        }
        private CommonGreenData CommonGreen(Sprite btnColor, int powerCode, CommonData.CommonRepeatTypes commonRepeatType, int powerUpCount, CommonData.Types type, IReadOnlyList<int> commonProperty)
        {
            var randomProperty = CommonRewardManager.RandomChanceMethod(commonProperty);
            var powerUp = new CommonGreenData(btnColor, powerCode, commonRepeatType, powerUpCount, type, new int[] { randomProperty });
            CommonGreenList.Add(powerUp);
            return powerUp;
        }
        private CommonBlueData CommonBlue(Sprite btnColor, int powerCode, CommonData.CommonRepeatTypes commonRepeatType, int powerUpCount, CommonData.Types type, IReadOnlyList<int> powerUpProperty)
        {
            var randomProperty = CommonRewardManager.RandomChanceMethod(powerUpProperty);
            var powerUp = new CommonBlueData(btnColor, powerCode, commonRepeatType, powerUpCount, type, new int[] { randomProperty });
            CommonBlueList.Add(powerUp);
            return powerUp;
        }
        private CommonPurpleData CommonPurple(Sprite btnColor, int powerCode, CommonData.CommonRepeatTypes commonRepeatType, int powerUpCount, CommonData.Types type, IReadOnlyList<int> powerUpProperty)
        {
            var randomProperty = CommonRewardManager.RandomChanceMethod(powerUpProperty);
            var powerUp = new CommonPurpleData(btnColor, powerCode, commonRepeatType, powerUpCount, type, new int[] { randomProperty });
            CommonPurpleList.Add(powerUp);
            return powerUp;
        }
    }
}
