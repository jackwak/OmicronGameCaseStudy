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
    public int CurrentLevel = 1; // 1-based (Level 1, 2, 3...)
    
    [ReadOnly, ShowInInspector]
    private int _currentPatternIndex = 0; // 0-based (array index)
    
    [ReadOnly, ShowInInspector]
    private LevelData _currentLevelData;
    
    [Title("Spawn Settings")]
    public Transform PatternSpawnParent; // Parent object for spawned patterns
    public GameObject HexagonStackPrefab; // HexagonStack prefab to spawn
    
    [Title("References")]
    public PatternSpawnTrigger SpawnTrigger;

    [Title("Pooling")]
    public string HexagonStackPoolKey = "HexagonStack";
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject); // Optional: keep between scenes

        // Initialize for current level
        LoadLevel(CurrentLevel);
    }
    
    private void Start()
    {
        // Optional: Spawn first pattern immediately
        // SpawnNextPattern();
    }
    
    public void LoadLevel(int levelNumber)
    {
        CurrentLevel = levelNumber;
        _currentPatternIndex = 0;
        
        // Get level data (levelNumber is 1-based, array is 0-based)
        int dataIndex = levelNumber - 1;
        if (dataIndex >= 0 && dataIndex < LevelDataList.Count)
        {
            _currentLevelData = LevelDataList[dataIndex];
            Debug.Log($"Loaded Level {levelNumber} with {_currentLevelData.GetPatternCount()} patterns");
        }
        else
        {
            Debug.LogError($"Level {levelNumber} data not found!");
            _currentLevelData = null;
        }
    }
    
    public void SpawnNextPattern(Vector3 spawnPosition)
    {
        Debug.Log("=== SpawnNextPattern called with position ===");
        
        if (_currentLevelData == null)
        {
            Debug.LogError("No level data loaded!");
            return;
        }
        
        Debug.Log($"Current pattern index: {_currentPatternIndex}, Total patterns: {_currentLevelData.GetPatternCount()}");
        
        HexagonPattern pattern = _currentLevelData.GetPattern(_currentPatternIndex);
        
        if (pattern == null)
        {
            Debug.LogWarning($"Pattern {_currentPatternIndex} is null or level completed!");
            return;
        }
        
        Debug.Log($"Spawning Pattern {_currentPatternIndex}: {pattern.name} with {pattern.StackGroups.Count} groups at position {spawnPosition}");
        
        // Spawn the pattern at the given position
        SpawnPattern(pattern, spawnPosition);
        
        // Move to next pattern
        _currentPatternIndex++;
        
        // Check if level is completed
        if (_currentPatternIndex >= _currentLevelData.GetPatternCount())
        {
            OnLevelCompleted();
        }
    }
    
    public void SpawnNextPattern()
    {
        // Find PatternSpawnPosition in scene for test button
        PatternSpawnPosition spawnPos = FindObjectOfType<PatternSpawnPosition>();
        Vector3 spawnPosition = spawnPos != null ? spawnPos.transform.position : Vector3.zero;
        
        // Call the main method with position
        SpawnNextPattern(spawnPosition);
    }
    
    private void SpawnPattern(HexagonPattern pattern, Vector3 spawnPosition)
    {
        GameObject patternParent = new GameObject($"Pattern_{_currentPatternIndex}_{pattern.name}");
        
        if (PatternSpawnParent != null)
            patternParent.transform.SetParent(PatternSpawnParent);
        
        patternParent.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        
        foreach (var groupData in pattern.StackGroups)
        {
            SpawnStackGroup(groupData, pattern, patternParent.transform);
        }
    }
    
    private void SpawnStackGroup(StackGroupData groupData, HexagonPattern pattern, Transform patternParent)
    {
        GameObject groupObject = new GameObject($"Group_{groupData.GroupId}_{groupData.GroupType}");
        groupObject.transform.SetParent(patternParent);
        groupObject.transform.localPosition = Vector3.zero;
        
        HexagonStackGroup groupComponent = null;
        switch (groupData.GroupType)
        {
            case StackGroupType.Static:
                groupComponent = groupObject.AddComponent<StaticStackGroup>();
                break;
            case StackGroupType.Rotating:
                var rotatingGroup = groupObject.AddComponent<RotatingStackGroup>();
                rotatingGroup.RotationSpeed = groupData.RotationSpeed;
                rotatingGroup.RotationAxis = groupData.RotationAxis;
                rotatingGroup.PivotPosition = groupData.PivotPosition;
                groupComponent = rotatingGroup;
                break;
            case StackGroupType.Oscillating:
                var oscillatingGroup = groupObject.AddComponent<OscillatingStackGroup>();
                oscillatingGroup.Amplitude = groupData.Amplitude;
                oscillatingGroup.Frequency = groupData.Frequency;
                oscillatingGroup.OscillationDirection = groupData.OscillationDirection;
                groupComponent = oscillatingGroup;
                break;
        }
        
        if (groupComponent != null)
        {
            groupComponent.IsActive = true;
        }
        
        List<GameObject> tempStacks = new List<GameObject>();
        
        foreach (var gridPosition in groupData.StackPositions)
        {
            GameObject stack = SpawnStackAtPosition(gridPosition, groupData, pattern, groupObject.transform, groupComponent);
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

        // Çocukların LOKAL konumlarından AABB (min-max) çıkar
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

        // Hedef pivot noktası (grup lokal uzayında)
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

        // Offset: mevcut pivot (0,0,0) -> hedef pivot (target)
        Vector3 offsetLocal = target;

        // 1) Tüm çocukları lokal uzayda -offset kadar kaydır
        foreach (var go in childStacks)
        {
            if (go == null) continue;
            go.transform.localPosition -= offsetLocal;
        }

        // 2) Grubu dünya uzayında +offset kadar taşı (görsel sabit kalsın)
        Vector3 offsetWorld = groupTransform.TransformVector(offsetLocal);
        groupTransform.position += offsetWorld;

        Debug.Log($"AdjustGroupPivot: {pivotPosition} için offset (local): {offsetLocal}, (world): {offsetWorld}");
    }

    private GameObject SpawnStackAtPosition(Vector2Int gridPosition,
    StackGroupData groupData,
    HexagonPattern pattern,
    Transform groupParent,
    HexagonStackGroup groupComponent)
    {
        var stackObject = ObjectPool.Instance.Get(HexagonStackPoolKey, parent: groupParent);
        if (stackObject == null)
        {
            Debug.LogError($"Pooldan obje alınamadı. Key: {HexagonStackPoolKey}");
            return null;
        }

        stackObject.name = $"Stack_{gridPosition.x}_{gridPosition.y}";

        // Lokal konum ver
        Vector3 localPosition = GridToWorldPosition(gridPosition, pattern.HexRadius);
        stackObject.transform.localPosition = localPosition;

        // Bileşen kontrolü
        var stackComponent = stackObject.GetComponent<HexagonStack>();
        if (stackComponent == null)
        {
            var po = stackObject.GetComponent<PooledObject>();
            if (po != null) po.ReturnToPool(); else Destroy(stackObject);
            return null;
        }

        // Initialize
        float health = groupData.GetRandomHealth();
        int hexagonCount = groupData.GetHexagonOnStackCount((int)health);
        stackComponent.InitializeHexagonStack(health, hexagonCount, groupData.GroupColor);

        if (groupComponent != null)
            groupComponent.RegisterChildStack(stackObject);

        return stackObject;
    }
    
    private Vector3 GridToWorldPosition(Vector2Int gridPos, float hexRadius)
    {
        
        float x = gridPos.x * hexRadius;
        float y = gridPos.y * hexRadius;
        
        return new Vector3(x, y, 0); // 2D: X-Y plane, Z=0
    }
    
    private void OnLevelCompleted()
    {
        Debug.Log($"Level {CurrentLevel} completed!");
        // TODO: Handle level completion
    }
    
    [Button("Spawn Next Pattern (Test)", ButtonSizes.Large)]
    private void TestSpawnNextPattern()
    {
        SpawnNextPattern(Vector3.zero);
    }

    [Button("Reset Level", ButtonSizes.Medium)]
    private void ResetLevel()
    {
        LoadLevel(CurrentLevel);
        Debug.Log("Level reset!");
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}