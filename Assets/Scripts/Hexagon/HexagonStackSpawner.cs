using UnityEngine;

public class HexagonStackSpawner : MonoBehaviour
{
     [Header("Prefab")]
    [SerializeField] private GameObject hexStackPrefab;

    // XZ düzleminde spawnlar. pattern ve origin’a göre konumlar.
    public void Spawn(HexagonPattern pattern, Transform origin)
    {
        float r = Mathf.Max(0.01f, pattern.HexRadius);

        foreach (Vector2Int cell in pattern.StackPositions)
        {
            Vector3 localPos = OddRPointyToLocal(cell.x, cell.y, r); // local konum
            Vector3 worldPos = origin.TransformPoint(localPos);

            // get from pool
            var go = Instantiate(hexStackPrefab, worldPos, Quaternion.identity, origin);

            // Initialize Hexagon Stack
            int health = Random.Range(pattern.HealthRange.x, pattern.HealthRange.y + 1);
            int hexagonCount = pattern.GetHexagonOnStackCount(health);
            var stack = go.GetComponent<HexagonStack>();

            stack.InitializeHexagonStack(health, hexagonCount, pattern.Color);
        }
    }

    /// <summary>
    /// Odd-r (pointy-top) offset -> XZ local koordinat.
    /// col = x, row = y
    /// </summary>
    private static Vector3 OddRPointyToLocal(int col, int row, float radius)
    {
        // pointy-top temel ölçüler
        float w = Mathf.Sqrt(3f) * radius; // yatay merkez-merkez
        float x = w * (col + ((row & 1) == 1 ? 0.5f : 0f));
        float z = (1.5f * radius) * row;   // dikey merkez-merkez adım
        return new Vector3(x, 0f, z);
    }
}
