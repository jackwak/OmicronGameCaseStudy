using UnityEngine;

public class StateMachine
{
    private IState _currentState;

    public void ChangeState(IState newState)
    {
        if (_currentState == newState) return;

        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void Update()
    {
        _currentState?.Update();
    }
}

public class GameState : IState
{
    public void Enter()
    {
        EventManager.Instance.OnEnterGameState();
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
}
public class ReadyState : IState
{
    public void Enter()
    {
        EventManager.Instance.OnEnterReadyState();
    }

    public void Exit()
    {

    }

    public void Update()
    {
        Vector2 delta = InputManager.Instance.SwipeDelta;
        if (delta.magnitude > 0)
        {
            GameManager.Instance.SwitchToGameState();
        }
    }
}
