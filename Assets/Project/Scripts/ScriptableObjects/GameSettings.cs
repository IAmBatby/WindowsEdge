using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public List<ControlInfo> ControlInfos { get; set; } = new List<ControlInfo>();

    public ControlInfo forwardMovement;
    public ControlInfo backMovement;
    public ControlInfo leftMovement;
    public ControlInfo rightMovement;

    public ControlInfo togglePause;

    public ControlInfo slideMovement;
}

[System.Serializable]
public class ControlInfo
{
    public KeyCode keyCode;
    public Sprite icon;
}
