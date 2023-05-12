using UnityEngine;
using TMPro;

public class CountManager : MonoBehaviour
{
    private int _moveCount;
    public TextMeshProUGUI _moveCountText;

    public void Initialize(int initialMoveCount)
    {
        _moveCount = initialMoveCount;
        UpdateMoveCountText();
    }

    public bool CanMove()
    {
        return _moveCount > 0;
    }

    public void DecreaseMoveCount()
    {
        if (_moveCount > 0)
        {
            _moveCount--;
            UpdateMoveCountText();
        }
    }

    private void UpdateMoveCountText()
    {
        _moveCountText.text = $"Count: {_moveCount}";
    }
}
