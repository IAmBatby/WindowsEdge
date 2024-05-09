using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectivePosition : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    private ObjectiveController objectiveController;

    public delegate void ObjectiveAction(ObjectivePosition objectivePosition);
    public ObjectiveAction onTriggerEnter;
    public ObjectiveAction onTriggerExit;
    public bool isActivePosition;
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    public void Awake()
    {
        Deactivate();
    }

    public void Initialize(ObjectiveController newObjectiveController, Color newColor)
    {
        objectiveController = newObjectiveController;
        meshRenderer.material.color = new Color(newColor.r, newColor.g, newColor.b, meshRenderer.material.color.a);
        meshRenderer.material.SetColor("_EmissionColor", new Color(newColor.r, newColor.g, newColor.b, meshRenderer.material.GetColor("_EmissionColor").a));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isActivePosition)
            onTriggerEnter?.Invoke(this);
    }

    public void OnTriggerExit(Collider other)
    {
        if (isActivePosition)
            onTriggerExit?.Invoke(this);
    }

    public void Activate()
    {
        isActivePosition = true;
        onActivate.Invoke();
    }

    public void Deactivate()
    {
        isActivePosition = false;
        onDeactivate.Invoke();
    }
}
