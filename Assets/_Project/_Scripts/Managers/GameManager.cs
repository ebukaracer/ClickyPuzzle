using Racer.Utilities;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This manages the various states of the game
/// </summary>
[DefaultExecutionOrder(-15)]
internal class GameManager : SingletonPattern.StaticInstance<GameManager>
{
    public static event Action<GameState> OnCurrentState;

    [SerializeField] private GameState currentState;
     
    [field: SerializeField] public float StartDelay { get; private set; }
    [field: SerializeField] public float EndDelay { get; private set; }


    private void Start()
    {
        SetGameState(GameState.Loading);
        StartCoroutine(LoadCountDown());
    }

    public void SetGameState(GameState state)
    {
        currentState = state;
        OnCurrentState?.Invoke(currentState);
    }

    /// <summary>
    /// Adds little delay before the game starts.
    /// </summary>
    private IEnumerator LoadCountDown()
    {
        yield return Utility.GetWaitForSeconds(StartDelay);

        SetGameState(GameState.Playing);
    }
}