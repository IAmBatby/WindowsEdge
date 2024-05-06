using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectivePosition : MonoBehaviour
{
    public BoxCollider triggerCollider;

    public delegate void ObjectiveAction(ObjectivePosition objectivePosition);
    public ObjectiveAction onTriggerEnter;
    public ObjectiveAction onTriggerExit;
    public bool isActivePosition;
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    public void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(this);
    }

    public void OnTriggerExit(Collider other)
    {
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
