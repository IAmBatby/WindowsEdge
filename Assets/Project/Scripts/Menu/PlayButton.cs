using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public Image playButtonBackground;
    public TextMeshProUGUI playButtonText;

    public Color selectColor;
    public Color unselectColor;

    public void TryLoadLevel()
    {
        if (MenuManager.Instance.currentSelectedLevel != null)
            SceneManager.LoadScene(MenuManager.Instance.currentSelectedLevel.sceneName);
    }

    public void Refresh()
    {
        if (MenuManager.Instance.currentSelectedLevel != null)
        {
            playButtonBackground.color = selectColor;
            playButtonText.color = Color.white;
        }
        else
        {
            playButtonBackground.color = unselectColor;
        }
    }
}
