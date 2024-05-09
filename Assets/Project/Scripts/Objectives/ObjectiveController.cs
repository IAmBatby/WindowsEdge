using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectiveController : MonoBehaviour
{
    [Header("Settings")]

    public ObjectiveData objectiveData;
    public Color objectiveColour;
    public bool activateOnStart;
    public bool isOrdered;
    public float requiredTime;

    [Space(15)]
    [Header("Objective Positions")]

    public ObjectivePosition startingPosition;

    [SerializeField] private List<ObjectivePosition> requiredObjectivePositions = new List<ObjectivePosition>();

    [SerializeField] private List<ObjectivePosition> failObjectivePositions = new List<ObjectivePosition>();

    [Space(15)]
    [Header("Runtime")]

    public List<ObjectivePosition> activeObjectivePositions = new List<ObjectivePosition>();

    public void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentLevel != null && objectiveData != null)
        {
            objectiveData = GameManager.Instance.currentLevel.GetObjectiveData(objectiveData);
            Initialize();

            if (activateOnStart == true)
                StartObjective(startingPosition);
        }
    }

    private void Initialize()
    {
        ChangeObjectiveState(ObjectiveState.Uncomplete);
        
        InitializeObjectivePosition(startingPosition, Color.green);

        foreach (ObjectivePosition requiredObjectivePosition in requiredObjectivePositions)
            InitializeObjectivePosition(requiredObjectivePosition, objectiveColour);

        foreach (ObjectivePosition failObjectivePosition in failObjectivePositions)
            InitializeObjectivePosition(failObjectivePosition, Color.red);

        startingPosition.Activate();
        startingPosition.onTriggerEnter += StartObjective;
    }


    private void StartObjective(ObjectivePosition startingPosition)
    {
        Debug.Log("ObjectiveController Started!", transform);

        ChangeObjectiveState(ObjectiveState.InProgress);

        activeObjectivePositions = new List<ObjectivePosition>(requiredObjectivePositions);

        startingPosition.Deactivate();

        if (isOrdered == true)
            requiredObjectivePositions.First().Activate();
        else
            foreach (ObjectivePosition requiredObjectivePosition in requiredObjectivePositions)
                requiredObjectivePosition.Activate();

        foreach (ObjectivePosition failObjectivePosition in failObjectivePositions)
            failObjectivePosition.Activate();
    }

    public void ObjectivePositionReached(ObjectivePosition objectivePosition)
    {
        if (isOrdered == false)
        {
            activeObjectivePositions.Remove(objectivePosition);
            objectivePosition.Deactivate();
        }
        else if (activeObjectivePositions.First() == objectivePosition)
        {
            activeObjectivePositions.Remove(objectivePosition);
            objectivePosition.Deactivate();
        }

        if (activeObjectivePositions.Count == 0)
            EndObjective(wasSuccessful: true);
        else if (isOrdered)
            activeObjectivePositions.First().Activate();
    }

    private void OnObjectivePositionTriggerEnter(ObjectivePosition objectivePosition)
    {
        if (activeObjectivePositions.Contains(objectivePosition))
            ObjectivePositionReached(objectivePosition);
        else if (failObjectivePositions.Contains(objectivePosition))
            EndObjective(wasSuccessful: false);

    }

    private void EndObjective(bool wasSuccessful)
    {
        Debug.Log("ObjectiveController Ended!", transform);

        foreach (ObjectivePosition requiredObjectivePosition in requiredObjectivePositions)
            requiredObjectivePosition.Deactivate();

        foreach (ObjectivePosition failObjectivePosition in failObjectivePositions)
            failObjectivePosition.Deactivate();

        startingPosition.Activate();

        if (wasSuccessful == true)
            ChangeObjectiveState(ObjectiveState.Complete);
        else
            ChangeObjectiveState(ObjectiveState.Failed);
    }

    private void InitializeObjectivePosition(ObjectivePosition objectivePosition, Color color)
    {
        objectivePosition.Initialize(this, color);
        objectivePosition.onTriggerEnter += OnObjectivePositionTriggerEnter;
    }

    public void ChangeObjectiveState(ObjectiveState newObjectiveState)
    {
        objectiveData.objectiveState = newObjectiveState;

        switch (objectiveData.objectiveState)
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

    private void OnDrawGizmos()
    {
        int counter = 0;

        List<ObjectivePosition> debugPositions;
        if (activeObjectivePositions.Count != 0)
            debugPositions = new List<ObjectivePosition>(activeObjectivePositions);
        else
            debugPositions = new List<ObjectivePosition>(requiredObjectivePositions);

        if (startingPosition != null)
            debugPositions.Insert(0, startingPosition);

        foreach (ObjectivePosition objectivePosition in debugPositions)
        {
            if (objectivePosition != null)
            {
                Handles.Label(objectivePosition.transform.position, counter.ToString());
                Gizmos.color = Color.white;

                if (isOrdered)
                    if (counter != 0)
                        Gizmos.DrawLine(debugPositions[counter - 1].transform.position, debugPositions[counter].transform.position);

                if (counter == 0)
                    Gizmos.color = Color.green;
                else if (counter == debugPositions.Count - 1)
                    Gizmos.color = Color.red;

                Gizmos.DrawWireCube(objectivePosition.transform.position, Vector3.one);

                counter++;
            }
        }
    }
}
