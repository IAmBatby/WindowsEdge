using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerExitEvent : MonoBehaviour
{
    [HideInInspector] public TriggerEvent triggerEvent;

    public void OnTriggerExit(Collider other)
    {
        triggerEvent.onTriggerExit.Invoke(other);
    }
}
