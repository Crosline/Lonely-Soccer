using _Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    [SerializeField]
    private GameObject _gameCanvas;
    [SerializeField]
    private Text _levelText;
    [SerializeField]
    private GameObject _winCanvas;
    [SerializeField]
    private GameObject _loseCanvas;

    [SerializeField]
    private Ball _ball;

    [SerializeField]
    private LookAt _mainCamera;

    public GameState State { get; private set; }

    [SerializeField]
    private InputHandler _inputHandler;

    [SerializeField]
    private Projection _projection;

    [SerializeField]
    private int _level = 0;
    public List<GameObject> levels = new List<GameObject>();

    private GameObject _currentLevel;

    void Start() {

        if (_gameCanvas == null)
            _gameCanvas = GameObject.Find("GameCanvas");

        if (_levelText == null)
            _levelText = GameObject.Find("LevelText").GetComponent<Text>();

        if (_winCanvas == null)
            _winCanvas = GameObject.Find("WinCanvas");

        if (_loseCanvas == null)
            _loseCanvas = GameObject.Find("LoseCanvas");

        if (_ball == null)
            _ball = GameObject.Find("Ball").GetComponent<Ball>();

        if (_inputHandler == null)
            _inputHandler = _ball.GetComponent<InputHandler>();

        if (_projection == null)
            _projection = _ball.transform.GetChild(1).GetComponent<Projection>();

        if (_mainCamera == null)
        _mainCamera = Camera.main.GetComponent<LookAt>();


        ChangeState(GameState.Starting);
    }

    void FixedUpdate() {
        if (State != GameState.WaitingRun) return;

        _projection.GetTrajectoryLine();
    }

    public void ChangeState(GameState newState) {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState) {
            case GameState.Starting:
                Init();
                break;
            case GameState.WaitingInput:
                WaitingInput();
                break;
            case GameState.WaitingRun:
                break;
            case GameState.Running:
                Running();
                break;
            case GameState.Win:
                Win();
                break;
            case GameState.Lose:
                Lose();
                break;
            case GameState.Restart:
                Restart();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);

        Debug.Log($"New State: {newState}");
    }
    #region State Functions

    private void Init() {

        ResetCanvases();

        _mainCamera.StopFollow();
        _projection.isGoalFound = false;

        if (_currentLevel != null)
            Destroy(_currentLevel);

        var environment = GameObject.Find("Environment");
        if (environment != null)
            _currentLevel = Instantiate(levels[_level], environment.transform);
        else
            _currentLevel = Instantiate(levels[_level]);

        ChangeState(GameState.WaitingInput);
    }

    private void WaitingInput() {

        _inputHandler.SetActive(true);

        _inputHandler.ResetTransform();

    }

    private void Running() {


        _projection.RemoveLine();
        _inputHandler.SetActive(false);


        if (!_projection.isGoalFound) {
            ChangeState(GameState.WaitingInput);
            return;
        }

        List<Vector3> pos = _projection.hitPoints;

        if (pos.Count == 0) {
            ChangeState(GameState.WaitingInput);
            return;
        } else {
            _ball.Init(pos);
            _mainCamera.StartFollow();
        }
    }
    private void Restart() {
        ResetCanvases();


        ChangeState(GameState.Starting);
    }

    private void Win() {
        _gameCanvas.SetActive(false);
        _winCanvas.SetActive(true);
    }

    private void Lose() {
        _gameCanvas.SetActive(false);
        _loseCanvas.SetActive(true);
    }

    #endregion

    private void ResetCanvases() {
        _winCanvas.SetActive(false);
        _loseCanvas.SetActive(false);

        _gameCanvas.SetActive(true);
        _levelText.text = $"Level {_level + 1}";
    }

    #region Button Functions

    public void RestartGame() {

        ChangeState(GameState.Restart);
    }



    public void NextGame() {
        _level++;

        if (_level == levels.Count) {
            _level = 0;
        }

        ChangeState(GameState.Restart);
    }

    #endregion

    [Serializable]
    public enum GameState {
        Starting = 0,
        WaitingInput = 1,
        Running = 2,
        WaitingRun = 3,
        Win = 4,
        Lose = 5,
        Restart = 6,
    }
}
