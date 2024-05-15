using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimpleTimer
{
    float m_timer = 0.0f;
    public float completeTime = 1.0f;

    public float currentTime { get {  return m_timer; } set { m_timer = value; } }

    public bool Tick(float deltaTime)
    {
        m_timer += deltaTime;
        return m_timer > completeTime;
    }

    public void Reset()
    {
        m_timer = 0.0f;
    }
}
