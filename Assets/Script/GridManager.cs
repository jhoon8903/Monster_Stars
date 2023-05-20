using System.Collections.Generic;
using System.Linq;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script
{
    public sealed class GridManager : MonoBehaviour
    {
        public int gridHeight = 6;
        public int gridWidth = 6;
        public GameObject grid1Sprite;
        public GameObject grid2Sprite;
        private int _currentRowType = 1;
        private const int MaxRows = 9;
        [SerializeField] private CharacterPool characterPool;
        [SerializeField] private SpawnManager spawnManager;

        /**
         * Create Grid Instance
         */
        public void GenerateInitialGrid()
        {
            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    var spritePrefab = (x + y) % 2 == 0 ? grid1Sprite : grid2Sprite;
                    Instantiate(spritePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                }
            }
        }
        
        /**
         * if Calling AddRow() => create New Row And Spawn New Character on Grid
         */
        public void AddRow()
        {
            if (gridHeight >= MaxRows) return;
            gridHeight++;
            
            foreach (var character in characterPool.UsePoolCharacterList())
            {
                var newPosition = character.transform.position;
                newPosition.y += 1;
                character.transform.position = newPosition;
            }
            
            foreach (Transform child in transform)
            {
                var newPosition = child.position;
                newPosition.y += 1;
                child.position = newPosition;
            }
            
            for (var x = 0; x < gridWidth; x++)
            {
                var spritePrefab = (x + _currentRowType) % 2 == 0 ? grid1Sprite : grid2Sprite;
                Instantiate(spritePrefab, new Vector3(x, 0, 0), Quaternion.identity, transform);
            }
            _currentRowType = _currentRowType == 1 ? 2 : 1;
            
            var notUseCharacters = characterPool.NotUsePoolCharacterList();
            for (var x = 0; x < gridWidth; x++)
            {
                var randomCharacterIndex = Random.Range(0, notUseCharacters.Count);
                var character = notUseCharacters[randomCharacterIndex];
                character.transform.position = new Vector3(x, 0, 0);
                character.SetActive(true);
                notUseCharacters.RemoveAt(randomCharacterIndex);
            }
        }
    }
}
