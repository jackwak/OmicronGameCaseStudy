using UnityEngine;

[CreateAssetMenu(fileName = "PatternSettings", menuName = "ScriptableObjects/Pattern Settings")]
public class PatternSettings : ScriptableObject
{
    [Min(0.01f)] public float HexRadius = .6f;

}
