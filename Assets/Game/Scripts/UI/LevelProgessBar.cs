using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera _cam;          
    [SerializeField] private Image fillImage;        
    [SerializeField] private GroundMover _groundMover;   
    [SerializeField] private Transform _playerTransform;    

    [Header("Options")]
    [SerializeField] private bool _useUnscaledTime = false;   

    private float _screenHeightW;   
    private float _totalDistance; 
    private float _coveredDistance;  
    private float _initialPlayerY;   
    private float _screenBottomY;  
    private bool _isMoving;

    void Awake()
    {
        if (_cam == null) _cam = Camera.main;
    }

    void OnEnable()
    {
        EventManager.Instance.EnterGameState += SetMovingTrue;
    }

    void OnDisable()
    {
        EventManager.Instance.EnterGameState -= SetMovingTrue;
    }

    private void SetMovingTrue() { _isMoving = true; }
    private void SetMovingFalse() { _isMoving = false; }

    void Start()
    {
        SetMovingFalse();
        
        _screenBottomY = _cam.transform.position.y - _cam.orthographicSize;
        
        if (_playerTransform != null)
        {
            _initialPlayerY = _playerTransform.position.y;
        }
        
        RecalculateTotals();
        UpdateUI();
    }

    void Update()
    {
        if (!_isMoving) return;
        if (_totalDistance <= 0f) return;

        float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        float v = Mathf.Abs(_groundMover.Speed);

        _coveredDistance += v * dt;
        _coveredDistance = Mathf.Min(_coveredDistance, _totalDistance);

        UpdateUI();
    }

    public void RecalculateTotals()
    {
        _screenHeightW = 2f * _cam.orthographicSize;
        int groundCount = LevelManager.Instance.GetPatternCount() + 2;
        float totalGroundDistance = groundCount * _screenHeightW;
        float playerOffsetFromBottom = _initialPlayerY - _screenBottomY;
        
        _totalDistance = totalGroundDistance - playerOffsetFromBottom;
        _coveredDistance = Mathf.Clamp(_coveredDistance, 0f, _totalDistance);
    }

    public void ResetProgress()
    {
        _coveredDistance = 0f;
        UpdateUI();
    }

    private void UpdateUI()
    {
        float fill = (_totalDistance > 0f) ? (_coveredDistance / _totalDistance) : 0f;
        if (fillImage != null) fillImage.fillAmount = fill;
    }
}