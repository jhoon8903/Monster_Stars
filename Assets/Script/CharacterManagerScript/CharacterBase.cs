using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script.EnemyManagerScript;
using Script.RewardScript;
using Script.RobbyScript.TopMenuGroup;
using UnityEngine.Serialization;

namespace Script.CharacterManagerScript
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] public GameObject cover;
        public int unitPeaceLevel = 1;
        protected internal int CharacterPeaceCount { get; set; }
        protected internal int CharacterMaxPeace => CheckForMaxPeace();
        protected internal int CharacterLevelUpCoin => CheckForLevelUpCoin();
        public enum UnitGrades { Green, Blue, Purple }
        protected internal UnitGrades UnitGrade;
        public bool unLock;
        public bool selected;
        public int unitPuzzleLevel;
        public enum Types { Character, Treasure }
        protected internal Types Type; 
        public enum UnitGroups { A,B,C,D,E,F,G,H,I,J,K,None }
        public enum UnitAtkTypes {None, Projectile, GuideProjectile, Circle}
        public enum UnitProperties { Divine, Darkness, Physics, Water, Poison, Fire, None }
        public enum UnitEffects { Slow, Bleed, Poison, Burn, Bind, None } 
        public UnitGroups unitGroup = UnitGroups.None; 
        protected internal UnitAtkTypes UnitAtkType = UnitAtkTypes.None;
        protected internal UnitProperties UnitProperty = UnitProperties.None;
        protected internal UnitEffects UnitEffect = UnitEffects.None;
        protected internal Dictionary<int, Sprite> UnitSkillDict = new Dictionary<int, Sprite>();
        [SerializeField] internal Sprite lv1;
        [SerializeField] internal Sprite lv3;
        [SerializeField] internal Sprite lv5;
        [SerializeField] internal Sprite lv7;
        [SerializeField] internal Sprite lv9;
        [SerializeField] internal Sprite lv11;
        [SerializeField] internal Sprite lv13;
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
        protected internal float UnitLevelDamage { get; protected set; }
        public Dictionary<EnemyBase, int> AttackCounts { get; set; } = new Dictionary<EnemyBase, int>();
        
        public virtual void Initialize()
        {
            unLock = true;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            UnitSkillDict = new Dictionary<int, Sprite> { {1, lv1}, {3, lv3}, {5, lv5}, {7, lv7}, {9, lv9}, {11, lv11}, {13, lv13} };
        }
        private int CheckForMaxPeace()
        {
            var maxPiece = 0;
            switch (unitPeaceLevel)
            {
                case 1:
                case 2:
                    maxPiece = 5;
                    break;
                case 3:
                    maxPiece = UnitGrade switch
                    {
                        UnitGrades.Green => 10,
                        UnitGrades.Blue => 5,
                    };
                    break;
                case 4:
                    maxPiece = UnitGrade switch
                    {
                        UnitGrades.Green => 25,
                        UnitGrades.Blue => 5,
                    };
                    break;
                 case 5:
                     maxPiece = UnitGrade switch
                     {
                       UnitGrades.Green => 50,
                       UnitGrades.Blue => 10,
                       UnitGrades.Purple => 3
                     };
                     break;
                 case 6:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 100,
                         UnitGrades.Blue => 25,
                         UnitGrades.Purple => 5
                     };
                     break;
                 case 7:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 200,
                         UnitGrades.Blue => 50,
                         UnitGrades.Purple => 10
                     };
                     break;
                 case 8:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 500,
                         UnitGrades.Blue => 100,
                         UnitGrades.Purple => 20
                     };
                     break;
                 case 9:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 1000,
                         UnitGrades.Blue => 200,
                         UnitGrades.Purple => 40
                     };
                     break;
                 case 10:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 2000,
                         UnitGrades.Blue => 400,
                         UnitGrades.Purple => 80
                     };
                     break;
                 case 11:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 4000,
                         UnitGrades.Blue => 800,
                         UnitGrades.Purple => 160
                     };
                     break;
                 case 12:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 6000,
                         UnitGrades.Blue => 1600,
                         UnitGrades.Purple => 320
                     };
                     break;
                 case 13:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 8000,
                         UnitGrades.Blue => 3200,
                         UnitGrades.Purple => 480
                     };
                     break;
                 case 14:
                     maxPiece = UnitGrade switch
                     {
                         UnitGrades.Green => 8000,
                         UnitGrades.Blue => 3200,
                         UnitGrades.Purple => 480
                     };
                     break;
            }
            return maxPiece;
        }
        private int CheckForLevelUpCoin()
        {
            var coin = 0;
            switch (unitPeaceLevel)
            {
                case 1:
                    coin = 0;
                    break;
                case 2:
                    coin = 100;
                    break;
                case 3:
                    coin = UnitGrade switch
                    {
                        UnitGrades.Green => 250,
                        UnitGrades.Blue => 0
                    };
                    break;
                case 4:
                    coin = UnitGrade switch
                    {
                        UnitGrades.Green => 500,
                        UnitGrades.Blue => 500
                    };
                    break;
                 case 5:
                     coin = UnitGrade switch
                     {
                       UnitGrades.Green => 1000,
                       UnitGrades.Blue => 1000,
                       UnitGrades.Purple => 0
                     };
                     break;
                 case 6:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 2500,
                         UnitGrades.Blue => 1500,
                         UnitGrades.Purple => 2500
                     };
                     break;
                 case 7:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 5000,
                         UnitGrades.Blue => 3000,
                         UnitGrades.Purple => 7500
                     };
                     break;
                 case 8:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 7500,
                         UnitGrades.Blue => 6000,
                         UnitGrades.Purple => 13000
                     };
                     break;
                 case 9:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 10000,
                         UnitGrades.Blue => 12000,
                         UnitGrades.Purple => 26000
                     };
                     break;
                 case 10:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 12500,
                         UnitGrades.Blue => 24000,
                         UnitGrades.Purple => 40000
                     };
                     break;
                 case 11:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 15000,
                         UnitGrades.Blue => 36000,
                         UnitGrades.Purple => 20000
                     };
                     break;
                 case 12:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 15000,
                         UnitGrades.Blue => 48000,
                         UnitGrades.Purple => 30000
                     };
                     break;
                 case 13:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 15000,
                         UnitGrades.Blue => 54000,
                         UnitGrades.Purple => 40000
                     };
                     break;
                 case 14:
                     coin = UnitGrade switch
                     {
                         UnitGrades.Green => 15000,
                         UnitGrades.Blue => 66000,
                         UnitGrades.Purple => 50000
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
            LevelUp();
            Tween scaleDown = sequence.Append(levelUpObject.transform.DOScale(_initialScale, 0.3f)); 
            scaleDown.WaitForCompletion();
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
        public IEnumerator UnitLevelUp()
        {
            CharacterPeaceCount -= CharacterMaxPeace;
            CoinsScript.Instance.Coin -= CharacterLevelUpCoin;
            unitPeaceLevel++;
            Initialize();
            yield return null;
        }
        protected internal virtual Sprite GetSprite(int level)
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
