using Racer.Utilities;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Game states available for transitioning
/// </summary>
public enum GameStates
{
    Loading,
    Playing,
    GameOver
}

/// <summary>
/// This manages the various states of the game
/// </summary>
class GameManager : SingletonPattern.StaticInstance<GameManager>
{

    public static event Action<GameStates> OnCurrentState;

    // For Visualization purpose
    [SerializeField]
    GameStates currentState;

    [Space(10)]

    [SerializeField]
    private float startDelay;

    public float GetStartDelay => startDelay;



    private void Start()
    {
        SetGameState(GameStates.Loading);

        StartCoroutine(LoadingCountDown());
    }

    /// <summary>
    /// Sets the current state of game
    /// </summary>
    /// <param name="state">Actual state to transition to</param>
    public void SetGameState(GameStates state)
    {
        currentState = state;

        // Updates other scripts listening to the game's current state
        OnCurrentState?.Invoke(state);
    }



    /// <summary>
    /// A little delay before the game starts
    /// </summary>
    IEnumerator LoadingCountDown()
    {
        yield return Utility.GetWaitForSeconds(startDelay);

        // Updates the game's state when the delay is over
        SetGameState(GameStates.Playing);
    }
}
