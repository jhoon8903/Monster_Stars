using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public CountManager countManager;
    public int initialMoveCount = 5;

    private void Start()
    {
        countManager.Initialize(initialMoveCount);
        StartCoroutine(gridManager.GenerateInitialGrid());
    }
}
