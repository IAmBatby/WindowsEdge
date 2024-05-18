using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioPreset", menuName = "ScriptableObjects/AudioPreset", order = 1)]
public class AudioPreset : ScriptableObject
{
    [PropertySpace, Title("Audio Assets")]
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioMixerGroup audioMixerGroup;

    [PropertySpace, Title("AudioSource General Settings")]
    public bool loop;

    [PropertySpace, Title("AudioSource Value Settings")]
    [Range(0f,1f)] public float volume;
    public Vector2 pitchMinMaxRange = Vector2.zero;
    [Range(0, 256)] public int priority = 128;
    [Range(0f, 1f)] public float stereoPan;
    [Range(0f, 1f)] public float spatialBlend;
    [Range(0f, 1f)] public float reverbZoneMix = 1f;

    [PropertySpace, Title("AudioSource 3D Settings")]
    [Range(0f, 5f)] public float dopplerLevel = 1f;
    [Range(0, 360)] public int spread;
    public Vector2 minMaxDistance = new Vector2(1, 500);
}
