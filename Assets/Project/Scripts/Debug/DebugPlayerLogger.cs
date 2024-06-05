using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayerLogger : MonoBehaviour
{
    [SerializeField] PlayerController m_player;

    public void LogPlayerState(string msg)
    {
        msg += m_player.currentState.ToString();
        Debug.Log(msg);
    }
}
