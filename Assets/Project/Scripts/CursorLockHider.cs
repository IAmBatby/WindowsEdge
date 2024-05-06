using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLockHider : MonoBehaviour
{
    [SerializeField] bool m_showCursor = true;

    private void Start()
    {
        OnValidate();
    }

    void HideCursor()
    {
        m_showCursor = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ShowCursor()
    {
        m_showCursor = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnValidate()
    {
        if(!Application.isPlaying)
        {
            return;
        }
        if (m_showCursor)
        {
            ShowCursor();
        }
        else
        {
            HideCursor();
        }
    }
}
