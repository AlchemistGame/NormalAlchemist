using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public virtual State CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            if (_currentState == value)
                return;

            if (_currentState != null)
                _currentState.Exit();

            _currentState = value;

            if (_currentState != null)
                _currentState.Enter();
        }
    }
    protected State _currentState;

    public virtual void ChangeState<T>() where T : State, new()
    {
        CurrentState = new T();
    }
}