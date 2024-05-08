using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventCollider : UnityEvent<Collider>
{
}

public class TriggerEvent : MonoBehaviour
{
    public UnityEventCollider onTriggerEnter;
    public UnityEventCollider onTriggerStay;
    public UnityEventCollider onTriggerExit;

    private void Awake()
    {
        /*
        TriggerEnterEvent triggerEnterForwarder;
        TriggerStayEvent triggerStayForwarder;
        TriggerExitEvent triggerExitForwarder;

        if (onTriggerEnter.GetPersistentEventCount() > 0)
        {
            triggerEnterForwarder = gameObject.AddComponent<TriggerEnterEvent>();
            triggerEnterForwarder.triggerEvent = this;
        }

        if (onTriggerStay.GetPersistentEventCount() > 0)
        {
            triggerStayForwarder = gameObject.AddComponent<TriggerStayEvent>();
            triggerStayForwarder.triggerEvent = this;
        }


        if (onTriggerExit.GetPersistentEventCount() > 0)
        {
            triggerExitForwarder = gameObject.AddComponent<TriggerExitEvent>();
            triggerExitForwarder.triggerEvent = this;
        }
        */
    }

    public void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }

    public void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke(other);
    }

    public void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke(other);
    }
}