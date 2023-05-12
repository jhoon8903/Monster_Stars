using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountManager : MonoBehaviour
{
    private int moveCount;
    public TextMeshProUGUI moveCountText;

    public void Initialize(int initialMoveCount)
    {
        moveCount = initialMoveCount;
        UpdateMoveCountText();
    }

    public bool CanMove()
    {
        return moveCount > 0;
    }

    public void DecreaseMoveCount()
    {
        if (moveCount > 0)
        {
            moveCount--;
            UpdateMoveCountText();
        }
    }

    private void UpdateMoveCountText()
    {
        moveCountText.text = $"Count: {moveCount}";
    }
}
