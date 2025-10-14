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
        EventManager.Instance.EnterFinishState += ReturnToPoolChildObjects;
    }

    void OnDisable()
    {
        EventManager.Instance.EnterGameState -= ResetData;
        EventManager.Instance.EnterFinishState -= ReturnToPoolChildObjects;
    }

    public void ResetData()
    {
        IsCreatedHexagon = false;
    }

    public void ReturnToPoolChildObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform group = transform.GetChild(i);

            for (int j = group.childCount - 1; j >= 0; j--)
            {
                Transform stack = group.GetChild(j);

                if (stack.childCount > 0)
                {
                    Transform hexParent = stack.GetChild(0);
                    for (int k = hexParent.childCount - 1; k >= 0; k--)
                    {
                        Transform hex = hexParent.GetChild(k);
                        hex.GetComponent<PooledObject>()?.ReturnToPool();
                    }
                }

                stack.GetComponent<PooledObject>()?.ReturnToPool();
            }

            group.GetComponent<PooledObject>()?.ReturnToPool();
        }
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
