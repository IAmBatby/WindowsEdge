using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelInfoPopup : MonoBehaviour
{
    public LevelData currentLevelData;

    public Animator popupAnimator;

    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI levelObjectivesText;

    public void LoadPopup(LevelData newLevelData)
    {
        currentLevelData = newLevelData;
        popupAnimator.SetBool("IsEnabled", true);
        levelNameText.text = currentLevelData.levelName.ToUpper();

        levelObjectivesText.text = string.Empty;

        foreach (ObjectiveData objectiveData in currentLevelData.levelObjectives)
            levelObjectivesText.text += "- " + GetObjectiveString(objectiveData) + "\n";
    }

    public void UnloadPopup()
    {
        popupAnimator.SetBool("IsEnabled", false);
    }

    public string GetObjectiveString(ObjectiveData objectiveData)
    {
        string returnString = objectiveData.objectiveDescription;

        if (objectiveData.objectiveState == ObjectiveState.Uncomplete)
            return (("<s>" + returnString.Colorize(Color.yellow) + "</s>").ToItalic());
        else
            return (returnString);
    }
}
