using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.RewardScript
{
    public class Data
    {
        private static readonly System.Random Random = new System.Random();
        public int? ChosenProperty;
        public PowerTypeManager.Types Type { get; internal set; }

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
        public Sprite BackGroundColor { get; private set; }
        public Sprite Icon { get; set; }

        protected Data(Sprite btnColor, Sprite backGroundColor, int code, PowerTypeManager.Types type, int[] property)
        {
            foreach (var skill in PowerTypeManager.Instance.skills.Where(skill => skill.skillTypes == type))
            {
                Icon = skill.skillIcon;
            }

            BtnColor = btnColor;
            BackGroundColor = backGroundColor;
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

    public class GreenData : Data
    {
        public GreenData(Sprite btnColor, Sprite backGroundColor, int code, PowerTypeManager.Types type, int[] property)
            : base(btnColor, backGroundColor, code, type, property)
        {
        }
    }

    public class BlueData : Data
    {
        public BlueData(Sprite btnColor, Sprite backGroundColor, int code, PowerTypeManager.Types type, int[] property)
            : base(btnColor, backGroundColor, code, type, property)
        {
        }
    }

    public class PurpleData : Data
    {
        public PurpleData(Sprite btnColor, Sprite backGroundColor, int code, PowerTypeManager.Types type,
            int[] property)
            : base(btnColor, backGroundColor, code, type, property)
        {
        }
    }

    public class PowerTypeManager : MonoBehaviour
    {
        public static PowerTypeManager Instance;

        private void Awake()
        {
            Instance = this;

        var csvFile = Resources.Load<TextAsset>("data");
        if (csvFile == null)
        {
            Debug.LogError("File not found in Resources folder");
            return;
        }

        var g = green;
        var b = blue;
        var p = purple;
        var gBack = greenBack;
        var bBack = blueBack;
        var pBack = purpleBack;
        GreenList = new List<Data>();
        BlueList = new List<Data>();
        PurpleList = new List<Data>();
        var csvData = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        for (var i = 1; i < csvData.Length; i++)
        {
            var data = csvData[i].Split(',');

            Sprite color = null;
            Sprite backGround = null;

            var code = int.Parse(data[1]);
            var type = (Types)Enum.Parse(typeof(Types), data[2]);
            var property = data[3].Contains(" ")
                ? Array.ConvertAll(data[3].Split(' '), int.Parse)
                : new[] { int.Parse(data[3]) };

            switch (data[0])
            {
                case "Green":
                    color = g;
                    backGround = gBack;
                    GreenList.Add(new GreenData(color, backGround, code, type, property));
                    break;
                case "Blue":
                    color = b;
                    backGround = bBack;
                    BlueList.Add(new BlueData(color, backGround, code, type, property));
                    break;
                case "Purple":
                    color = p;
                    backGround = pBack;
                    PurpleList.Add(new PurpleData(color, backGround, code, type, property));
                    break;
                default:
                    Debug.LogWarning("Unknown color type: " + data[0]);
                    break;
            }
        }
        }

        public enum Types
        {
            // Common Property
            Step,
            NextStep,
            StepLimit,
            RandomLevelUp,
            GroupLevelUp,
            LevelUpPattern,
            Match5Upgrade,
            AddRow,
            GroupDamage,
            GroupAtkSpeed,
            StepDirection,
            Exp,
            CastleRecovery,
            CastleMaxHp,
            Slow,
            NextStage,
            Gold,

            //Divine A
            DivineFifthAttackBoost,
            DivineDualAttack,
            DivineBindDurationBoost,
            DivineShackledExplosion,
            DivinePoisonDamageBoost,
            DivineBindChanceBoost,
            DivineRateBoost,

            //Darkness B
            DarkFifthAttackDamageBoost,
            DarkStatusAilmentSlowEffect,
            DarkRangeIncrease,
            DarkAttackPowerBoost,
            DarkStatusAilmentDamageBoost,
            DarkAttackSpeedBoost,
            DarkKnockBackChance,

            //Water1 C
            WaterFreeze,
            WaterFreezeChance,
            WaterSlowDurationBoost,
            WaterFreezeDamageBoost,
            WaterSlowCPowerBoost,
            WaterAttackRateBoost,
            WaterGlobalFreeze,

            //Physical D
            PhysicalSwordScaleIncrease,
            PhysicalSwordAddition,
            PhysicalAttackSpeedBoost,
            PhysicalRatePerAttack,
            PhysicalBindBleed,
            PhysicalDamageBoost,
            PhysicalBleedDuration,

            //Water2 E
            Water2Freeze,
            Water2SlowPowerBoost,
            Water2FreezeTimeBoost,
            Water2DamageBoost,
            Water2FreezeChanceBoost,
            Water2FreezeDamageBoost,
            Water2SlowTimeBoost,

            //Poison F
            PoisonPerHitEffect,
            PoisonBleedingEnemyDamageBoost,
            PoisonDamagePerBoost,
            PoisonDamageBoost,
            PoisonDotDamageBoost,
            PoisonAttackSpeedIncrease,
            PoisonDurationBoost,

            //Fire2 G
            Fire2FreezeDamageBoost,
            Fire2BurnDurationBoost,
            Fire2ChangeProperty,
            Fire2DamageBoost,
            Fire2RangeBoost,
            Fire2RateBoost,
            Fire2BossDamageBoost,

            //Fire1 H
            FireBurnPerAttackEffect,
            FireStackOverlap,
            FireProjectileBounceDamage,
            FireBurnedEnemyExplosion,
            FireAttackSpeedBoost,
            FireProjectileSpeedIncrease,
            FireProjectileBounceIncrease,

            //Poison2 I
            Poison2StunToChance,
            Poison2RangeBoost,
            Poison2DotDamageBoost,
            Poison2StunTimeBoost,
            Poison2SpawnPoisonArea,
            Poison2RateBoost,
            Poison2PoolTimeBoost,

            //Physical2 J
            Physical2CastleCrushStatBoost,
            Physical2FifthBoost,
            Physical2BleedTimeBoost,
            Physical2PoisonDamageBoost,
            Physical2RangeBoost,
            Physical2RateBoost,
            Physical2BossBoost,
            
            //Darkness2 K
            Dark2BackBoost,
            Dark2DualAttack,
            Dark2StatusDamageBoost,
            Dark2ExplosionBoost,
            Dark2DoubleAttack,
            Dark2StatusPoison,
            Dark2SameEnemyBoost,

        }

        [Serializable]
        public class Skill
        {
            public Types skillTypes;
            public Sprite skillIcon;
        }

        [SerializeField] public List<Skill> skills = new List<Skill>();

        public List<Data> GreenList { get; private set; } = new List<Data>();
        public List<Data> BlueList { get; private set; } = new List<Data>();
        public List<Data> PurpleList { get; private set; } = new List<Data>();

        [SerializeField] internal Sprite green;
        [SerializeField] internal Sprite blue;
        [SerializeField] internal Sprite purple;
        [SerializeField] internal Sprite greenBack;
        [SerializeField] internal Sprite blueBack;
        [SerializeField] internal Sprite purpleBack;
    }
}
