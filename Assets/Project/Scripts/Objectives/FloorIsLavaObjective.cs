using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorIsLavaObjective : Objective
{
    public List<ObjectivePosition> objectivePositions = new List<ObjectivePosition>();
    public ObjectivePosition startingPosition;
    public ObjectivePosition endingPosition;

    public override void Awake()
    {
        foreach (ObjectivePosition objectivePosition in objectivePositions)
            objectivePosition.Deactivate();
        base.Awake();

        startingPosition.onTriggerEnter += StartObjective;
        startingPosition.Activate();
    }

    protected override void OnObjectiveStart()
    {
        base.OnObjectiveStart();
        startingPosition.Deactivate();
        endingPosition.Activate();
        endingPosition.onTriggerEnter += SucceedObjective;

        foreach (ObjectivePosition objectivePosition in objectivePositions)
        {
            objectivePosition.Activate();
            objectivePosition.onTriggerEnter += FailObjective;
        }
    }

    public void FailObjective(ObjectivePosition _)
    {
        EndObjective(successValue: false);
        foreach (ObjectivePosition objectivePosition in objectivePositions)
        {
            objectivePosition.Deactivate();
            objectivePosition.onTriggerEnter -= FailObjective;
        }
        endingPosition.Deactivate();
        startingPosition.Activate();
        endingPosition.onTriggerEnter -= SucceedObjective;
    }

    public void SucceedObjective(ObjectivePosition _)
    {
        EndObjective(successValue: true);
        foreach (ObjectivePosition objectivePosition in objectivePositions)
        {
            objectivePosition.Deactivate();
            objectivePosition.onTriggerEnter -= FailObjective;
        }
        endingPosition.Deactivate();
        startingPosition.Activate();
        endingPosition.onTriggerEnter -= SucceedObjective;
    }


    public void OnDrawGizmos()
    {
        foreach (ObjectivePosition objectivePosition in objectivePositions)
            if (objectivePosition.isActivePosition == true)
            {
                Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 125 * 255);
                Gizmos.DrawCube(objectivePosition.transform.position, objectivePosition.transform.localScale);
            }

        Gizmos.color = Color.green;
        Gizmos.DrawCube(startingPosition.transform.position, Vector3.one);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(endingPosition.transform.position, Vector3.one);
    }
}
