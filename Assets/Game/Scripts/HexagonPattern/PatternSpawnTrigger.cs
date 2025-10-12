using UnityEngine;

public class PatternSpawnTrigger : StretchToScreen
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PatternSpawnPosition patternSpawnPosition))
        {
            // Trigger pattern spawn
            patternSpawnPosition.IsCreatedHexagon = true;
            LevelManager.Instance.SpawnNextPattern(patternSpawnPosition.transform);
        }
    }
}