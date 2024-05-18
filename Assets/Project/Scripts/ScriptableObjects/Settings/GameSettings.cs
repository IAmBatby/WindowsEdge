using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Settings/GameSettings", order = 1)]
public class GameSettings : ScriptableSettings
{
    private bool m_subtitlesEnabled;

    [PropertySpace]
    [Title("Accessibility")]

    [ShowInInspector]
    public bool SubtitlesEnabled { get => m_subtitlesEnabled; set => UpdateSetting(ref m_subtitlesEnabled, value); }
}

