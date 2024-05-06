using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LevelData levelData;

    [Range(0,1)]
    public float atmosphereLevel;
    public Color skyColor;
    public Color groundColor;

    public bool mouseOver;
    public Image image;
    public TextMeshProUGUI text;

    public Color hoverColor;

    public delegate void LevelEvent(LevelData levelData);

    public event LevelEvent onHover;
    public event LevelEvent onUnhover;

    public void Awake()
    {
        if (levelData != null)
            text.text = levelData.levelName.ToUpper();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    public void Hover()
    {
        mouseOver = true;
        image.color = hoverColor;
        text.color = Color.white;
        onHover?.Invoke(levelData);
        RefreshLevelInfoPopup();
    }

    public void Unhover()
    {
        image.color = Color.white;
        text.color = Color.black;
        onUnhover?.Invoke(levelData);
    }

    public void RefreshLevelInfoPopup()
    {
        MenuManager.Instance.RefreshLevelInfoPopup(levelData);
        foreach (LevelHover levelHover in MenuManager.Instance.levelHovers)
            if (levelHover != this)
                levelHover.Unhover();
    }
}
