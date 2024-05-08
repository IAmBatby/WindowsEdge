using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public string levelName;
    public string sceneName;

    [Header("Editor")]
    [SerializeField] private List<ObjectiveData> levelObjectives = new List<ObjectiveData>();

    private Dictionary<ObjectiveData, ObjectiveData> assetLiveObjectiveDataDictionary = new Dictionary<ObjectiveData, ObjectiveData>();

    [Header("In-Game")]
    [HideInInspector] public List<ObjectiveData> currentLevelObjectives = new List<ObjectiveData>();

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
