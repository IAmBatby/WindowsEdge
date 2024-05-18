using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "ScriptableObjects/Settings/AudioSettings", order = 1)]
public class AudioSettings : ScriptableSettings
{
    private float m_globalSoundVolume;
    private float m_globalMusicVolume;

    [ShowInInspector]
    public float GlobalSoundVolume { get => m_globalSoundVolume; set { m_globalSoundVolume = value; ApplySettings(); } }

    [ShowInInspector]
    public float GlobalMusicVolume { get => m_globalMusicVolume; set => UpdateSetting(ref m_globalMusicVolume, value); }
}
