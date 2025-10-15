using TMPro;
using UnityEngine;

public class LevelPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    
    void Start()
    {
        _levelText.text = $"Level {LevelManager.Instance.GetLevelNumber}";
    }
}
