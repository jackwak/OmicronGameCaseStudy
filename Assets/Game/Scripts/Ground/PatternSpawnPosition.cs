using UnityEngine;

public class PatternSpawnPosition : MonoBehaviour
{
    public bool IsCreatedHexagon = false;

    void Awake()
    {
        ResetData();
    }

    void OnEnable()
    {
        EventManager.Instance.EnterGameState += ResetData;
    }

    void OnDisable()
    {
        EventManager.Instance.EnterGameState -= ResetData;
    }

    public void ResetData()
    {
        IsCreatedHexagon = false;
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere to visualize spawn position
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // Draw a label
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up, "Pattern Spawn");
#endif
    }
}
