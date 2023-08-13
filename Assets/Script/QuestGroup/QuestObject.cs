using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.QuestGroup
{
    public class QuestObject : MonoBehaviour
    {
        [SerializeField] internal TextMeshProUGUI questDesc;
        [SerializeField] internal Slider questProgress;
        [SerializeField] internal TextMeshProUGUI questProgressText;
        [SerializeField] internal Image item1Sprite;
        [SerializeField] internal TextMeshProUGUI item1Value;
        [SerializeField] internal Image item2Sprite;
        [SerializeField] internal TextMeshProUGUI item2Value;
        [SerializeField] internal Button receiveBtn;
        [SerializeField] internal TextMeshProUGUI receiveBtnText;
        [SerializeField] internal Button shuffleBtn;
        public QuestManager.QuestTypes questType;
        public QuestManager.QuestCondition questCondition;
        public int questValue;
        public int questGoal;
    }
}
