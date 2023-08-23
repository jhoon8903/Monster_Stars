using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UIManager
{
    public class InfoObject : MonoBehaviour
    {
        [SerializeField] public Image infoIcon;
        [SerializeField] public Sprite damageSprite;
        [SerializeField] public Sprite attackRangeSprite;
        [SerializeField] public Sprite attackSpeedSprite;
        // 0 : slow / 1 : Burn / 2 : Poison / 3 : Bleed
        [SerializeField] public List<Sprite> durationSprites;
        [SerializeField] public List<Sprite> intensitySprites;

        [SerializeField] public Sprite bounceSprite;
        [SerializeField] public TextMeshProUGUI infoTitle;
        [SerializeField] public TextMeshProUGUI infoDesc;


    }
}
