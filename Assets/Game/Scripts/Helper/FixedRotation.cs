using UnityEngine;

public class FixedRotation : MonoBehaviour
{
    private Quaternion _initialRotation;

    void Start()
    {
        _initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = _initialRotation;
    }
}
