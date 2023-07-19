using System;
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
             StepDirection,
             Exp,
             CastleRecovery,
             CastleMaxHp,
             Slow,
             NextStage,
             Gold,

             //Divine
             DivinePoisonDamageBoost,
             DivineBindDurationBoost,
             DivineShackledExplosion,
             DivineFifthAttackBoost,
             DivineUnitOneAttackBoost,
             DivineBindChanceBoost,
             DivineUnitOneDualAttack,
             DivineProjectilePierce,
             //Physical
             PhysicalAttackSpeedBoost,
             PhysicalDamage100Boost,
             PhysicalDamage9Boost,
             PhysicalBleedingChance,
             PhysicalSwordAddition,
             PhysicalSlowEnemyDamageBoost,
             PhysicalSwordScaleIncrease,
             PhysicalDamage24Boost,
             //Poison
             PoisonAilmentStun,
             PoisonMaxStackIncrease,
             PoisonDamageAttackPowerIncrease,
             PoisonProjectileIncrease,
             PoisonRangeIncrease,
             PoisonBleedingEnemyDamageBoost,
             PoisonBleedingEnemyInstantKill,
             PoisonPerHitEffect,
             //Fire1
             FireImageOverlapIncrease,
             FireUnitSixAttackSpeedBoost,
             FireSlowEnemyDamageBoost,
             FireProjectileSpeedIncrease,
             FireBurnedEnemyExplosion,
             FireProjectileBounceDamage,
             FireBurnPerAttackEffect,
             FireProjectileBounceIncrease,
             //Water1
             WaterUnitEightAttackSpeedBoost,
             WaterAllyDamageBoost,
             WaterProjectileIncrease,
             WaterUnitEightAttackBoost,
             WaterSlowEnemyDamageBoost,
             WaterGlobalSlowEffect,
             WaterSlowEnemyStunChance,
             WaterDamageIncreaseDebuff,
             //Fire2
             FirePoisonDamageIncrease,
             Fire2AttackSpeedIncrease,
             Fire2BleedingDamageIncrease,
             Fire2AttackPowerIncrease,
             Fire2StunChance,
             Fire2SwordSizeIncrease,
             Fire2RangeIncrease,
             Fire2NoBurnDamageIncrease,
             //Water2
             Water2DebuffDurationIncrease,
             Water2AttackSpeedIncrease,
             Water2StunChanceAgainstBleeding,
             Water2IceSpikeProjectile,
             Water2AttackPowerIncrease,
             Water2ProjectileSpeedIncrease,
             Water2DebuffStrengthIncrease,
             Water2AttackSpeedBuffToAdjacentAllies,
             //Darkness
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
            var csvFile = Resources.Load<TextAsset>("expData");
            if (csvFile == null)
            {
                Debug.LogError("File not found in Resources folder");
                return;
            }

            var g = levelUpRewardManager.greenSprite;
            var b = levelUpRewardManager.blueSprite;
            var p = levelUpRewardManager.purpleSprite;

            ExpGreenList = new List<ExpData>();
            ExpBlueList = new List<ExpData>();
            ExpPurpleList = new List<ExpData>();

            var csvData = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    
            for(var i = 1; i < csvData.Length; i++)
            {
                var data = csvData[i].Split(',');

                var color = data[0] switch
                {
                    "Green" => g,
                    "Blue" => b,
                    _ => p
                };

                var code = int.Parse(data[1]);

                var type = (ExpData.Types)Enum.Parse(typeof(ExpData.Types), data[2]);

                var property = data[3].Contains(" ") ? Array.ConvertAll(data[3].Split(' '), int.Parse) : new [] { int.Parse(data[3]) };

                switch (data[0])
                {
                    case "Green":
                        ExpGreenList.Add(new ExpGreenData(color, code, type, property));
                        break;
                    case "Blue":
                        ExpBlueList.Add(new ExpBlueData(color, code, type, property));
                        break;
                    default:
                        ExpPurpleList.Add(new ExpPurpleData(color, code, type, property));
                        break;
                }
            }
        }
    }
}
