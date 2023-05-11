using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GridManager gridManager;
    public CharacterManager characterManager;
    public CharacterPool characterPool;

    public void FillEmptyGridPositions()
    {
        foreach (Transform rowTransform in gridManager.transform)
        {
            foreach (Transform gridTransform in rowTransform)
            {
                if (gridTransform.childCount == 0)
                {
                    SpawnCharacterInGridPosition(gridTransform);
                }
            }
        }
    }

    public void SpawnCharactersInRow(Transform rowTransform)
    {
        for (int i = 0; i < rowTransform.childCount; i++)
        {
            Transform gridTransform = rowTransform.GetChild(i);
            SpawnCharacterInGridPosition(gridTransform);
        }
    }

    private void SpawnCharacterInGridPosition(Transform gridTransform)
    {
        GameObject characterPrefab = characterManager.GetRandomCharacterPrefab();
        Vector3 newPosition = new Vector3(gridTransform.position.x, gridTransform.position.y, -0.5f);
        GameObject characterInstance = characterPool.GetPooledObject(characterPrefab);
        characterInstance.transform.position = newPosition;
        characterInstance.transform.SetParent(characterPool.transform, true);
        characterInstance.SetActive(true);

        // Disable all child objects except for the one named "level0"
        for (int j = 0; j < characterInstance.transform.childCount; j++)
        {
            Transform child = characterInstance.transform.GetChild(j);
            if (child.name != "level0")
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
