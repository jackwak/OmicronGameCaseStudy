using UnityEngine;

public class GroundFirstMoveStoper : StretchToScreen
{
    private bool _isFirstTriggered = true;

    void Start()
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

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PatternSpawnPosition patternSpawnPosition))
        {
            if (!patternSpawnPosition.IsCreatedHexagon) return;

            if (_isFirstTriggered)
            {
                _isFirstTriggered = false;
                EventManager.Instance.OnTriggerFirstSpawn();
            }
        }
    }

    private void ResetData()
    {
        _isFirstTriggered = true;
    }
}
