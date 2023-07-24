using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.RewardScript
{
    public class CommonData
    {
        private static readonly System.Random Random = new System.Random();
        public int? ChosenProperty;

        public enum Types
        {
            GroupDamage, 
            GroupAtkSpeed, 
            Step, 
            StepLimit, 
            StepDirection , 
            RandomLevelUp, 
            GroupLevelUp, 
            LevelUpPattern, 
            Exp, 
            CastleRecovery, 
            CastleMaxHp, 
            Match5Upgrade, 
            Slow,
            NextStage, 
            Gold, 
            AddRow
        }
        public Types Type { get; private set; }
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
            var csvFile = Resources.Load<TextAsset>("commonData");
            if (csvFile == null)
            {
                Debug.LogError("File not found in Resources folder");
                return;
            }

            var g = commonRewardManager.greenSprite;
            var b = commonRewardManager.blueSprite;
            var p = commonRewardManager.purpleSprite;
            CommonGreenList = new List<CommonData>();
            CommonBlueList = new List<CommonData>();
            CommonPurpleList = new List<CommonData>();
            var csvData = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (var i = 1; i < csvData.Length; i++)
            {
                var data = csvData[i].Split(',');

                var color = data[0] switch
                {
                    "Green" => g,
                    "Blue" => b,
                    _ => p
                };

                var code = int.Parse(data[1]);
                var type = (CommonData.Types)Enum.Parse(typeof(CommonData.Types), data[2]);
                var property = data[3].Contains(" ")
                    ? Array.ConvertAll(data[3].Split(' '), int.Parse)
                    : new [] { int.Parse(data[3]) };

                switch (data[0])
                {
                    case "Green":
                        CommonGreenList.Add(new CommonGreenData(color, code, type, property));
                        break;
                    case "Blue":
                        CommonBlueList.Add(new CommonBlueData(color, code, type, property));
                        break;
                    default:
                        CommonPurpleList.Add(new CommonPurpleData(color, code, type, property));
                        break;
                }
            }
        }
    }
}