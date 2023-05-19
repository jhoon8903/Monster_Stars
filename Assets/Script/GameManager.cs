using System.Collections;
using Script.CharacterManagerScript;
using UnityEngine;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        public int moveCount;
        [SerializeField]
        private GridManager _gridManager;
        [SerializeField]
        private CountManager _countManager;
        [SerializeField]
        private SpawnManager _spawnManager;
        [SerializeField]
        private MatchManager _matchManager;
        [SerializeField] 
        private CharacterPool _characterPool;

        private bool spawnFinsh = false;
        private void Start()
        {
            _countManager.Initialize(moveCount);
            StartCoroutine(SpawnDoneCheck());
        }

        private IEnumerator SpawnDoneCheck()
        {
            yield return new WaitUntil(() => _spawnManager.SpawnCharacters());
            StartMatches();
        }
        
        private void StartMatches()
        {
            var allCharacterPosition = new Vector3();
            for (var x = 0; x < _gridManager.gridWidth; x++)
            {
                for (var y = 0; y < _gridManager.gridHeight; y++)
                {
                    var character = _spawnManager.GetCharacterAtPosition(allCharacterPosition);
                    _matchManager.IsMatched(character);
                }
            }
        }
    }
}