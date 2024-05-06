using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePosition : MonoBehaviour
{
    public BoxCollider triggerCollider;

    public delegate void ObjectiveAction(ObjectivePosition objectivePosition);
    public ObjectiveAction onTriggerEnter;
    public ObjectiveAction onTriggerExit;

    public void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(this);
    }

    public void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(this);
    }
}
