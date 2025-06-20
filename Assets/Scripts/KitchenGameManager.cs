using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KitchenGameManager : MonoBehaviour
{
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnPause;
    public event EventHandler OnStateChanged;
    public static KitchenGameManager Instance { get; private set; }

    private enum State
    {
        WaitingForStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 60f; //10 mins
    private bool isGamePause = false;

    private State state;

    private void Awake()
    {
        Instance = this;
        state = State.WaitingForStart;
        gamePlayingTimer = gamePlayingTimerMax;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state == State.WaitingForStart)
        {
            state = State.CountdownToStart;
            OnStateChanged.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingForStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;

                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;

                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                OnStateChanged?.Invoke(this, EventArgs.Empty);
                break;

        }

        // Debug.Log(state);
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsCountdownStartActive()
    {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public float GetGamePlayingtimerCountdown()
    {
        return gamePlayingTimer / gamePlayingTimerMax;
    }

    public void TogglePauseGame()
    {
        isGamePause = !isGamePause;
        if (!isGamePause)
        {
            OnGamePause.Invoke(this, EventArgs.Empty);
            Time.timeScale = 0f;
        }
        else
        {
            OnGameUnPause.Invoke(this, EventArgs.Empty);
            Time.timeScale = 1f;
        }
    }

    public bool IsGamePause()
    {
        return isGamePause;
    }
    
}
