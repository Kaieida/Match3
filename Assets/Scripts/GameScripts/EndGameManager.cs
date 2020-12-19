using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameType
{
    Moves,
    Time
}
[System.Serializable]
public class EndGameReq
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public EndGameReq requirements;
    public GameObject movesLabel;
    public GameObject timesLabel;
    public GameObject youWinPanel;
    public GameObject youLosePanel;
    public Text counter;
    public int currentCounterValue;
    private float _timerSeconds;
    private Board _board;
    // Start is called before the first frame update
    void Start()
    {
        _board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();
        
    }

    private void SetGameType()
    {
       if(_board.world != null)
        {
            if (_board.level < _board.world.levels.Length)
            {
                if (_board.world.levels[_board.level] != null)
                {
                    requirements = _board.world.levels[_board.level].endGameReq;
                }
            }
        }
    }

    private void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if(requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timesLabel.SetActive(false);
        }
        else
        {
            _timerSeconds = 1;
            movesLabel.SetActive(false);
            timesLabel.SetActive(true);
        }
        counter.text = ""+currentCounterValue;
    }
    public void DecreaseCounterValue()
    {
        if (_board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0 && _board.currentState != GameState.win)
            {
                LoseGame();
            }
        }   
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        _board.currentState = GameState.win;
        //currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }
    public void LoseGame()
    {
        youLosePanel.SetActive(true);
        _board.currentState = GameState.lose;
        Debug.Log("You suck!");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    private void Update()
    {
        if(requirements.gameType == GameType.Time)
        {
            _timerSeconds -= Time.deltaTime;
            if(_timerSeconds <= 0)
            {
                DecreaseCounterValue();
                _timerSeconds = 1;
            }
        }
    }
}
