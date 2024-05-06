using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    static MenuManager _instance;
    public static MenuManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MenuManager>();
            return _instance;
        }
    }

    public LevelInfoPopup levelInfoPopup;
    public List<LevelHover> levelHovers = new List<LevelHover>();
    public LevelHover activeHover;

    public List<Transform> menuParents = new List<Transform>();
    public Transform activeMenuParent;
    public PlayButton playButton;

    public Material skyboxMaterial;

    public LevelData currentSelectedLevel;

    public float menuLerpRate;
    public float atmosphereLerpRate;

    public float currentAtmosphereRate;
    public float startingAtmosphereRate;
    public Color currentSkyColor;
    public Color currentGroundColor;

    public void RefreshLevelInfoPopup(LevelData levelData)
    {
        foreach (LevelHover hover in levelHovers)
            if (hover.levelData == levelData)
                activeHover = hover;
        currentSelectedLevel = levelData;
        playButton.Refresh();
        levelInfoPopup.LoadPopup(levelData);
    }

    public void ToggleActiveMenu(GameObject gameObject)
    {
        if (menuParents.Contains(gameObject.transform))
            activeMenuParent = gameObject.transform;
    }

    public void Update()
    {
        foreach (Transform menuParent in menuParents)
        {
            if (menuParent == activeMenuParent)
                menuParent.localScale = Vector3.Lerp(menuParent.localScale, Vector3.one, 0.5f * menuLerpRate);
            else
                menuParent.localScale = Vector3.Lerp(menuParent.localScale, Vector3.one * 2, 0.5f * menuLerpRate);
        }

        if (activeHover != null && currentSelectedLevel != null)
        {
            currentAtmosphereRate = Mathf.Lerp(currentAtmosphereRate, activeHover.atmosphereLevel, 0.5f * atmosphereLerpRate);
            currentSkyColor = Color.Lerp(currentSkyColor, activeHover.skyColor, 0.5f * atmosphereLerpRate);
            currentGroundColor = Color.Lerp(currentGroundColor, activeHover.groundColor, 0.5f * atmosphereLerpRate);
        }
        RenderSettings.skybox.SetFloat("_AtmosphereThickness", currentAtmosphereRate);
        //RenderSettings.skybox.SetColor("_SkyTint", currentSkyColor);
        //RenderSettings.skybox.SetColor("_GroundColor", currentGroundColor);
    }
}
