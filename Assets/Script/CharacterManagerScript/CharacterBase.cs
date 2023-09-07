using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.EnemyManagerScript;
using Script.QuestGroup;
using Script.RewardScript;
using Script.RobbyScript.TopMenuGroup;
using UnityEngine.Serialization;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] public GameObject cover;
        [FormerlySerializedAs("unitPeaceLevel")] public int unitPieceLevel = 1;
        protected internal int UnitPieceCount { get; set; }
        protected internal int UnitPieceMaxPiece => CheckForMaxPeace();
        protected internal int CharacterLevelUpCoin => CheckForLevelUpCoin();
        protected internal int CumulativeDamage;
        public enum UnitGrades { G, B, P }
        protected internal UnitGrades UnitGrade;
        public bool unLock;
        public bool selected;
        public int unitPuzzleLevel;
        public enum Types { Character, Treasure }
        protected internal Types Type; 
        public enum UnitGroups { Octopus,Ogre,DeathChiller,Orc,Fishman,Skeleton,Phoenix,Beholder,Cobra,Berserker,DarkElf,None }
        public enum UnitAtkTypes {None, Projectile, GuideProjectile, Circle}
        public enum UnitProperties { Darkness, Physics, Water, Poison, Fire, None }
        public enum UnitEffects { Slow, Bleed, Poison, Burn, Bind, None } 
        public UnitGroups unitGroup = UnitGroups.None; 
        protected internal UnitAtkTypes UnitAtkType = UnitAtkTypes.None;
        protected internal UnitProperties UnitProperty = UnitProperties.None;
        protected internal UnitEffects UnitEffect = UnitEffects.None;
        protected internal Dictionary<int, Dictionary<Sprite, Sprite>> UnitSkillDict = new Dictionary<int, Dictionary<Sprite, Sprite>>();
        [SerializeField] internal Sprite lv1;
        [SerializeField] internal Sprite lv1Lock;
        [SerializeField] internal Sprite lv3;
        [SerializeField] internal Sprite lv3Lock;
        [SerializeField] internal Sprite lv5;
        [SerializeField] internal Sprite lv5Lock;
        [SerializeField] internal Sprite lv7;
        [SerializeField] internal Sprite lv7Lock;
        [SerializeField] internal Sprite lv9;
        [SerializeField] internal Sprite lv9Lock;
        [SerializeField] internal Sprite lv11;
        [SerializeField] internal Sprite lv11Lock;
        [SerializeField] internal Sprite lv13;
        [SerializeField] internal Sprite lv13Lock;
        public int baseDamage;
        public float dotDamage;
        public int effectChance;
        public int effectStack;
        public float slowTime;
        public float slowPower;
        public float bindTime;
        public float freezeTime;
        public float knockBackPower;
        public float knockBackTime;
        public float bleedTime;
        public float poisonTime;
        public float burnTime;
        public float stunTime;
        public float poisonAreaTime;
        public float DefaultDamage
        {
            get
            {
                var increaseDamageAmount = EnforceManager.Instance.increaseAtkDamage;
                return baseDamage * (1.0f + (increaseDamageAmount / 100f));
            }
            protected set => baseDamage = (int)value;
        }
        public float defaultAtkRate;
        public float projectileSpeed; 
        public float swingSpeed;
        public float defaultAtkDistance;
        private readonly Vector3 _initialScale = Vector3.one; 
        private readonly Vector3 _levelUpScale = new Vector3(1.3f, 1.3f, 0); 
        public GameObject CurrentWeapon { get; set; }
        protected internal bool IsClicked { get; set; }
        protected static List<GameObject> DetectedEnemies = new List<GameObject>();
        private SpriteRenderer _spriteRenderer;
        protected internal string UnitDesc { get; protected set; }
        public float UnitLevelDamage { get; protected set; }
        public Dictionary<EnemyBase, int> AttackCounts { get; set; } = new Dictionary<EnemyBase, int>();
        private const string LevelKey = "level";
        protected internal const string SelectKey = "Select";
        protected internal const string UnLockKey = "Unlock";
        protected internal const string PieceKey = "Piece";
        public virtual void Initialize()
        {
            if (bool.Parse(PlayerPrefs.GetString("TutorialKey", "true")))
            {
                switch (unitGroup)
                {
                    case UnitGroups.Octopus:
                        selected = false;
                        unLock = false;
                        break;
                    case UnitGroups.Ogre:
                        selected = true;
                        unLock = true;
                        PlayerPrefs.SetString($"{unitGroup}{SelectKey}","true");
                        PlayerPrefs.SetString($"{unitGroup}{UnLockKey}","true");
                        break;
                    case UnitGroups.DeathChiller:
                        selected = false;
                        unLock = false;
                        break;
                    case UnitGroups.Orc:
                        selected = true;
                        unLock = true;
                        PlayerPrefs.SetString($"{unitGroup}{SelectKey}","true");
                        PlayerPrefs.SetString($"{unitGroup}{UnLockKey}","true");
                        break;
                    case UnitGroups.Fishman:
                        selected = true;
                        unLock = true;
                        PlayerPrefs.SetString($"{unitGroup}{SelectKey}","true");
                        PlayerPrefs.SetString($"{unitGroup}{UnLockKey}","true");
                        break;
                    case UnitGroups.Skeleton:
                        selected = true;
                        unLock = true;
                        PlayerPrefs.SetString($"{unitGroup}{SelectKey}","true");
                        PlayerPrefs.SetString($"{unitGroup}{UnLockKey}","true");
                        break;
                    case UnitGroups.Phoenix:
                        selected = false;
                        unLock = false;
                        break;
                    case UnitGroups.Beholder:
                        selected = false;
                        unLock = false;
                        break;
                    case UnitGroups.Cobra:
                        selected = false;
                        unLock = false;
                        break;
                    case UnitGroups.Berserker:
                        selected = false;
                        unLock = false;
                        break;
                    case UnitGroups.DarkElf:
                        selected = false;
                        unLock = false;
                        break;
                }
            }
            else
            {
                selected = bool.Parse(PlayerPrefs.GetString($"{unitGroup}{SelectKey}", "false"));
                unLock = bool.Parse(PlayerPrefs.GetString($"{unitGroup}{UnLockKey}","false"));
            }
            var level = UnitGrade switch
            {
                UnitGrades.B => 3,
                UnitGrades.P => 5,
                _ => 1,
            };
            unitPieceLevel = PlayerPrefs.GetInt($"{unitGroup}{LevelKey}", level);
            UnitPieceCount = PlayerPrefs.GetInt($"{unitGroup}{PieceKey}", 0);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (unitGroup != UnitGroups.None)
            {
                UnitSkillDict = new Dictionary<int, Dictionary<Sprite, Sprite>>
                {
                    {1, new Dictionary<Sprite, Sprite>{{lv1, lv1Lock}}}, 
                    {3, new Dictionary<Sprite, Sprite>{{lv3, lv3Lock}}}, 
                    {5, new Dictionary<Sprite, Sprite>{{lv5, lv5Lock}}}, 
                    {7,new Dictionary<Sprite, Sprite>{{lv7, lv7Lock}}}, 
                    {9, new Dictionary<Sprite, Sprite>{{lv9, lv9Lock}}}, 
                    {11, new Dictionary<Sprite, Sprite>{{lv11, lv11Lock}}}, 
                    {13, new Dictionary<Sprite, Sprite>{{lv13, lv13Lock}}}
                };
            }
        }
        private int CheckForMaxPeace()
        {
            var maxPiece = 0;
            switch (unitPieceLevel)
            {
                case 1:
                    maxPiece = UnitGrade switch
                    {
                        UnitGrades.G => 5
                    };
                    break;
                case 2:
                    maxPiece = UnitGrade switch
                    {
                        UnitGrades.G => 10
                    };
                    break;
                case 3:
                    maxPiece = UnitGrade switch
                    {
                        UnitGrades.G => 25,
                        UnitGrades.B => 5,
                    };
                    break;
                case 4:
                    maxPiece = UnitGrade switch
                    {
                        UnitGrades.G => 50,
                        UnitGrades.B => 10,
                    };
                    break;
                 case 5:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 100,
                         UnitGrades.B => 25,
                         UnitGrades.P => 5,
                     };
                     break;
                 case 6:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 200,
                         UnitGrades.B => 50,
                         UnitGrades.P => 10
                     };
                     break;
                 case 7:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 500,
                         UnitGrades.B => 100,
                         UnitGrades.P => 20
                     };
                     break;
                 case 8:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 1000,
                         UnitGrades.B => 200,
                         UnitGrades.P => 40
                     };
                     break;
                 case 9:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 2000,
                         UnitGrades.B => 400,
                         UnitGrades.P => 80
                     };
                     break;
                 case 10:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 4000,
                         UnitGrades.B => 800,
                         UnitGrades.P => 160
                     };
                     break;
                 case 11:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 6000,
                         UnitGrades.B => 1600,
                         UnitGrades.P => 320
                     };
                     break;
                 case 12:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 8000,
                         UnitGrades.B => 3200,
                         UnitGrades.P => 480
                     };
                     break;
                 case 13:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.G => 8000,
                         UnitGrades.B => 3200,
                         UnitGrades.P => 480
                     };
                     break;
            }
            return maxPiece;
        }
        private int CheckForLevelUpCoin()
        {
            var coin = 0;
            switch (unitPieceLevel)
            {
                case 1:
                    coin = 100;
                    break;
                case 2:
                    coin = UnitGrade switch
                    {
                        UnitGrades.G => 250,
                        UnitGrades.B => 0
                    };
                    break;
                case 3:
                    coin = UnitGrade switch
                    {
                        UnitGrades.G => 500,
                        UnitGrades.B => 500
                    };
                    break;
                case 4:
                    coin = UnitGrade switch
                    {
                        UnitGrades.G => 1000,
                        UnitGrades.B => 1000,
                        UnitGrades.P => 0
                    };
                    break;
                 case 5:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 2500,
                         UnitGrades.B => 1500,
                         UnitGrades.P => 2500
                     };
                     break;
                 case 6:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 5000,
                         UnitGrades.B => 3000,
                         UnitGrades.P => 7500
                     };
                     break;
                 case 7:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 7500,
                         UnitGrades.B => 6000,
                         UnitGrades.P => 13000
                     };
                     break;
                 case 8:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 10000,
                         UnitGrades.B => 12000,
                         UnitGrades.P => 26000
                     };
                     break;
                 case 9:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 12500,
                         UnitGrades.B => 24000,
                         UnitGrades.P => 40000
                     };
                     break;
                 case 10:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 15000,
                         UnitGrades.B => 36000,
                         UnitGrades.P => 20000
                     };
                     break;
                 case 11:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 15000,
                         UnitGrades.B => 48000,
                         UnitGrades.P => 30000
                     };
                     break;
                 case 12:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 15000,
                         UnitGrades.B => 54000,
                         UnitGrades.P => 40000
                     };
                     break;
                 case 13:
                     coin = UnitGrade switch
                     {
                         UnitGrades.G => 15000,
                         UnitGrades.B => 66000,
                         UnitGrades.P => 50000
                     };
                     break;
            }
            return coin;
        }
        public void LevelUpScale(GameObject levelUpObject)
        {
            var sequence = DOTween.Sequence();
            Tween scaleUp = sequence.Append(levelUpObject.transform.DOScale(_levelUpScale, 0.3f)); 
            scaleUp.WaitForCompletion();
            Tween scaleDown = sequence.Append(levelUpObject.transform.DOScale(_initialScale, 0.3f)); 
            scaleDown.WaitForCompletion();
            LevelUp();
        }
        protected virtual void LevelUp()
        {
            unitPuzzleLevel++;
        }
        protected internal virtual void CharacterReset()
        {
            unitPuzzleLevel = 1;
        }
        public virtual List<GameObject> DetectEnemies()
        {
            return new List<GameObject>();
        }
        public virtual Sprite GetSpriteForLevel(int characterObjectLevel)
        {
            return null;
        }
        public IEnumerator UnitLevelUp(UnitGroups unitGroups)
        {
            UnitPieceCount -= UnitPieceMaxPiece;

            CoinsScript.Instance.Coin -= CharacterLevelUpCoin;
            unitPieceLevel++;
            PlayerPrefs.SetInt($"{unitGroups}{LevelKey}", unitPieceLevel);
            PlayerPrefs.SetInt($"{unitGroups}{PieceKey}", UnitPieceCount);
            PlayerPrefs.Save();
            Initialize();
            Quest.Instance.UseCoinQuest(CharacterLevelUpCoin);
            yield return null;
        }
        protected internal virtual Sprite GetSprite(int level)
        {
            return null;
        }

        protected internal virtual Sprite GetBasicSprite()
        {
            return null;
        }
        protected internal virtual void SetLevel(int level)
        {
            unitPuzzleLevel = level;
            _spriteRenderer.sprite = GetSprite(level);
        }
    }
}
