using System;
using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
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

        public CharacterBase.UnitGroups SkillGroup { get; set; }
        public int SkillLevel { get; set; }
        public int Code { get; set; }
        public Sprite BtnColor { get; private set; }
        public Sprite BackGroundColor { get; private set; }
        public Sprite Icon { get; set; }
        public string Desc { get; set; }
        public string PopupDesc { get; set; }

        protected Data(
            CharacterBase.UnitGroups skillGroup, 
            int skillLevel, 
            int code, 
            PowerTypeManager.Types type, 
            Sprite btnColor,
            Sprite backGroundColor,
            string desc, 
            string popupDesc, 
            int[] property)
        {
            foreach (var skill in PowerTypeManager.Instance.skills.Where(skill => skill.skillTypes == type))
            {
                Icon = skill.skillIcon;
            }
            SkillGroup = skillGroup;
            SkillLevel = skillLevel;
            BtnColor = btnColor;
            BackGroundColor = backGroundColor;
            Code = code;
            Type = type;
            Desc = desc;
            PopupDesc = popupDesc;
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
        public GreenData(CharacterBase.UnitGroups skillGroup, 
            int skillLevel, 
            int code, 
            PowerTypeManager.Types type, 
            Sprite btnColor,
            Sprite backGroundColor,
            string desc, 
            string popupDesc, 
            int[] property)
            : base(skillGroup, skillLevel, code, type, btnColor, backGroundColor, desc, popupDesc, property)
        {
        }
    }

    public class BlueData : Data
    {
        public BlueData(CharacterBase.UnitGroups skillGroup, 
            int skillLevel, 
            int code, 
            PowerTypeManager.Types type, 
            Sprite btnColor,
            Sprite backGroundColor,
            string desc, 
            string popupDesc, 
            int[] property)
            : base(skillGroup, skillLevel, code, type, btnColor, backGroundColor, desc, popupDesc, property)
        {
        }
    }

    public class PurpleData : Data
    {
        public PurpleData(CharacterBase.UnitGroups skillGroup, 
            int skillLevel, 
            int code, 
            PowerTypeManager.Types type, 
            Sprite btnColor,
            Sprite backGroundColor,
            string desc, 
            string popupDesc, 
            int[] property)
            : base(skillGroup, skillLevel, code, type, btnColor, backGroundColor, desc, popupDesc, property)
        {
        }
    }

    public class PowerTypeManager : MonoBehaviour
    {
        public static PowerTypeManager Instance;
        public string[] skillData;
        private void Awake()
        {
            Instance = this;

        var csvFile = Resources.Load<TextAsset>("SkillData");
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
            skillData = csvData[i].Split(',');

            Sprite btnColor;
            Sprite backGroundColor;
            var skillGroup = (CharacterBase.UnitGroups)Enum.Parse(typeof(CharacterBase.UnitGroups), skillData[0]);
            var skillLevel = int.Parse(skillData[1]);
            var code = int.Parse(skillData[2]);
            var type = (Types)Enum.Parse(typeof(Types), skillData[3]);
            var desc = skillData[6];
            var popupDesc = skillData[7];
            var property = skillData[8].Contains(" ")
                ? Array.ConvertAll(skillData[8].Split(' '), int.Parse)
                : new[] { int.Parse(skillData[8]) };

            switch (skillData[4])
            {
                case "G":
                    btnColor = g;
                    backGroundColor = gBack;
                    GreenList.Add(new GreenData(skillGroup, skillLevel, code, type, btnColor, backGroundColor, desc, popupDesc, property));
                    break;
                case "B":
                    btnColor = b;
                    backGroundColor = bBack;
                    BlueList.Add(new BlueData(skillGroup, skillLevel, code, type, btnColor, backGroundColor, desc, popupDesc, property));
                    break;
                case "P":
                    btnColor = p;
                    backGroundColor = pBack;
                    PurpleList.Add(new PurpleData(skillGroup, skillLevel, code, type, btnColor, backGroundColor, desc, popupDesc, property));
                    break;
                default:
                    Debug.LogWarning("Unknown color type: " + skillData[4]);
                    break;
            }
        }
        }

        public enum Types
        {
            // Common Property
            Step,
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

            //Darkness3 Octopus
            OctopusThirdAttackBoost,
            OctopusPoisonAttack,
            OctopusBleedDamageBoost,
            OctopusShackledExplosion,
            OctopusBleedDurationBoost,
            OctopusDamageBoost,
            OctopusRateBoost,

            //Darkness Ogre
            OgreThirdAttackDamageBoost,
            OgreStatusAilmentSlowEffect,
            OgreRangeIncrease,
            OgreAttackPowerBoost,
            OgreStatusAilmentDamageBoost,
            OgreAttackSpeedBoost,
            OgreKnockBackChance,

            //Water1 DeathChiller
            DeathChillerFreeze,
            DeathChillerFreezeChance,
            DeathChillerSlowDurationBoost,
            DeathChillerFreezeDamageBoost,
            DeathChillerSlowCPowerBoost,
            DeathChillerAttackRateBoost,
            DeathChillerGlobalFreeze,

            //Physical Orc
            OrcSwordScaleIncrease,
            OrcSwordAddition,
            OrcAttackSpeedBoost,
            OrcRatePerAttack,
            OrcBindBleed,
            OrcDamageBoost,
            OrcBleedDuration,

            //Water2 Fishman
            FishmanFreeze,
            FishmanSlowPowerBoost,
            FishmanFreezeTimeBoost,
            FishmanDamageBoost,
            FishmanFreezeChanceBoost,
            FishmanFreezeDamageBoost,
            FishmanSlowTimeBoost,

            //Poison Skeleton
            SkeletonPerHitEffect,
            SkeletonBleedingEnemyDamageBoost,
            SkeletonDamagePerBoost,
            SkeletonDamageBoost,
            SkeletonDotDamageBoost,
            SkeletonAttackSpeedIncrease,
            SkeletonDurationBoost,

            //Fire2 Phoenix
            PhoenixFreezeDamageBoost,
            PhoenixBurnDurationBoost,
            PhoenixChangeProperty,
            PhoenixDamageBoost,
            PhoenixRangeBoost,
            PhoenixRateBoost,
            PhoenixBossDamageBoost,

            //Fire1 Beholder
            BeholderBurnPerAttackEffect,
            BeholderStackOverlap,
            BeholderProjectileBounceDamage,
            BeholderBurnedEnemyExplosion,
            BeholderAttackSpeedBoost,
            BeholderProjectileSpeedIncrease,
            BeholderProjectileBounceIncrease,

            //Poison2 Cobra
            CobraStunToChance,
            CobraRangeBoost,
            CobraDotDamageBoost,
            CobraStunTimeBoost,
            CobraSpawnPoisonArea,
            CobraRateBoost,
            CobraPoolTimeBoost,

            //Physical2 Berserker
            BerserkerCastleCrushStatBoost,
            BerserkerThirdBoost,
            BerserkerBleedTimeBoost,
            BerserkerPoisonDamageBoost,
            BerserkerRangeBoost,
            BerserkerRateBoost,
            BerserkerBossBoost,
            
            //Darkness2 DarkElf
            DarkElfBackBoost,
            DarkElfDualAttack,
            DarkElfStatusDamageBoost,
            DarkElfExplosionBoost,
            DarkElfDoubleAttack,
            DarkElfStatusPoison,
            DarkElfSameEnemyBoost,

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
