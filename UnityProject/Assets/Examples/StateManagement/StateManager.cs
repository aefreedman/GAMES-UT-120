using System;
using System.Collections;
using UnityEngine;

namespace Examples.StateManagement
{
    public class StateManager : MonoBehaviour
    {
        private enum State
        {
            Init,
            Playing,
            GameOver
        }

        private State _state;
        private int _score;
        private int _maxScore = 3;

        private Coroutine _gameOverTimer;

        // Unity Event Function, called automatically
        private void Start()
        {
            _state = State.Init;
        }

        // Unity Event Callback, happens every frame
        private void Update()
        {
            switch (_state)
            {
                case State.Init:
                    // Do something & enter play state
                    _state = State.Playing;
                    break;
                case State.Playing:
                    PlayState();
                    if (IsGameOver())
                        GoToGameOver();
                    break;
                case State.GameOver:
                    GameOverState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PlayState()
        {
            // do stuff
        }

        private bool IsGameOver()
        {
            // Check some condition that ends the game, like score
            return _score >= _maxScore;
        }

        private void GoToGameOver()
        {
            _state = State.GameOver;
            
            // Do something, like show a win screen for some number of seconds
            // winScreen.SetActive(true);
            _gameOverTimer = StartCoroutine(GameOverTimer(3f));
        }

        private void GameOverState()
        {
            // The game over timer coroutine is set when exiting the play state
            // if it's null when we're in the game over state, the timer is over
            if (_gameOverTimer == null)
                ResetGame();
        }

        private void ResetGame()
        {
            // Do something
        }

        private IEnumerator GameOverTimer(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _gameOverTimer = null;
        }
    }
}