using UnityEngine;
using Sirenix.OdinInspector;

public class OscillatingStackGroup : HexagonStackGroup
{
    [Title("Oscillation Settings")]
    [Range(0.1f, 10f)]
    public float Amplitude = 2f; // How far to move
    
    [Range(0.1f, 5f)]
    public float Frequency = 1f; // How fast to oscillate
    
    public Vector3 OscillationDirection = Vector3.right;
    
    private Vector3 _startPosition;
    private float _time;
    
    protected override void OnGroupInitialized()
    {
        GroupName = "Oscillating Group";
        _startPosition = transform.position;
    }
    
    protected override void UpdateGroupBehavior()
    {
        _time += Time.deltaTime * Frequency;
        float offset = Mathf.Sin(_time * Mathf.PI * 2f) * Amplitude;
        transform.position = _startPosition + OscillationDirection.normalized * offset;
    }
}
