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

    // Start is called before the first frame update
    void Start()
    {
        _board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }
    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        if (_board != null && scoreBar != null)
        {
            int length = _board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)_board.scoreGoals[length - 1];
        }
    }
}
