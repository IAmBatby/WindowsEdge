using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public string levelName;
    public string sceneName;

    [Header("Editor")]
    [HideInPlayMode, SerializeField] private List<ObjectiveData> levelObjectives = new List<ObjectiveData>();
    [ShowInPlayMode, ShowInInspector, ReadOnly] public List<ObjectiveData> debugLevelObjectives => levelObjectives;

    private Dictionary<ObjectiveData, ObjectiveData> assetLiveObjectiveDataDictionary = new Dictionary<ObjectiveData, ObjectiveData>();

    [Header("In-Game")]
    [ShowInPlayMode] public List<ObjectiveData> currentLevelObjectives = new List<ObjectiveData>();

    public void InitializeObjectives()
    {
        currentLevelObjectives.Clear();
        assetLiveObjectiveDataDictionary.Clear();
        foreach (ObjectiveData objectiveData in levelObjectives)
        {
            ObjectiveData newObjectiveData = ObjectiveData.Create(objectiveData);
            currentLevelObjectives.Add(newObjectiveData);
            assetLiveObjectiveDataDictionary.Add(objectiveData, newObjectiveData);
        }
    }

    public ObjectiveData GetObjectiveData(ObjectiveData assetObjectiveData)
    {
        return (assetLiveObjectiveDataDictionary[assetObjectiveData]);
    }
}
