using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameHandler : MonoBehaviour
{
    public event Action OnStateChanged;
    public event Action OnPause;
    public event Action OnUnPause;

    public static GameHandler Instance { get; private set; }

    private enum GameState
    {
        WaitingToStart,
        Stopped,
        CountDownToStart,
        GamePlaying,
        GameOver
    }

    private GameState state;
    private GameState previousState;

    [SerializeField] private float countDownToStartTimer = 3f;
    private float gamePlayingTimer;
    [SerializeField] private float gamePlayingTimerMax = 10f;
    private bool isPaused = false;

    

    private void Awake()
    {
        Instance = this;

        ChangeState(GameState.WaitingToStart);
    }

    private void Start()
    {
        InputHandler.Instance.OnInteractPerformed += InputHandler_OnInteractPerformed;
        InputHandler.Instance.OnPausePerformed += Instance_OnPause;

        ManagerUI.Instance.OnManagerUIShow += ManagerUI_OnManagerUIShow;
        ManagerUI.Instance.OnManagerUIHide += ManagerUI_OnManagerUIHide;
    }

    private void InputHandler_OnInteractPerformed()
    {
        if (state == GameState.WaitingToStart)
        {
            if (RecipeSelectionManager.Instance.HasRecipesAvaialable())
            {
                ChangeState(GameState.CountDownToStart);
            }
            else
            {
                ChangeState(GameState.Stopped);
            }
        }
    }


    private void ManagerUI_OnManagerUIHide()
    {
        if (previousState == GameState.WaitingToStart)
        {
            ChangeState(GameState.CountDownToStart);
        }
        else
        {
            ChangeState(GameState.GamePlaying);
        }
        
    }
    private void ManagerUI_OnManagerUIShow()
    {
        ChangeState(GameState.Stopped);
    }

    private void Instance_OnPause()
    {
        TogglePauseGame();
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.WaitingToStart:
              
                break;

            case GameState.CountDownToStart:
                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer <= 0)
                {
                    state = GameState.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke();
                }
                break;

            case GameState.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0)
                {
                    state = GameState.GameOver;

                    OnStateChanged?.Invoke();
                }
                break;

            case GameState.GameOver:
                break;
        }
    }


    public void TogglePauseGame()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0;
            OnPause?.Invoke();
        }
        else
        {
            Time.timeScale = 1;
            OnUnPause?.Invoke();
        }
    }


    private void ChangeState(GameState state)
    {
        if(this.state != state)
        {
            previousState = this.state;
            this.state = state;

            OnStateChanged?.Invoke();
        }
    }


    public bool IsGamePlaying()
    {
        return state == GameState.GamePlaying;
    }
    public bool IsCountingDownToStart()
    {
        return state == GameState.CountDownToStart;
    }
    public bool IsGameStopped()
    {
        return state == GameState.Stopped;
    }


    public float GetCountDownToStartTimer()
    {
        return countDownToStartTimer;
    }
    public bool IsGameOver()
    {
        return state == GameState.GameOver;
    }
    public float GetGamePlayingTimerNormalized()
    {
        return (gamePlayingTimer / gamePlayingTimerMax);
    }
    public bool IsWaitingToStart()
    {
        return state == GameState.WaitingToStart;
    }
}
