using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 0)]
public class LevelData : ScriptableObject 
{
    public HexagonPattern[] HexagonPatterns;
}
