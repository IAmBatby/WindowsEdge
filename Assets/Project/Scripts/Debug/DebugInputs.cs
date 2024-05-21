using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInputs : MonoBehaviour
{
    [SerializeField] PlayerController m_controller;

    [SerializeField] float debug_force = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            m_controller.TryStartWallClimb();
        }
    }
}
