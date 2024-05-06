using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    public LevelData currentLevel;
    public List<LevelData> allLevels = new List<LevelData>();

    public enum GameState { Play, Pause }
    public GameState gameState;

    public PlayerController playerController;
    public Volume playerControllerCamera;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        foreach (LevelData levelData in allLevels)
            levelData.InitializeObjectives();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadLevel(LevelData levelData)
    {
        GameManager.Instance.currentLevel = MenuManager.Instance.currentSelectedLevel;
        SceneManager.LoadScene(MenuManager.Instance.currentSelectedLevel.sceneName);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        playerController = Object.FindObjectOfType<PlayerController>();
        foreach (Volume volume in Object.FindObjectsOfType<Volume>())
            if (volume.profile.name.Contains("Pause"))
                playerControllerCamera = volume;

        ChangeGameState(GameState.Play);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (gameState == GameState.Pause)
                ChangeGameState(GameState.Play);
            else
                ChangeGameState(GameState.Pause);
        }
    }

    public void ChangeGameState(GameState newGameState)
    {
        gameState = newGameState;

        switch (gameState)
        {
            case GameState.Play:
                Time.timeScale = 1.0f;
                MenuManager.Instance.ToggleActiveMenu(MenuManager.Instance.gameplayPlayMenuParent.gameObject);
                MenuManager.Instance.gameplayLevelSelectInfoPopup.UnloadPopup();
                playerControllerCamera.weight = 0;
                break;
            case GameState.Pause:
                Time.timeScale = 0.0f;
                MenuManager.Instance.ToggleActiveMenu(MenuManager.Instance.gameplayPauseMenuParent.gameObject);
                MenuManager.Instance.gameplayLevelSelectInfoPopup.LoadPopup(currentLevel);
                playerControllerCamera.weight = 1;
                break;
        }
    }
}
