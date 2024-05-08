using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveData", menuName = "ScriptableObjects/ObjectiveData", order = 1)]
public class ObjectiveData : ScriptableObject
{
    public string objectiveName;
    public string objectiveDescription;
    public ObjectiveState objectiveState;

    public static ObjectiveData Create(ObjectiveData objectiveData)
    {
        ObjectiveData instancedObjectiveData = (ObjectiveData)ScriptableObject.CreateInstance(objectiveData.GetType());
        instancedObjectiveData.name = "Live" + objectiveData.name;
        instancedObjectiveData.objectiveName = objectiveData.objectiveName;
        instancedObjectiveData.objectiveDescription = objectiveData.objectiveDescription;
        instancedObjectiveData.objectiveState = ObjectiveState.Uncomplete;

        return (instancedObjectiveData);
    }
}
