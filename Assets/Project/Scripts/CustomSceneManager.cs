using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : SceneManagerAPI
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        overrideAPI = new CustomSceneManager();
    }

    protected override int GetNumScenesInBuildSettings()
    {
        Debug.LogWarning("SceneManager.GetNumScenesInBuildSettings() called, please load scenes by path to avoid issues when scenes are reordered.");
        return base.GetNumScenesInBuildSettings();
    }

    protected override Scene GetSceneByBuildIndex(int buildIndex)
    {
        Debug.Log($"SceneManager.GetSceneByBuildIndex(buildIndex = {buildIndex}) called, please load scenes by path to avoid issues when scenes are reordered.");
        return base.GetSceneByBuildIndex(buildIndex);
    }

    protected override AsyncOperation LoadFirstScene(bool mustLoadAsync)
    {
        Debug.Log("Loading First Scene");
        return (base.LoadFirstScene(mustLoadAsync));
    }
}
