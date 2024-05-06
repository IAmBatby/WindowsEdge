using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveState { Uncomplete, InProgress, Complete, Failed }
public class Objective : MonoBehaviour
{
    public string objectiveName;
    public ObjectiveState ObjectiveState { get; private set; }

    public virtual void Awake()
    {
        StartObjective();
    }


    public void StartObjective()
    {
        ChangeObjectiveState(ObjectiveState.InProgress);
        OnObjectiveStart();
    }

    public void EndObjective(bool successValue)
    {
        if (successValue == true)
            ChangeObjectiveState(ObjectiveState.Complete);
        else
            ChangeObjectiveState(ObjectiveState.Failed);

        OnObjectiveEnd();
    }

    public void ChangeObjectiveState(ObjectiveState newObjectiveState)
    {
        ObjectiveState = newObjectiveState;

        switch (ObjectiveState)
        {
            case ObjectiveState.Uncomplete:
                break;
            case ObjectiveState.InProgress:
                break;
            case ObjectiveState.Complete:
                break;
            case ObjectiveState.Failed:
                break;
        }
    }

    protected virtual void OnObjectiveStart()
    {
        Debug.Log("Meow!", transform);
    }

    protected virtual void OnObjectiveEnd()
    {

    }
}
