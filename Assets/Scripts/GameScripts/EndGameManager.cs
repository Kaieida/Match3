using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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
    public UnityEvent gameOverEvent;
    public GameObject test;
    public EndGameReq requirements;
    public GameObject movesLabel;
    public GameObject timesLabel;
    public GameObject youWinPanel;
    public GameObject youLosePanel;
    [SerializeField] private GameObject[] starArray = null;
    public Text counter;
    public int currentCounterValue;
    private float _timerSeconds;
    [SerializeField] private ScoreManager _scoreManager;
    private Board _board;
    private bool _adTime;

    // Start is called before the first frame update
    void Start()
    {
        _adTime = false; 
        _board = GameObject.FindWithTag("Board").GetComponent<Board>();
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
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        GameData _gameData = FindObjectOfType<GameData>();
        for(int i = 0; i < _gameData._saveData.stars[_board.level]; i++)
        {
            starArray[i].GetComponent<Image>().enabled = true;
        }
        _scoreManager.WinScoreUpdate();
        fade.GameOver();
        PlayerPrefs.SetInt("levelPanel", 1);
        if (!_adTime && TimeToAds.Instance.flag)
        {
            StartCoroutine(PlayAd());
        }
        
    }
    public void LoseGame()
    {
        youLosePanel.SetActive(true);
        _board.currentState = GameState.lose;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
        PlayerPrefs.SetInt("levelPanel", 1);
        if (!_adTime && TimeToAds.Instance.flag)
        {
            StartCoroutine(PlayAd());
        }
    }
    public IEnumerator PlayAd()
    {
        _adTime = true;
        yield return new WaitForSeconds(1f);
        gameOverEvent.Invoke();
        TimeToAds.Instance.StartTimer();
        yield return null;

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
