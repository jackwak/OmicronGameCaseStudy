using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [Title("Level Info")]
    [ReadOnly]
    public int LevelNumber;
    
    [Title("Patterns")]
    [InfoBox("Patterns will be spawned in order. Each pattern spawns when the player reaches a trigger.")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "PatternName")]
    public List<HexagonPattern> Patterns = new List<HexagonPattern>();
    
    public int GetPatternCount()
    {
        return Patterns != null ? Patterns.Count : 0;
    }
    
    public HexagonPattern GetPattern(int index)
    {
        if (Patterns == null || index < 0 || index >= Patterns.Count)
            return null;
        return Patterns[index];
    }
    
    private string PatternName(HexagonPattern pattern, int index)
    {
        if (pattern == null)
            return $"Pattern {index}: [Empty]";
        return $"Pattern {index}: {pattern.name} ({pattern.StackGroups.Count} groups)";
    }
}