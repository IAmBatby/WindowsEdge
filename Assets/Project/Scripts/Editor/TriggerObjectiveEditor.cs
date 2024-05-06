using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggerObjective))]
public class TriggerObjectiveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TriggerObjective triggerObjective = (TriggerObjective)target;

        int counter = 0;
        foreach (ObjectivePosition objectivePosition in triggerObjective.objectivePositions)
        {
            objectivePosition.transform.parent = triggerObjective.transform;
            objectivePosition.name = "Objective Position #" + counter;
            counter++;
        }
    }
}
