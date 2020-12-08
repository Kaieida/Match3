using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private Board _board;
    public TextMeshProUGUI scoreText;
    public int score;
    public Image scoreBar;
    private GameData _gameData;

    // Start is called before the first frame update
    void Start()
    {
        _board = FindObjectOfType<Board>();
        _gameData = FindObjectOfType<GameData>();
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }
    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        if (_gameData != null)
        {
            int highScore = _gameData._saveData.highScores[_board.level];
            if (score > highScore)
            {
                _gameData._saveData.highScores[_board.level] = score;
            }
            _gameData.Save();
        }
        UpdateBar();
    }
    private void UpdateBar()
    {
        if (_board != null && scoreBar != null)
        {
            int length = _board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)_board.scoreGoals[length - 1];
        }
    }
}
