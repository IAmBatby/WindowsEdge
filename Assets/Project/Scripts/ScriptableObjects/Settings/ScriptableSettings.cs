using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

public class ScriptableSettings : ScriptableObject
{

    public enum DebugValues { Disabled, Enabled }
    [SerializeField]
    [EnumToggleButtons] public DebugValues debugValueToggle;
    public bool IsDebuggingEnabled => debugValueToggle == DebugValues.Enabled;

    public virtual void ApplySettings()
    {
        Debug.Log("Applied " + GetType().ToString());
    }

    public void OnValidate()
    {
        //ApplySettings();
    }

    /*public void UpdateSetting(ref float targetValue, float newValue)
    {
        targetValue = newValue;
        ApplySettings();
    }*/

    public void UpdateSetting<T>(ref T targetValue, T newValue)
    {
        targetValue = newValue;
        ApplySettings();
    }
}
