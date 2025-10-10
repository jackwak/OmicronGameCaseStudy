using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

[CreateAssetMenu(fileName = "New HexagonPattern", menuName = "Hexagon Pattern")]
public class HexagonPattern : ScriptableObject
{
    
    [Title(" Pattern Settings ")]
    public PatternSettings PatternSettings;

    [Title("Pattern Settings")]
    [InfoBox("Create multiple stack groups, each with their own stack positions, health ranges, and colors.")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetGroupLabel")]
    public List<StackGroupData> StackGroups = new List<StackGroupData>();
    
    [Title("Grid / Spacing")]
    [MinValue(0.01f)] public float HexRadius = 1f;
    public bool CanRandomize = false;

    public StackGroupData GetGroupById(int groupId)
    {
        return StackGroups.FirstOrDefault(g => g.GroupId == groupId);
    }

    #region Editor
#if UNITY_EDITOR
    [Title("Grid Settings")]
    [MinValue(1)] public int GridWidth = 9;
    [MinValue(1)] public int GridHeight = 9;

    [Title("Group Selection")]
    [InfoBox("Select which group to paint with. Left click to paint, Right click to erase.")]
    [ValueDropdown(nameof(GetGroupDropdown))]
    [OnValueChanged(nameof(OnSelectedGroupChanged))]
    [ShowInInspector, PropertyOrder(98)]
    private int? SelectedGroupId
    {
        get => _selectedGroupId;
        set
        {
            _selectedGroupId = value;
            OnSelectedGroupChanged();
        }
    }

    [ShowIf(nameof(HasSelectedGroup))]
    [ShowInInspector, PropertyOrder(99), ReadOnly]
    [LabelText("Current Group Info")]
    private string CurrentGroupInfo
    {
        get
        {
            var group = GetSelectedGroup();
            if (group == null) return "No group selected";
            return $"ID: {group.GroupId} | Type: {group.GroupType} | HP: {group.HealthRange.x}-{group.HealthRange.y} | Color: {group.GroupColor} | Stacks: {group.StackPositions.Count}";
        }
    }

    [Title("Pattern Grid - All Groups")]
    [InfoBox("Shows all groups on the same grid. Each group has its own color. Paint positions for the selected group.")]
    [ShowInInspector, PropertyOrder(100)]
    [TableMatrix(
        DrawElementMethod = nameof(DrawCell),
        SquareCells = true,
        HideRowIndices = false,
        HideColumnIndices = false,
        ResizableColumns = false)]
    private CellData[,] SelectionGrid
    {
        get => BuildGrid();
        set => ApplyGrid(value);
    }

    [Button("Clear All Positions", ButtonSizes.Medium), PropertyOrder(101)]
    private void ClearAllPositions()
    {
        foreach (var group in StackGroups)
        {
            group.StackPositions.Clear();
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
    
    [ShowIf(nameof(HasSelectedGroup))]
    [Button("Clear Selected Group Positions", ButtonSizes.Medium), PropertyOrder(101)]
    private void ClearSelectedGroupPositions()
    {
        var group = GetSelectedGroup();
        if (group != null)
        {
            group.StackPositions.Clear();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    private static int? _selectedGroupId = null;

    private bool HasSelectedGroup() => _selectedGroupId.HasValue;

    private StackGroupData GetSelectedGroup()
    {
        if (!_selectedGroupId.HasValue) return null;
        return StackGroups.FirstOrDefault(g => g.GroupId == _selectedGroupId.Value);
    }

    private IEnumerable<ValueDropdownItem<int?>> GetGroupDropdown()
    {
        var items = new List<ValueDropdownItem<int?>>();
        
        if (StackGroups.Count == 0)
        {
            items.Add(new ValueDropdownItem<int?>("No groups created", null));
        }
        else
        {
            foreach (var group in StackGroups)
            {
                string label = $"Group {group.GroupId} ({group.GroupType}) - HP:{group.HealthRange.x}-{group.HealthRange.y} - {group.StackPositions.Count} stacks";
                items.Add(new ValueDropdownItem<int?>(label, group.GroupId));
            }
        }
        
        return items;
    }

    private void OnSelectedGroupChanged()
    {
        UnityEditor.EditorUtility.SetDirty(this);
    }

    private CellData[,] BuildGrid()
    {
        int w = Mathf.Max(1, GridWidth);
        int h = Mathf.Max(1, GridHeight);
        var grid = new CellData[w, h];

        // Initialize all cells
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                grid[x, y] = new CellData();

        // Fill with all groups' positions
        foreach (var group in StackGroups)
        {
            foreach (var pos in group.StackPositions)
            {
                if (pos.x >= 0 && pos.x < w && pos.y >= 0 && pos.y < h)
                {
                    grid[pos.x, pos.y].GroupId = group.GroupId;
                    grid[pos.x, pos.y].GroupColor = group.GroupColor;
                }
            }
        }
        
        return grid;
    }

    private void ApplyGrid(CellData[,] grid)
    {
        // This is called when the grid is modified
        // We need to rebuild all groups' positions based on the grid
        
        // Clear all positions first
        foreach (var group in StackGroups)
        {
            group.StackPositions.Clear();
        }

        int w = grid.GetLength(0);
        int h = grid.GetLength(1);

        // Rebuild positions from grid
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid[x, y].GroupId.HasValue)
                {
                    var group = StackGroups.FirstOrDefault(g => g.GroupId == grid[x, y].GroupId.Value);
                    if (group != null)
                    {
                        group.StackPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }

    // Drag paint state
    private static bool _isPainting;
    private static bool _paintValue;
    private static HexagonPattern _currentInstance;

    private CellData DrawCell(Rect rect, CellData value)
    {
        _currentInstance = this;
        return DrawCellStatic(rect, value);
    }

    private static CellData DrawCellStatic(Rect rect, CellData value)
    {
        if (value == null) value = new CellData();

        // Arka plan rengi - eğer bir gruba ait ise o grubun rengi
        Color bgColor;
        if (value.GroupId.HasValue)
        {
            bgColor = value.GroupColor;
            bgColor.a = 0.8f;
        }
        else
        {
            bgColor = new Color(0f, 0f, 0f, 0.08f);
        }

        UnityEditor.EditorGUI.DrawRect(rect, bgColor);

        // Grup ID'sini göster
        if (value.GroupId.HasValue)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 10;
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            
            // Koyu arka plan için beyaz, açık için siyah yazı
            float brightness = value.GroupColor.r * 0.299f + value.GroupColor.g * 0.587f + value.GroupColor.b * 0.114f;
            style.normal.textColor = brightness > 0.5f ? Color.black : Color.white;
            
            GUI.Label(rect, value.GroupId.Value.ToString(), style);
        }

        // Kenarlık
        UnityEditor.Handles.color = new Color(0, 0, 0, 0.25f);
        UnityEditor.Handles.DrawAAPolyLine(2f,
            new Vector3(rect.xMin, rect.yMin),
            new Vector3(rect.xMax, rect.yMin),
            new Vector3(rect.xMax, rect.yMax),
            new Vector3(rect.xMin, rect.yMax),
            new Vector3(rect.xMin, rect.yMin));

        var e = Event.current;

        if (e.type == EventType.MouseUp)
        {
            _isPainting = false;
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            if (e.button == 0) // LMB = paint with selected group
            {
                if (_currentInstance != null && _selectedGroupId.HasValue)
                {
                    _isPainting = true;
                    _paintValue = true;
                    value.GroupId = _selectedGroupId.Value;
                    
                    // Get the color from the selected group
                    var selectedGroup = _currentInstance.StackGroups.FirstOrDefault(g => g.GroupId == _selectedGroupId.Value);
                    if (selectedGroup != null)
                    {
                        value.GroupColor = selectedGroup.GroupColor;
                    }
                    
                    GUI.changed = true;
                    e.Use();
                }
            }
            else if (e.button == 1) // RMB = erase
            {
                _isPainting = true;
                _paintValue = false;
                value.GroupId = null;
                value.GroupColor = Color.clear;
                GUI.changed = true;
                e.Use();
            }
        }
        else if (e.type == EventType.MouseDrag && _isPainting && rect.Contains(e.mousePosition))
        {
            if (_paintValue && _currentInstance != null && _selectedGroupId.HasValue)
            {
                value.GroupId = _selectedGroupId.Value;
                
                var selectedGroup = _currentInstance.StackGroups.FirstOrDefault(g => g.GroupId == _selectedGroupId.Value);
                if (selectedGroup != null)
                {
                    value.GroupColor = selectedGroup.GroupColor;
                }
                
                GUI.changed = true;
            }
            else if (!_paintValue)
            {
                value.GroupId = null;
                value.GroupColor = Color.clear;
                GUI.changed = true;
            }
            e.Use();
        }

        return value;
    }

    public class CellData
    {
        public int? GroupId;
        public Color GroupColor;
    }
#endif
    #endregion
}

public enum StackGroupType
{
    Static,
    Rotating,
    Oscillating
}

[System.Serializable]
public class StackGroupData
{
    [HorizontalGroup("Header", Width = 0.7f)]
    [HideLabel]
    [Title("Group Info")]
    public int GroupId;
    
    [HorizontalGroup("Header")]
    [HideLabel, EnumToggleButtons]
    public StackGroupType GroupType;
    
    [Title("Health & Visual Settings")]
    [MinMaxSlider(0, 300, true)]
    [InfoBox("Health range for hexagons spawned in this group.")]
    public Vector2Int HealthRange = new Vector2Int(0, 0);
    
    [MinValue(1)]
    [InfoBox("Reference health value to calculate hexagon count on stacks.")]
    public int HexagonHealthReference = 1;
    
    [ColorPalette]
    [InfoBox("Color for this group's hexagons.")]
    public Color GroupColor = Color.white;
    
    [Title("Stack Positions")]
    [InfoBox("Use the grid below to select stack positions for this group.")]
    [ReadOnly]
    public List<Vector2Int> StackPositions = new List<Vector2Int>();
    
    // Helper methods
    public int GetHexagonOnStackCount(int stackHealth)
    {
        if (HexagonHealthReference <= 0)
            return 0;

        float ratio = (float)stackHealth / HexagonHealthReference;
        int count = Mathf.CeilToInt(ratio);

        return Mathf.Clamp(count, 1, 10);
    }
    
    public int GetRandomHealth()
    {
        return Random.Range(HealthRange.x, HealthRange.y + 1);
    }
    
    // Rotating settings
    [ShowIf("@GroupType == StackGroupType.Rotating")]
    [FoldoutGroup("Rotation Settings")]
    [Range(-360f, 360f)]
    public float RotationSpeed = 45f;
    
    [ShowIf("@GroupType == StackGroupType.Rotating")]
    [FoldoutGroup("Rotation Settings")]
    public Vector3 RotationAxis = Vector3.up;

    [ShowIf("@GroupType == StackGroupType.Rotating")]
    [FoldoutGroup("Rotation Settings")]
    public PivotPosition PivotPosition = PivotPosition.Center;
    
    // Oscillating settings
    [ShowIf("@GroupType == StackGroupType.Oscillating")]
    [FoldoutGroup("Oscillation Settings")]
    [Range(0.1f, 10f)]
    public float Amplitude = 2f;
    
    [ShowIf("@GroupType == StackGroupType.Oscillating")]
    [FoldoutGroup("Oscillation Settings")]
    [Range(0.1f, 5f)]
    public float Frequency = 1f;
    
    [ShowIf("@GroupType == StackGroupType.Oscillating")]
    [FoldoutGroup("Oscillation Settings")]
    public Vector3 OscillationDirection = Vector3.right;

    public string GetGroupLabel()
    {
        return $"Group {GroupId} - {GroupType} | HP:{HealthRange.x}-{HealthRange.y} | {StackPositions.Count} pos";
    }
}