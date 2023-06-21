using System.Collections.Generic;
using UnityEngine;

namespace Script.RewardScript
{
    public class CommonData
    {
        private static readonly System.Random Random = new System.Random();
        public int? chosenProperty;
        public enum Types 
        { GroupDamage, GroupAtkSpeed, Step, StepLimit, StepDirection , RandomLevelUp, GroupLevelUp, LevelUpPattern, Exp, CastleRecovery, CastleMaxHp, Match5Upgrade, Slow, NextStage, Gold, AddRow}
        public Types Type { get; private set; }
        private readonly int[] _property;
        public int[] Property 
        { 
            get
            {
                chosenProperty ??= RandomChanceMethod(_property);
                return new[] { chosenProperty.Value };
            } 
        }
        public int Code { get; set; }
        public Sprite BtnColor { get; private set; }

        protected CommonData(Sprite btnColor, int code, Types type, int[] property)
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

    public class CommonGreenData : CommonData
    {
        public CommonGreenData(Sprite btnColor, int code,Types type, int[] property)
            : base(btnColor, code, type, property)
        {
        }
    }
    public class CommonBlueData : CommonData
    {
        public CommonBlueData(Sprite btnColor, int code, Types type, int[] property)
            : base(btnColor, code,type, property)
        {
        }
    }
    public class CommonPurpleData : CommonData
    {
        public CommonPurpleData(Sprite btnColor, int code, Types type, int[] property)
            : base(btnColor, code, type, property)
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
            CommonGreenList = new List<CommonData>
            {
                new CommonGreenData(g,1, CommonData.Types.GroupDamage, new[]{4}),
                new CommonGreenData(g,2,CommonData.Types.GroupAtkSpeed,new[]{4}),
                new CommonGreenData(g,3,CommonData.Types.Step,new []{2,3,4}),
                new CommonGreenData(g,4,CommonData.Types.RandomLevelUp, new []{2,3}),
                new CommonGreenData(g,5,CommonData.Types.GroupLevelUp, new []{0,1,2,3}),
                new CommonGreenData(g,6,CommonData.Types.Exp, new []{5}),
            };

            CommonBlueList = new List<CommonData>
            {
                new CommonBlueData(b,1,CommonData.Types.GroupDamage, new []{8}),
                new CommonBlueData(b,2,CommonData.Types.GroupAtkSpeed, new []{6,7,8,9,10}),
                new CommonBlueData(b,3,CommonData.Types.Step, new []{7,8,9,10}),
                new CommonBlueData(b,7,CommonData.Types.Gold, new []{1}),
                new CommonBlueData(b,8,CommonData.Types.CastleMaxHp, new []{200}),
                new CommonBlueData(b,9,CommonData.Types.CastleRecovery, new []{200}),
                new CommonBlueData(b,11,CommonData.Types.Slow, new []{15})
            };

            CommonPurpleList = new List<CommonData>
            {
                new CommonPurpleData(p,1,CommonData.Types.GroupDamage,new []{18}),
                new CommonPurpleData(p,2,CommonData.Types.GroupAtkSpeed, new []{19}),
                new CommonPurpleData(p,3,CommonData.Types.Step, new []{14,15,16,17}),
                new CommonPurpleData(p,12,CommonData.Types.StepLimit, new []{1} ),
                new CommonPurpleData(p,13,CommonData.Types.StepDirection, new []{1}),
                new CommonPurpleData(p,14,CommonData.Types.NextStage, new []{1,2}),
                new CommonPurpleData(p,15,CommonData.Types.LevelUpPattern, new []{0,1,2,3}),
                new CommonPurpleData(p,10,CommonData.Types.Match5Upgrade, new []{1}),
                new CommonPurpleData(p,16,CommonData.Types.AddRow, new []{1})
            };
        }
    }
}