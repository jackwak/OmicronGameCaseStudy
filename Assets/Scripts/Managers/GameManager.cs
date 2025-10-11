using UnityEngine;

public class GameManager : MonoBehaviour
{
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
}
