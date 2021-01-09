using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board _board;
    public Text scoreText;
    [SerializeField] private Text _scoreGoal;
    [SerializeField] private Text _winPanelScore;
    public int score;
    public Image scoreBar;
    private GameData _gameData;
    private int numberStars;

    // Start is called before the first frame update
    void Start()
    {
        _board = GameObject.FindWithTag("Board").GetComponent<Board>();
        _gameData = FindObjectOfType<GameData>();
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
        _scoreGoal.text = "Score goal: " + _board.scoreGoals[2];
    }
    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        /*for(int i = 0; i < _board.scoreGoals.Length; i++)
        {
            if(score > _board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }
        if (_gameData != null)
        {
            int highScore = _gameData._saveData.highScores[_board.level];
            if (score > highScore)
            {
                _gameData._saveData.highScores[_board.level] = score;
            }
            int currentStars = _gameData._saveData.stars[_board.level];
            if(numberStars > currentStars)
            {
                _gameData._saveData.stars[_board.level] = numberStars;
            }
            //_gameData.Save();
        }*/
        UpdateBar();
    }
    public void WinScoreUpdate()
    {
        for (int i = 0; i < _board.scoreGoals.Length; i++)
        {
            if (score > _board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }
        if (_gameData != null)
        {
            int highScore = _gameData._saveData.highScores[_board.level];
            if (score > highScore)
            {
                _gameData._saveData.highScores[_board.level] = score;
            }
            int currentStars = _gameData._saveData.stars[_board.level];
            if (numberStars > currentStars)
            {
                _gameData._saveData.stars[_board.level] = numberStars;
            }
            //_gameData.Save();
        }
    }
    private void UpdateBar()
    {
        if (_board != null && scoreBar != null)
        {
            int length = _board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)_board.scoreGoals[length - 1];
            _winPanelScore.text = "Score: " + score;
        }
    }
}
