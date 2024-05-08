using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class HelperFunctions
{
    public static bool isInEditor => IsInEditorCheck();

    public static IEnumerable<ScriptableObject> GetScriptableObjects(Type type)
    {
        if (isInEditor)
        {
            IEnumerable<ScriptableObject> allScriptableObjects;
            List<ScriptableObject> returnScriptableObjects = new List<ScriptableObject>();


            allScriptableObjects = UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject")
            .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
            .Select(x => UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(x));

            foreach (ScriptableObject item in allScriptableObjects)
            {
                if (item.GetType() == type)
                    returnScriptableObjects.Add(item);
                else if (item.GetType().BaseType != null && item.GetType().BaseType == type)
                    returnScriptableObjects.Add(item);
            }
            return (returnScriptableObjects);
        }
        else
            return (null);
    }

    public static IEnumerable<Type> GetTypes(Type filterType)
    {
        List<Type> moveTypes = new List<Type>();

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            //Debug.Log(type.ToString());
            if (type == filterType || type.IsSubclassOf(filterType))
                moveTypes.Add(type);
        }

        return (moveTypes);
    }


    static bool IsInEditorCheck()
    {
        bool returnBool = false;
#if (UNITY_EDITOR)
        returnBool = true;
#endif
        return (returnBool);
    }
}
