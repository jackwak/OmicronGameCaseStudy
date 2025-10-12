using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    [Header(" State Pattern ")]
    private StateMachine _stateMachine;
    private GameState _gameState;
    private ReadyState _readyState;


    void Start()
    {
        _stateMachine = new StateMachine();
        _gameState = new GameState();
        _readyState = new ReadyState();

        _stateMachine.ChangeState(_readyState);
    }

    void Update()
    {
        _stateMachine.Update();
    }

    public void SwitchToGameState()
    {
        _stateMachine.ChangeState(_gameState);
    }

    public void SwitchToReadyState()
    {
        _stateMachine.ChangeState(_readyState);
    }
}
