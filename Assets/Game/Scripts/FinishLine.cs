using UnityEngine;

public class FinishLine : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement playerMovement))
        {
            GameManager.Instance.SwitchToFinishState();
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        }
    }
}
