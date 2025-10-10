using UnityEngine;

public class PatternSpawnPosition : MonoBehaviour
{
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
