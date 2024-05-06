using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TriggerObjective : Objective
{
    public List<ObjectivePosition> objectivePositions = new List<ObjectivePosition>();
    public ObjectivePosition currentObjectivePosition;

    public override void Awake()
    {
        foreach (ObjectivePosition objectivePosition in objectivePositions)
        {
            objectivePosition.onTriggerEnter += OnObjectiveReached;
            objectivePosition.gameObject.SetActive(false);
        }
        base.Awake();


        
    }

    protected override void OnObjectiveStart()
    {
        //base.OnObjectiveStart();

        Debug.Log("TriggerObjective Started!", transform);

        foreach (ObjectivePosition objectivePosition in objectivePositions)
            objectivePosition.gameObject.SetActive(true);

        currentObjectivePosition = objectivePositions.First();
    }

    protected override void OnObjectiveEnd()
    {
        base.OnObjectiveEnd();

        Debug.Log("TriggerObjective Ended!", transform);
    }

    public void OnObjectiveReached(ObjectivePosition objectivePosition)
    {
        if (currentObjectivePosition == objectivePosition)
        {
            Debug.Log("TriggerObjective Position Reached!");
            if (objectivePositions.Last() == objectivePosition)
                EndObjective(true);
            else
                currentObjectivePosition = objectivePositions[objectivePositions.IndexOf(objectivePosition) + 1];
        }
    }

    public void OnDrawGizmos()
    {
        int counter = 0;
        foreach (ObjectivePosition objectivePosition in objectivePositions)
        {
            Handles.Label(objectivePosition.transform.position, counter.ToString());
            Gizmos.color = Color.white; 
            if (counter != 0)
                Gizmos.DrawLine(objectivePositions[counter - 1].transform.position, objectivePositions[counter].transform.position);

            if (counter == 0)
                Gizmos.color = Color.green;
            else if (counter == objectivePositions.Count - 1)
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(objectivePosition.transform.position, Vector3.one);

            counter++;
        }
    }
}