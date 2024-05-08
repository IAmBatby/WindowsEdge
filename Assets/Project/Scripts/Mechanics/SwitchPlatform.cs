using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlatform : MonoBehaviour
{
    public bool disabled;

    public Color color;
    public Color hollowColor;

    public MeshRenderer filledRenderer;
    public MeshRenderer hollowRenderer;
    public Collider mainCollider;
    public void Awake()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onFlip += OnFlip;

        filledRenderer.material.color = color;
        hollowRenderer.material.color = hollowColor;


        if (disabled == true)
        {
            filledRenderer.enabled = false;
            mainCollider.enabled = false;
        }
    }
    public void Flip()
    {
        if (disabled == false)
            GameManager.Instance.Flip();
    }

    public void OnFlip()
    {
        if (disabled == true)
        {
            disabled = false;
            filledRenderer.enabled = true;
            mainCollider.enabled = true;
        }
        else
        {
            disabled = true;
            filledRenderer.enabled = false;
            mainCollider.enabled = false;
        }
    }

    public void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onFlip -= OnFlip;
    }
}
