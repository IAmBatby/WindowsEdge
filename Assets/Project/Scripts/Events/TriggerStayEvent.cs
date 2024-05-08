using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStayEvent : MonoBehaviour
{
    [HideInInspector] public TriggerEvent triggerEvent;

    public void OnTriggerStay(Collider other)
    {
        triggerEvent.onTriggerStay.Invoke(other);
    }
}
