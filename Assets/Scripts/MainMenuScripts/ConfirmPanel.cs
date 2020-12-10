﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ConfirmPanel : MonoBehaviour
{
    [Header ("Level Information")]
    public string levelToLoad;
public int level;
 private GameData _gameData;
    private int _highScore;

    [Header ("UI stuff")]
    public Image[] stars;
    private int _starsActive;
    public TextMeshProUGUI startText;
    public TextMeshProUGUI highScoreText;
   


    // Start is called before the first frame update
    void OnEnable()
    {
        _gameData = FindObjectOfType<GameData>();
        LoadData();
        ActivateStars();
        SetText();
    }
    private void LoadData()
    {
        if(_gameData != null)
        {
            _starsActive = _gameData._saveData.stars[level - 1];
            _highScore = _gameData._saveData.highScores[level - 1];
        }
    }
    private void ActivateStars()
    {
        for (int i = 0; i < _starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }
    private void SetText()
    {
        highScoreText.text = "" + _highScore;
        startText.text = "" + _starsActive + "/3";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }
    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", level-1);
        SceneManager.LoadScene(levelToLoad);
    }
}