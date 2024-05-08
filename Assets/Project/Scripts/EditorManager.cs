using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[FilePath("Project/ScriptableObjects/scriptableManager.foo", FilePathAttribute.Location.PreferencesFolder)]
public class EditorManager : ScriptableSingleton<EditorManager>
{
    public static bool cachedIsPlaying;
    [InitializeOnLoadMethod]
    public static void StartUpdate()
    {
        cachedIsPlaying = Application.isPlaying;
        EditorApplication.update -= EditorManager.instance.EditorUpdate;
        EditorApplication.update += EditorManager.instance.EditorUpdate;
        EditorSceneManager.activeSceneChangedInEditMode -= EditorManagerData.OnSceneLoad;
        EditorSceneManager.activeSceneChangedInEditMode += EditorManagerData.OnSceneLoad;

        if (EditorManagerData.currentEditorLevel == null)
        {
            foreach (LevelData levelData in EditorManagerData.AllLevels)
            {
                string comparePath = EditorSceneManager.GetSceneManagerSetup()[0].path.Substring(EditorSceneManager.GetSceneManagerSetup()[0].path.LastIndexOf("/") + 1).Replace(".unity", string.Empty);
                if (comparePath == levelData.sceneName)
                    EditorManagerData.ChangeCurrentLevel(levelData);
            }
            //
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void OnFirstSceneLoad()
    {
        //Debug.Log("Yipz");
        if (EditorManagerData.currentEditorLevel != null)
        {
            //Debug.Log("Loading Level!");
            SceneManager.LoadScene("MainMenu");
            GameManager.onInitalize += instance.LoadCurrentEditorLevel;
        }
    }

    public void LoadCurrentEditorLevel()
    {
        MenuManager.Instance.RefreshLevelInfoPopup(EditorManagerData.currentEditorLevel);
        MenuManager.Instance.ToggleActiveMenu(MenuManager.Instance.gameplayPlayMenuParent.gameObject);
        MenuManager.Instance.levelSelectLevelInfoPopup.UnloadPopup();
        GameManager.Instance.LoadLevel(EditorManagerData.currentEditorLevel);
        GameManager.onInitalize -= LoadCurrentEditorLevel;
    }

    public void EditorUpdate()
    {
        //Debug.Log("Yippeee");


        if (cachedIsPlaying != Application.isPlaying)
        {
            OnApplicationPlayingChange();
            cachedIsPlaying = Application.isPlaying;
        }
    }

    public void OnApplicationPlayingChange()
    {
        if (!string.IsNullOrEmpty(EditorManagerData.cachedSceneName))
            EditorManagerData.LoadPreviousLevel();
    }

    public void OpenScene(string scenePath, OpenSceneMode sceneMode)
    {
        //EditorSceneManager.OpenScene(scenePath, sceneMode);

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath, sceneMode);
        }
    }

    public void LoadLevel()
    {
        //Debug.Log("Fake Loading Level!");
        GameManager.onInitalize -= LoadLevel;
        foreach (ScriptableObject levelDataObj in HelperFunctions.GetScriptableObjects(typeof(LevelData)))
        {
            LevelData levelData = (LevelData)levelDataObj;
            //Debug.Log("Active SceneName: " + SceneManager.GetActiveScene().name);
            Debug.Log(levelData.levelName + " | " + levelData.sceneName);
            if (EditorManagerData.cachedSceneName.Contains(levelData.sceneName))
            {
                GameManager.Instance.LoadLevel(levelData);
            }
        }
    }

    public static string GetScenePathFromLevelData(LevelData levelData)
    {
        string returnPath = string.Empty;
        for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
        {
            string comparePath = SceneUtility.GetScenePathByBuildIndex(i).Substring(SceneUtility.GetScenePathByBuildIndex(i).LastIndexOf("/") + 1).Replace(".unity", string.Empty);
            if (comparePath == levelData.sceneName)
                returnPath = SceneUtility.GetScenePathByBuildIndex(i);
        }

        return (returnPath);
    }
}

public static class EditorManagerData
{
    public static EditorManager EditorManager => EditorManager.instance;
    public static string cachedSceneName;
    public static LevelData currentEditorLevel;

    public static string scenesPath = "Assets/Project/Scenes/";

    static List<LevelData> _levelDatas;
    public static List<LevelData> AllLevels
    {
        get
        {
            if (_levelDatas == null)
            {
                _levelDatas = new List<LevelData>();
                foreach (ScriptableObject levelDataSO in HelperFunctions.GetScriptableObjects(typeof(LevelData)))
                    _levelDatas.Add(levelDataSO as LevelData);
            }
            return (_levelDatas);
        }
    }

    public static void LoadPreviousLevel()
    {
        //Debug.Log("Previous Level Callback");
        if (currentEditorLevel != null && Application.isPlaying == false)
        {
            Debug.Log("Returning To Previous LevelScene: " + currentEditorLevel.levelName);
            string scenePath = EditorManager.GetScenePathFromLevelData(currentEditorLevel);
            EditorManager.OpenScene(scenePath, OpenSceneMode.Single);
            cachedSceneName = string.Empty;
        }

    }

    public static void OnSceneLoad(Scene previousScene, Scene newScene)
    {
        currentEditorLevel = null;

        foreach (LevelData levelData in AllLevels)
            if (newScene.name == levelData.sceneName)
                ChangeCurrentLevel(levelData);
    }

    public static void ChangeCurrentLevel(LevelData levelData)
    {
        currentEditorLevel = levelData;

        if (currentEditorLevel == null)
            Debug.Log("Editor Scene Changed To: " + levelData.sceneName + " (Non-Level)");
        else
            Debug.Log("Editor Scene Changed To: " + levelData.sceneName + " (Level)");
    }


    [MenuItem("Editor Tools/Play Loaded Level", false, 1)]
    static void PlayLoadedLevel()
    {
        cachedSceneName = EditorSceneManager.GetActiveScene().path;
        EditorManager.OpenScene(scenesPath + "MainMenu.unity", OpenSceneMode.Single);
        GameManager.onInitalize += EditorManager.LoadLevel;
        EditorApplication.EnterPlaymode();
    }

    static void PlayCurrentLevel()
    {
        EditorManager.OpenScene(scenesPath + "MainMenu.unity", OpenSceneMode.Single);
        GameManager.onInitalize += EditorManager.LoadLevel;
        EditorApplication.EnterPlaymode();
    }

    [MenuItem("Editor Tools/Scenes/MainMenu", false, 1)]
    static void LoadLevelMainMenu()
    {
        EditorManager.OpenScene(scenesPath + "MainMenu.unity", OpenSceneMode.Single);
    }

    [MenuItem("Editor Tools/Scenes/01. Warehouse", false, 100)]
    static void LoadLevelWarehouse()
    {
        EditorManager.OpenScene(scenesPath + "Levels/WarehouseScene.unity", OpenSceneMode.Single);
    }

    [MenuItem("Editor Tools/Scenes/02. Suburbs", false, 102)]
    static void LoadLevelSuburbs()
    {
        EditorManager.OpenScene(scenesPath + "Levels/SuburbsScene.unity", OpenSceneMode.Single);
    }

    [MenuItem("Editor Tools/Scenes/03. The Mall", false, 104)]
    static void LoadLevelMall()
    {
        EditorManager.OpenScene(scenesPath + "Levels/MallScene.unity", OpenSceneMode.Single);
    }

    [MenuItem("Editor Tools/Scenes/04. Rooftops", false, 106)]
    static void LoadLevelRooftops()
    {
        EditorManager.OpenScene(scenesPath + "Levels/RooftopsScene.unity", OpenSceneMode.Single);
    }

    [MenuItem("Editor Tools/Scenes/05. Art Gallery", false, 108)]
    static void LoadLevelArtGallery()
    {
        EditorManager.OpenScene(scenesPath + "Levels/ArtGalleryScene.unity", OpenSceneMode.Single);
    }
}
