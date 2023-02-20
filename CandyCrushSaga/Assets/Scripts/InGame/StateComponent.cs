using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static State;

public class StateInfo
{
    string stateName;
    float stateTime;
    string prevState;

    public string StateName
    {
        get { return stateName; }
        set { stateName = value; }
    }

    public float StateTime
    {
        get { return stateTime; }
        set { stateTime = value; }
    }

    public string PrevState
    {
        get { return prevState; }
        set { prevState = value; }
    }
}

public class State
{
    StateInfo info;

    public delegate void StartState();
    public delegate void UpdateState();
    public delegate void EndState();

    public StartState startState;
    public UpdateState updateState;
    public EndState endState;

    public StateInfo Info
    {
        get { return info; }
        set { info = value; }
    }

    //여러 개의 스테이트 함수를 추가, 제거할 수 있음(멀티캐스트)
    public void SetStateStart(StartState _State)
    {
        startState = _State;
    }

    public void AddStateStart(StartState _State)
    {
        startState += _State;
    }

    public void RemoveStateStart(StartState _State)
    {
        startState -= _State;
    }

    public void SetStateUpdate(UpdateState _State)
    {
        updateState = _State;
    }

    public void AddStateUpdate(UpdateState _State)
    {
        updateState += _State;
    }

    public void RemoveStateUpdate(UpdateState _State)
    {
        updateState -= _State;
    }

    public void SetStateEnd(EndState _State)
    {
        endState= _State;
    }

    public void AddStateEnd(EndState _State)
    {
        endState += _State;
    }

    public void RemoveStateEnd(EndState _State)
    {
        endState -= _State;
    }
}


public class StateComponent : MonoBehaviour
{
    Dictionary<string, State> allStates = new Dictionary<string, State>();
    State curState;

    public StateInfo CurStateInfo
    {
        get { return curState.Info; }
        set { curState.Info = value; }
    }

    public void StateUpdate()
    {
        if (null != curState
            && null != curState.updateState)
        {
            curState.updateState();
        }
    }

    public void CreateState(string _StateName, UpdateState _Update, StartState _Start = null, EndState _End = null)
    {
        _StateName.ToUpper();

        if (true == allStates.ContainsKey(_StateName))
        {
            Debug.Log(_StateName + "는(은) 이미 존재하는 스테이트입니다.");
            return;
        }

        State newState = new State();
        newState.Info = new StateInfo();
        newState.Info.StateName = "";
        newState.Info.StateTime = 0.0f;
        newState.Info.PrevState = "";

        newState.SetStateStart(_Start);
        newState.SetStateUpdate(_Update);
        newState.SetStateEnd(_End);

        allStates.Add(_StateName, newState);
    }

    public void ChangeState(string _StateName)
    {
        _StateName.ToUpper();

        if (false == allStates.ContainsKey(_StateName))
        {
            Debug.Log(_StateName + "는(은) 존재하지 않는 스테이트입니다.");
            return;
        }

        string prevState = "";

        if (null != curState)
        {
            prevState = curState.Info.PrevState;

            if (null != curState.endState)
            {
                curState.endState();
            }
        }

        curState = allStates[_StateName];
        curState.Info.StateName = _StateName;
        curState.Info.StateTime = 0.0f;
        curState.Info.PrevState = prevState;

        if (null != curState.startState)
        {
            curState.startState();
        }

    }
}
