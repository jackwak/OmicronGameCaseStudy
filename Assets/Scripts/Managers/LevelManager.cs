using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private LevelData[] _levelDatas;

    [Header(" Variables ")]
    private LevelData _currentLevelData;
    private int _patternIndex = 0;

    public void InitializeLevel(int level)
    {
        _currentLevelData = _levelDatas[level - 1];
        _patternIndex = 0;
    }

    public HexagonPattern GetNextPattern()
    {
         return _currentLevelData.HexagonPatterns[_patternIndex++];
    }
}
