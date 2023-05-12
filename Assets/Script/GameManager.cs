using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public CountManager countManager;
    public int moveCount;
    public SpawnManager spawnManager;

    private void Start()
    {
        countManager.Initialize(moveCount);
        spawnManager.SpawnCharacters();
        //StartCoroutine(gridManager.GenerateInitialGrid());
    }
}
