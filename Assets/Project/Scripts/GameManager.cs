using System.Collections;
using System.Collections.Generic;
using TriInspector;
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

    public enum GameState { Play, Pause }
    [PropertySpace]
    [Title("General")]

    [ReadOnly] public GameState gameState = GameState.Pause;

    [PropertySpace]
    [Title("Levels")]

    [ReadOnly] public LevelData currentLevel;
    public List<LevelData> allLevels = new List<LevelData>();

    [PropertySpace]
    [Title("Settings")]

    [InlineEditor] public PlayerSettings activePlayerSettings;
    [InlineEditor] public GameSettings activeGameSettings;
    [InlineEditor] public AudioSettings activeAudioSettings;
    [InlineEditor] public ControlSettings activeControlSettings;
    [InlineEditor] public LevelSettings activeLevelSettings;

    [PropertySpace]
    [Title("References")]

    [ReadOnly] public PlayerController playerController;
    [ReadOnly] public Volume playerControllerCamera;

    public delegate void Initialized();
    public static event Initialized onInitalize;

    public System.Action onFlip;

    [HideInInspector] public float currentTimerProgress;
    [HideInInspector] public float currentTimerLength;

    Coroutine coroutine;

    float startTime;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (LevelData levelData in allLevels)
            levelData.InitializeObjectives();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadLevel(LevelData levelData)
    {
        GameManager.Instance.currentLevel = levelData;
        SceneManager.LoadScene(currentLevel.sceneName);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (currentLevel != null && scene.name == currentLevel.sceneName)
        {
            playerController = Object.FindObjectOfType<PlayerController>();
            foreach (Volume volume in Object.FindObjectsOfType<Volume>())
                if (volume.profile.name.Contains("Pause"))
                    playerControllerCamera = volume;

            ChangeGameState(GameState.Play);
            OnLevelLoaded();
        }
    }

    public void OnLevelLoaded()
    {
        currentTimerProgress = 0f;
        currentTimerLength = 0f;
        startTime = Time.time;
        if (coroutine == null)
            coroutine = StartCoroutine(Timer(120f));

        foreach (ObjectiveData objective in currentLevel.currentLevelObjectives)
            objective.objectiveState = ObjectiveState.Uncomplete;

        onInitalize?.Invoke();
    }

    public void Update()
    {
        if (currentLevel != null)
        {
            if (Input.GetKeyDown(activeControlSettings.MenuControl.keyCode))
            {
                if (gameState == GameState.Pause)
                    ChangeGameState(GameState.Play);
                else
                    ChangeGameState(GameState.Pause);
            }

            if (Input.GetKeyDown(activeControlSettings.QuickRestartControl.keyCode))
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
                LoadLevel(currentLevel);
            }

            int objectivesCompleteCount = 0;
            foreach (ObjectiveData objectiveData in currentLevel.currentLevelObjectives)
                if (objectiveData.objectiveState == ObjectiveState.Complete)
                    objectivesCompleteCount++;

            MenuManager.Instance.objectiveCountText.text = objectivesCompleteCount + " / " + currentLevel.currentLevelObjectives.Count;

            currentTimerProgress = (currentTimerLength - (startTime - Time.time)) - currentTimerLength;
            MenuManager.Instance.timerTexts.text = currentTimerProgress.ToString("F2");

            if (playerController != null)
            {
                MenuManager.Instance.speedText.text = Mathf.RoundToInt(playerController.lateralSpeed) + "km/h";
                MenuManager.Instance.speedImage.fillAmount = playerController.lateralSpeed / playerController.maxPlayerSpeed;
            }
        }
    }

    public void Flip()
    {
        onFlip?.Invoke();
    }

    public void ChangeGameState(GameState newGameState)
    {
        gameState = newGameState;

        switch (gameState)
        {
            case GameState.Play:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                Time.timeScale = 1.0f;
                MenuManager.Instance.ToggleActiveMenu(MenuManager.Instance.gameplayPlayMenuParent.gameObject);
                MenuManager.Instance.gameplayLevelSelectInfoPopup.UnloadPopup();
                playerControllerCamera.weight = 0;
                break;
            case GameState.Pause:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                Time.timeScale = 0.0f;
                MenuManager.Instance.ToggleActiveMenu(MenuManager.Instance.gameplayPauseMenuParent.gameObject);
                MenuManager.Instance.gameplayLevelSelectInfoPopup.LoadPopup(currentLevel);
                playerControllerCamera.weight = 1;
                break;
        }
    }

    public IEnumerator Timer(float timerLength)
    {
        float startTime = Time.time;
        currentTimerLength = timerLength;
        currentTimerProgress = 0f;
        yield return new WaitForSeconds(timerLength);
        currentTimerProgress = 0f;
    }
}
