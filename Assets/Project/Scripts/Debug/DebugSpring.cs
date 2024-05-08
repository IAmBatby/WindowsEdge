using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpring : MonoBehaviour
{
    [SerializeField] float m_impulse = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var dir = other.transform.position - transform.position;
        dir = dir.normalized;

        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddImpulse(dir * m_impulse);
        }

        Debug.Log("Boing");
    }
}
