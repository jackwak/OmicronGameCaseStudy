using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public UnityAction StartGame;

    //State Events
    public UnityAction EnterReadyState;
    public UnityAction EnterFinishState;
    public UnityAction EnterGameState;
    public UnityAction ExitGameState;


    public UnityAction CollideWithOctagon;
    public UnityAction FinishGame;
    public UnityAction GameWin;
    public UnityAction GameLose;
    public UnityAction TriggerFirstSpawn;
    public UnityAction<WeaponData> InitializeWeaponData;


    public void OnStartGame()
    {
        StartGame?.Invoke();
    }

    public void OnFinishGame()
    {
        FinishGame?.Invoke();
    }

    public void OnTriggerFirstSpawn()
    {
        TriggerFirstSpawn?.Invoke();
    }

    public void OnEnterReadyState()
    {
        EnterReadyState?.Invoke();
    }

    public void OnEnterGameState()
    {
        EnterGameState?.Invoke();
    }

    public void OnExitGameState()
    {
        ExitGameState?.Invoke();
    }

    public void OnEnterFinishState()
    {
        EnterFinishState?.Invoke();
    }

    public void OnCollideWithOctagon()
    {
        CollideWithOctagon?.Invoke();
    }

    public void OnGameLose()
    {
        GameLose?.Invoke();
    }

    public void OnGameWin()
    {
        GameWin?.Invoke();
    }

    public void OnInitializeWeaponData(WeaponData weaponData)
    {
        InitializeWeaponData?.Invoke(weaponData);
    }
}
