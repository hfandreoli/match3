using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundController : MonoBehaviour
{
    [SerializeField] private int pointsForMatch3;
    [SerializeField] private int pointsForMatch4;
    [SerializeField] private int pointsForMatch5;
    [SerializeField] private int firstRoundGoal;
    [SerializeField] private int additionPerRound;
    [SerializeField] private float endRoundDelay;
    [SerializeField] private GameObject successScreen;
    [SerializeField] private GameObject failScreen;

    private SceneManager _sceneManager;
    private GridController _gridController;
    private Timer _timer;
    private RoundCounter _roundCounter;
    private SceneLoader _sceneLoader;

    private int _currentRound;
    private int _score;
    private int _roundGoal;
    private bool _gameEnded;
    private bool _success;


    private void Start()
    {
        _gridController = FindObjectOfType<GridController>();
        _timer = FindObjectOfType<Timer>();
        _roundCounter = FindObjectOfType<RoundCounter>();
        _currentRound = _roundCounter.GetRound();
        _sceneLoader = FindObjectOfType<SceneLoader>();
        
        _roundGoal = firstRoundGoal + additionPerRound * (_currentRound - 1);
    }

    public void ScoreMatch3()
    {
        _score += pointsForMatch3;
        UpdateScore();
    }

    public void ScoreMatch4()
    {
        _score += pointsForMatch4;
        UpdateScore();
    }

    public void ScoreMatch5()
    {
        _score += pointsForMatch5;
        UpdateScore();
    }

    public int GetScore()
    {
        return _score;
    }

    public int GetRoundGoal()
    {
        return _roundGoal;
    }

    public void TimesUp()
    {
        _gameEnded = true;
        
        if ( !_gridController.GetBusy() ) {
            StartCoroutine(EndRound());
        }
        else {
            _gridController.OnNotBusy += OnGridNotBusy;
        }
    }

    private void UpdateScore()
    {
        if ( _score >= _roundGoal && !_gameEnded) {
            _gameEnded = true;
            _success = true;
            
            if ( !_gridController.GetBusy() ) {
                StartCoroutine(EndRound());
            }
            else {
                _gridController.OnNotBusy += OnGridNotBusy;
            }
        }
    }

    private IEnumerator EndRound()
    {
        _gridController.SetBusy(true);
        _timer.Stop();
        yield return new WaitForSeconds(endRoundDelay);

        if ( _success ) {
            _roundCounter.NextRound();
            successScreen.SetActive(true);
        }
        else {
            failScreen.SetActive(true);
        }
    }

    private void OnGridNotBusy()
    {
        StartCoroutine(EndRound());
    }

    private void GoToNextRound()
    {
        if ( _success ) {
            _roundCounter.NextRound();
        }
        _sceneLoader.StartGame();
    }
}