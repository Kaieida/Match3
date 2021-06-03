using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level Information")]
    public string levelToLoad;
    public int level;
    private GameData _gameData;
    private int _highScore;

    [Header("UI stuff")]
    public Image[] stars;
    private int _starsActive;
    //public Text startText;
    public Text highScoreText;
    public Text cloudLevel;

    [SerializeField] private AudioSource _audioSource;

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
        if (_gameData != null)
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
        highScoreText.text = "HighScore: " + _highScore;
        //startText.text = "" + _starsActive + "/3";
        cloudLevel.text = "Level " + level;
    }
    // Update is called once per frame
    public void Cancel()
    {
        for(int i = 0; i< _starsActive; i++)
        {
            stars[i].enabled = false;
        }
        _audioSource.Play();
        this.gameObject.SetActive(false);
    }
    public void Play()
    {
        _audioSource.Play();
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(levelToLoad);
    }
}
