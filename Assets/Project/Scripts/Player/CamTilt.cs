using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CamTilt : MonoBehaviour
{
    [SerializeField] PlayerController m_player;

    [SerializeField] float m_tiltAngle = 15.0f;

    float m_smoothTilt = 0.0f;
    float m_smoothVel = 0.0f;
    [SerializeField] float m_smoothTime = 0.1f;

    float m_target = 0.0f;

    private void Update()
    {
        m_smoothTilt = Mathf.SmoothDamp(m_smoothTilt, m_target, ref m_smoothVel, m_smoothTime);
        UpdateTilt(m_smoothTilt);
    }

    public void TiltWallRun()
    {
        m_target = 0.0f;
        if(m_player.wallLeft)
        {
            m_target = -m_tiltAngle;
        }
        else if(m_player.wallRight)
        {
            m_target = m_tiltAngle;
        }
    }

    public void StopTilt()
    {
        m_target = 0.0f;
    }

    void UpdateTilt(float tiltAngle)
    {
        Vector3 euler = transform.localEulerAngles;
        euler.z = tiltAngle;
        transform.localEulerAngles = euler;
    }
}
