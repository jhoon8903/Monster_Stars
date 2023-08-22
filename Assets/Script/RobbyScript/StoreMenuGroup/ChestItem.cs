using Script.CharacterManagerScript;
using Script.RewardScript;
using Script.RobbyScript.TopMenuGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RobbyScript.StoreMenuGroup
{
    public class ChestItem : MonoBehaviour
    {
        public GameObject chestRewardItem;
        public TextMeshProUGUI chestRewardText;
        public Goods goodsObject;
        [SerializeField] private SpecialOffer specialOffer;

        public Goods CoinInstance(Goods rewardItem, Transform contentLayer, StoreMenu.BoxGrade boxTypes, int coinReward)
        {
            goodsObject = Instantiate(rewardItem, contentLayer);
            goodsObject.goodsBack.GetComponent<Image>().color = Color.white;
            goodsObject.goodsSprite.GetComponent<Image>().sprite = specialOffer.RewardSprite(boxTypes);
            goodsObject.goodsSprite.GetComponent<RectTransform>().localScale = boxTypes switch
            {
                StoreMenu.BoxGrade.Coin => new Vector3(1, 0.8f, 0),
                StoreMenu.BoxGrade.Stamina => new Vector3(0.8f, 0.9f, 0),
                StoreMenu.BoxGrade.Gem => new Vector3(1, 0.8f, 0),
                _ => new Vector3(0.8f, 0.8f, 0)
            };
            goodsObject.goodsValue.text = $"{coinReward}";
            goodsObject.goodsValue.GetComponent<RectTransform>().localScale = boxTypes switch
            {
                StoreMenu.BoxGrade.Coin =>  new Vector3(1, 1, 0),
                StoreMenu.BoxGrade.Stamina =>  new Vector3(1, 1, 0),
                StoreMenu.BoxGrade.Gem =>  new Vector3(1, 1, 0),
                _ =>   new Vector3(1, 1, 0)
            };
            CoinsScript.Instance.Coin += coinReward;
            CoinsScript.Instance.UpdateCoin();
            return goodsObject;
        }

        public Goods PieceInstance(CharacterBase characterBase, Goods rewardItem, int rewardValue, Transform contentsLayer)
        {
            goodsObject = Instantiate(rewardItem, contentsLayer);
            goodsObject.goodsSprite.GetComponent<Image>().sprite = characterBase.GetSpriteForLevel(characterBase.unitPeaceLevel);
            goodsObject.goodsSprite.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
            goodsObject.goodsValue.text = $"{rewardValue}";
            goodsObject.goodsValue.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
            return goodsObject;
        }
    }
}

