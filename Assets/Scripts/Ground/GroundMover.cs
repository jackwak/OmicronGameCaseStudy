using UnityEngine;

public class GroundMover : MonoBehaviour
{
    [SerializeField] private GroundMoverSettings _settings;
    public float Speed { get; private set; }

    void Start()
    {
        StopGrounds();
    }

    void OnEnable()
    {
        EventManager.Instance.EnterGameState += SetStartSpeed;
        EventManager.Instance.TriggerFirstSpawn += SetNormalSpeed;
        EventManager.Instance.ExitGameState += StopGrounds;
    }

    void OnDisable()
    {
        EventManager.Instance.EnterGameState -= SetStartSpeed;
        EventManager.Instance.TriggerFirstSpawn -= SetNormalSpeed;
        EventManager.Instance.ExitGameState -= StopGrounds;
    }

    void Update()
    {
        transform.Translate(Vector3.down * Speed * Time.deltaTime);
    }

    public void SetStartSpeed()
    {
        Speed = _settings.StartSpeed;
    }

    public void SetNormalSpeed()
    {
        Speed = _settings.Speed;
    }

    public void StopGrounds()
    {
        Speed = 0;
    }
}
