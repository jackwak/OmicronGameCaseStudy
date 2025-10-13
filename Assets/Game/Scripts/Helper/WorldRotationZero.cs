using UnityEngine;

public class WorldRotationZero : MonoBehaviour
{
    void LateUpdate()
    {
        // Global (world) rotasyonu sıfırla
        transform.rotation = Quaternion.identity;
    }
}
