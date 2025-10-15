using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Title("Level Configuration")]
    [InfoBox("Assign level data for each level. Index 0 = Level 1, Index 1 = Level 2, etc.")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<LevelData> LevelDataList = new List<LevelData>();
    
    [Title("Current Level")]
    [ReadOnly, ShowInInspector]
    private int _currentLevel = 1;
    
    [ReadOnly, ShowInInspector]
    private int _currentPatternIndex = 0;
    
    [ReadOnly, ShowInInspector]
    private LevelData _currentLevelData;

    [Title("Pooling Keys")]
    private const string HEXAGON_STACK_POOL_KEY = "OctagonStack";
    private const string ROTATION_STACK_GROUP_KEY = "RotationStackGroup";
    private const string OSCILLATING_STACK_GROUP_KEY = "OscillatingStackGroup";
    private const string STATIC_STACK_GROUP_KEY = "StaticStackGroup";

    public int GetLevelNumber => _currentLevel;

    private void Awake()
    {
        Instance = this;
        _currentLevel = PlayerPrefs.GetInt("Level", 1);
    }

    void OnEnable()
    {
        EventManager.Instance.EnterReadyState += LoadCurrentLevel;
        EventManager.Instance.GameWin += IncreaseCurrentLevelNumber;
    }

    void OnDisable()
    {
        EventManager.Instance.EnterReadyState -= LoadCurrentLevel;
        EventManager.Instance.GameWin -= IncreaseCurrentLevelNumber;
    }

    public void LoadCurrentLevel()
    {
        LoadLevel(_currentLevel);
    }

    public void IncreaseCurrentLevelNumber(){ _currentLevel++; }

    public void LoadLevel(int levelNumber)
    {
        _currentLevel = levelNumber;
        _currentPatternIndex = 0;

        int dataIndex = levelNumber - 1;
        if (dataIndex >= 0 && dataIndex < LevelDataList.Count)
        {
            _currentLevelData = LevelDataList[dataIndex];
        }
        else
        {
            int randomLevelIndex = Random.Range(0, LevelDataList.Count);
            _currentLevelData = LevelDataList[randomLevelIndex];
        }

        EventManager.Instance.OnInitializeWeaponData(_currentLevelData.WeaponData);
    }
    
    public int GetPatternCount()
    { 
        return _currentLevelData.GetPatternCount(); 
    }
    
    public void SpawnNextPattern(Transform spawnTransform)
    {
        if (_currentLevelData == null)
        {
            return;
        }

        HexagonPattern pattern = _currentLevelData.GetPattern(_currentPatternIndex);
        if (pattern == null)
        {
            SpawnFinishLine(spawnTransform);
            return;
        }

        SpawnPattern(pattern, spawnTransform);

        _currentPatternIndex++;

        if (_currentPatternIndex >= _currentLevelData.GetPatternCount())
        {
            OnLevelCompleted();
        }
    }

    private void SpawnPattern(HexagonPattern pattern, Transform spawnTransform)
    {
        float xOffset = CalculateScreenCenterXOffset(pattern);
        
        foreach (var groupData in pattern.StackGroups)
        {
            SpawnStackGroup(groupData, pattern, spawnTransform, xOffset);
        }
    }

    private float CalculateScreenCenterXOffset(HexagonPattern pattern)
    {
        if (pattern.StackGroups == null || pattern.StackGroups.Count == 0)
            return 0f;

        int minX = int.MaxValue;
        int maxX = int.MinValue;

        foreach (var group in pattern.StackGroups)
        {
            foreach (var pos in group.StackPositions)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
            }
        }

        Camera mainCamera = Camera.main;
        float screenWidth = mainCamera.orthographicSize * 2f * mainCamera.aspect;
        float gridWidth = (maxX - minX) * pattern.PatternSettings.HexRadius;
        float offset = (screenWidth - gridWidth) / 2f;
        offset -= minX * pattern.PatternSettings.HexRadius;

        return offset;
    }

    private void SpawnFinishLine(Transform spawnTransform)
    {
        GameObject groupObject = ObjectPool.Instance.Get("FinishLine");
        groupObject.transform.SetParent(spawnTransform);
        groupObject.transform.localPosition = new Vector3(0, 0, 0);
    }
    
    private void SpawnStackGroup(StackGroupData groupData, HexagonPattern pattern, Transform spawnTransform, float xOffset)
    {
        GameObject groupObject = null;
        HexagonStackGroup groupComponent = null;

        switch (groupData.GroupType)
        {
            case StackGroupType.Static:
                groupObject = ObjectPool.Instance.Get(STATIC_STACK_GROUP_KEY);
                groupComponent = groupObject.GetComponent<StaticStackGroup>();
                break;
            case StackGroupType.Rotating:
                groupObject = ObjectPool.Instance.Get(ROTATION_STACK_GROUP_KEY);
                var rotatingGroup = groupObject.GetComponent<RotatingStackGroup>();
                rotatingGroup.RotationSpeed = groupData.RotationSpeed;
                rotatingGroup.RotationAxis = groupData.RotationAxis;
                rotatingGroup.PivotPosition = groupData.PivotPosition;
                groupComponent = rotatingGroup;
                break;
            case StackGroupType.Oscillating:
                groupObject = ObjectPool.Instance.Get(OSCILLATING_STACK_GROUP_KEY);
                var oscillatingGroup = groupObject.GetComponent<OscillatingStackGroup>();
                oscillatingGroup.Amplitude = groupData.Amplitude;
                oscillatingGroup.Frequency = groupData.Frequency;
                oscillatingGroup.OscillationDirection = groupData.OscillationDirection;
                groupComponent = oscillatingGroup;
                break;
        }

        groupObject.transform.SetParent(spawnTransform);
        groupObject.transform.localPosition = new Vector3(0, 0, -1);

        if (groupComponent != null)
        {
            groupComponent.IsActive = true;
        }

        List<GameObject> tempStacks = new List<GameObject>();

        foreach (var gridPosition in groupData.StackPositions)
        {
            GameObject stack = SpawnStackAtPosition(gridPosition, groupData, pattern, groupObject.transform, groupComponent, xOffset);
            if (stack != null)
                tempStacks.Add(stack);
        }

        if (groupData.GroupType == StackGroupType.Rotating && tempStacks.Count > 0)
        {
            AdjustGroupPivot(groupObject.transform, tempStacks, groupData.PivotPosition);
        }
    }

    private void AdjustGroupPivot(Transform groupTransform, List<GameObject> childStacks, PivotPosition pivotPosition)
    {
        if (groupTransform == null || childStacks == null || childStacks.Count == 0)
        {
            Debug.LogWarning("AdjustGroupPivot: Geçersiz grup veya çocuk listesi.");
            return;
        }

        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (var go in childStacks)
        {
            if (go == null) continue;
            Vector3 lp = go.transform.localPosition;
            if (lp.x < min.x) min.x = lp.x;
            if (lp.y < min.y) min.y = lp.y;
            if (lp.z < min.z) min.z = lp.z;

            if (lp.x > max.x) max.x = lp.x;
            if (lp.y > max.y) max.y = lp.y;
            if (lp.z > max.z) max.z = lp.z;
        }

        Vector3 center = (min + max) * 0.5f;

        Vector3 target = center;
        switch (pivotPosition)
        {
            case PivotPosition.Center:
                target = center;
                break;
            case PivotPosition.Left:
                target = new Vector3(min.x, center.y, center.z);
                break;
            case PivotPosition.Right:
                target = new Vector3(max.x, center.y, center.z);
                break;
            case PivotPosition.Top:
                target = new Vector3(center.x, max.y, center.z);
                break;
            case PivotPosition.Bottom:
                target = new Vector3(center.x, min.y, center.z);
                break;
            case PivotPosition.TopLeft:
                target = new Vector3(min.x, max.y, center.z);
                break;
            case PivotPosition.TopRight:
                target = new Vector3(max.x, max.y, center.z);
                break;
            case PivotPosition.BottomLeft:
                target = new Vector3(min.x, min.y, center.z);
                break;
            case PivotPosition.BottomRight:
                target = new Vector3(max.x, min.y, center.z);
                break;
        }

        Vector3 offsetLocal = target;

        foreach (var go in childStacks)
        {
            if (go == null) continue;
            go.transform.localPosition -= offsetLocal;
        }

        Vector3 offsetWorld = groupTransform.TransformVector(offsetLocal);
        groupTransform.position += offsetWorld;
    }

    private GameObject SpawnStackAtPosition(Vector2Int gridPosition,
        StackGroupData groupData,
        HexagonPattern pattern,
        Transform groupParent,
        HexagonStackGroup groupComponent,
        float xOffset)
    {
        var stackObject = ObjectPool.Instance.Get(HEXAGON_STACK_POOL_KEY, parent: groupParent);
        stackObject.name = $"Stack_{gridPosition.x}_{gridPosition.y}";

        Vector3 localPosition = GridToWorldPosition(gridPosition, pattern.PatternSettings.HexRadius);
        stackObject.transform.localPosition = localPosition + new Vector3(xOffset, 0, .1f * gridPosition.y);

        var stackComponent = stackObject.GetComponent<HexagonStack>();
        if (stackComponent == null)
        {
            var po = stackObject.GetComponent<PooledObject>();
            if (po != null) po.ReturnToPool(); else Destroy(stackObject);
            return null;
        }

        float health = groupData.GetRandomHealth();
        int hexagonCount = groupData.GetHexagonOnStackCount((int)health);
        stackComponent.InitializeHexagonStack(health, groupData.PerOctagonHealth, hexagonCount, groupData.GroupColor);

        if (groupComponent != null)
            groupComponent.RegisterChildStack(stackObject);

        return stackObject;
    }
    
    private Vector3 GridToWorldPosition(Vector2Int gridPos, float hexRadius)
    {
        float x = gridPos.x * hexRadius;
        float y = gridPos.y * hexRadius;
        
        return new Vector3(x, y, 0);
    }
    
    private void OnLevelCompleted()
    {
        Debug.Log($"Level {_currentLevel} completed!");
    }

    [Button("Reset Level", ButtonSizes.Medium)]
    private void ResetLevel()
    {
        LoadLevel(_currentLevel);
        Debug.Log("Level reset!");
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}