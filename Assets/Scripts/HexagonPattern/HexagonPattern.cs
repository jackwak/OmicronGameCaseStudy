using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New HexagonPattern", menuName = "Hexagon Pattern")]
public class HexagonPattern : ScriptableObject
{
    [Title("Pattern Settings")]
    [MinMaxSlider(0, 300, true)]
    public Vector2Int HealthRange = new Vector2Int(0, 0);

    [InfoBox("The health values within this range can be used during spawning.")]
    [InlineButton(nameof(ClearPositions), "Clear")]
    public List<Vector2Int> StackPositions = new List<Vector2Int>();

    public Color Color = Color.white;
    [Title("Grid / Spacing")]
    [MinValue(0.01f)] public float HexRadius = 1f;
    public bool CanRandomize = false;
    public int HexagonHealthReference = 1;

    public int GetHexagonOnStackCount(int stackHealth)
    {
        if (HexagonHealthReference <= 0)
            return 0;

        float ratio = (float)stackHealth / HexagonHealthReference;
        int count = Mathf.CeilToInt(ratio);

        return Mathf.Clamp(count, 1, 10);
    }

    #region Editor

    [Title("Grid")]
    [MinValue(1)] public int GridWidth = 9;
    [MinValue(1)] public int GridHeight = 9;

    [Title("Select Stack Positions")]
    [ShowInInspector, PropertyOrder(100)]
    [TableMatrix(
        DrawElementMethod = nameof(DrawCell),
        SquareCells = true,
        HideRowIndices = false,
        HideColumnIndices = false,
        ResizableColumns = false)]
    private bool[,] SelectionGrid
    {
        get => BuildGrid();
        set => ApplyGrid(value);
    }

    private void ClearPositions()
    {
        StackPositions.Clear();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private bool[,] BuildGrid()
    {
        int w = Mathf.Max(1, GridWidth);
        int h = Mathf.Max(1, GridHeight);
        var grid = new bool[w, h];

        foreach (var v in StackPositions)
        {
            if (v.x >= 0 && v.x < w && v.y >= 0 && v.y < h)
                grid[v.x, v.y] = true;
        }
        return grid;
    }

    private void ApplyGrid(bool[,] grid)
    {
        StackPositions.Clear();
        int w = grid.GetLength(0);
        int h = grid.GetLength(1);

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                if (grid[x, y])
                    StackPositions.Add(new Vector2Int(x, y));

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

#if UNITY_EDITOR
    // --- Drag paint state (editor only) ---
    private static bool _isPainting;
    private static bool _paintValue; // true = set, false = clear

    // Tek tık / sürükle boya (LMB), sağ tık sil (RMB)
    private static bool DrawCell(Rect rect, bool value)
    {
        // arka plan
        UnityEditor.EditorGUI.DrawRect(rect, value ? new Color(0.3f, 0.8f, 0.4f, 0.9f)
                                                   : new Color(0f, 0f, 0f, 0.08f));

        // kenarlık
        UnityEditor.Handles.color = new Color(0, 0, 0, 0.25f);
        UnityEditor.Handles.DrawAAPolyLine(2f,
            new Vector3(rect.xMin, rect.yMin),
            new Vector3(rect.xMax, rect.yMin),
            new Vector3(rect.xMax, rect.yMax),
            new Vector3(rect.xMin, rect.yMax),
            new Vector3(rect.xMin, rect.yMin));

        var e = Event.current;

        // MouseUp her yerde boyamayı bitirir
        if (e.type == EventType.MouseUp)
        {
            _isPainting = false;
        }

        // MouseDown: hücrenin içinde başladıysa mod ve hedef değeri belirle
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            if (e.button == 0)       // LMB = boya (true)
            {
                _isPainting = true;
                _paintValue = true;
                value = true;
                GUI.changed = true;
                e.Use();
            }
            else if (e.button == 1)  // RMB = sil (false)
            {
                _isPainting = true;
                _paintValue = false;
                value = false;
                GUI.changed = true;
                e.Use();
            }
        }
        // MouseDrag: boyama açıksa, üzerinden geçtiğimiz hücrenin değerini sürekli uygula
        else if (e.type == EventType.MouseDrag && _isPainting && rect.Contains(e.mousePosition))
        {
            if (value != _paintValue)
            {
                value = _paintValue;
                GUI.changed = true;
            }
            e.Use();
        }
        // Tek tık (mouse move yoksa) için güvence: MouseDown anında zaten yukarıda set ediyoruz.

        return value;
    }
#endif
#endregion
}
