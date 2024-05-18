using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

[CreateAssetMenu(fileName = "ControlSettings", menuName = "ScriptableObjects/Settings/ControlSettings", order = 1)]
public class ControlSettings : ScriptableSettings
{
    [SerializeField, HideInInspector] private ControlInfo m_forwardInfo;
    [SerializeField, HideInInspector] private ControlInfo m_backwardInfo;
    [SerializeField, HideInInspector] private ControlInfo m_leftInfo;
    [SerializeField, HideInInspector] private ControlInfo m_rightInfo;

    [SerializeField, HideInInspector] private ControlInfo m_jumpInfo;
    [SerializeField, HideInInspector] private ControlInfo m_lowerInfo;
    [SerializeField, HideInInspector] private ControlInfo m_interactInfo;

    [SerializeField, HideInInspector] private ControlInfo m_quickRestartInfo;
    [SerializeField, HideInInspector] private ControlInfo m_objectiveInfo;
    [SerializeField, HideInInspector] private ControlInfo m_menuInfo;

    [PropertySpace]
    [Title("General Movement")]

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo ForwardControl { get => m_forwardInfo; set => UpdateSetting(ref m_forwardInfo, value); }

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo BackwardControl { get => m_backwardInfo; set => UpdateSetting(ref m_backwardInfo, value); }

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo LeftControl { get => m_leftInfo; set => UpdateSetting(ref m_leftInfo, value); }

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo RightControl { get => m_rightInfo; set => UpdateSetting(ref m_rightInfo, value); }

    //

    [PropertySpace]
    [Title("Additional Movement")]

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo JumpControl { get => m_jumpInfo; set => UpdateSetting(ref m_jumpInfo, value); }

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo LowerControl { get => m_lowerInfo; set => UpdateSetting(ref m_lowerInfo, value); }

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo InteractControl { get => m_interactInfo; set => UpdateSetting(ref m_interactInfo, value); }

    //

    [PropertySpace]
    [Title("Menus")]

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo QuickRestartControl { get => m_quickRestartInfo; set => UpdateSetting(ref m_quickRestartInfo, value); }

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo ObjectiveControl { get => m_objectiveInfo; set => UpdateSetting(ref m_objectiveInfo, value); }

    [ShowInInspector, InlineProperty, HideReferencePicker]
    public ControlInfo MenuControl { get => m_menuInfo; set => UpdateSetting(ref m_menuInfo, value); }
}

[System.Serializable] [DeclareHorizontalGroup("values")]
public class ControlInfo
{
    [Group("values")] [HideLabel] public KeyCode keyCode;
    [Group("values")] [HideLabel] public Sprite keyCodeSprite;
}
