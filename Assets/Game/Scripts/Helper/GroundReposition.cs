using UnityEngine;

public class GroundReposition : MonoBehaviour
{
    private float _screenHeightWorld;

    void Start()
    {
        // Kameranın alt ve üst noktalarını world space'e çeviriyoruz
        Camera cam = Camera.main;
        Vector3 bottom = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 top = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, cam.nearClipPlane));

        // İki nokta arasındaki world-space yüksekliği
        _screenHeightWorld = top.y - bottom.y;

        // Şimdi objeyi ekran yüksekliği kadar aşağı indir
        transform.position += Vector3.down * _screenHeightWorld * 2f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PatternSpawnPosition patternSpawnPosition))
        {
            patternSpawnPosition.ReturnToPoolChildObjects();
            
            Vector3 pos = patternSpawnPosition.transform.parent.position;
            patternSpawnPosition.transform.parent.position = new Vector3(pos.x, pos.y + _screenHeightWorld * 3f, pos.z);
        }
    }
}
