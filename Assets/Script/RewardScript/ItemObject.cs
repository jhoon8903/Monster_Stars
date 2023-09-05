using System.Collections.Generic;
using Script.CharacterManagerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.RewardScript
{
    public class ItemObject : MonoBehaviour
    {
        
        [SerializeField] private Image gradeBack;
        [SerializeField] private Sprite greenBack;
        [SerializeField] private Sprite blueBack;
        [SerializeField] private Sprite purpleBack;
        [SerializeField] private Sprite greyBack;
        [SerializeField] private Image item;
        [SerializeField] private Sprite coin;
        [SerializeField] private TextMeshProUGUI itemValue;
        private ItemObject _itemInstantiate;

        public void CoinObject(Transform rewardGrid)
        {
            if (_itemInstantiate != null)
            {
                Destroy(_itemInstantiate);
            }
            _itemInstantiate = Instantiate(this, rewardGrid);
            _itemInstantiate.itemValue.text = $"{ClearRewardManager.Instance.cumulativeCoin}";
            _itemInstantiate.gradeBack.sprite = greyBack;
            _itemInstantiate.item.sprite = coin;
        }

        private static Dictionary<CharacterBase, int> GetCumulativeUnitPieces()
        {
            return ClearRewardManager.Instance.CumulativeUnitPieces;
        }
        
        public void InstantiateCumulativeUnitPieces(Transform rewardGrid)
        {
            var unitPieces = GetCumulativeUnitPieces();
            foreach (var (unit, pieceCount) in unitPieces)
            {
                if (pieceCount <= 0) continue;
                var instantiatedItem = Instantiate(this, rewardGrid);
                instantiatedItem.itemValue.text = $"{pieceCount}";
                instantiatedItem.item.sprite = unit.GetSpriteForLevel(unit.unitPieceLevel);
                switch (unit.UnitGrade)
                {
                    case CharacterBase.UnitGrades.G:
                        instantiatedItem.gradeBack.sprite = greenBack;
                        break;
                    case CharacterBase.UnitGrades.B:
                        instantiatedItem.gradeBack.sprite = blueBack;
                        break;
                    case CharacterBase.UnitGrades.P:
                        instantiatedItem.gradeBack.sprite = purpleBack;
                        break;
                }
            }
        } 
    }
}
