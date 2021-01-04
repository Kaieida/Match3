using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header ("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image _buttonImage;
    private Button _myButton;
    //private Text _levelText;
    private int _starsActive;

    [Header ("Level UI")]
    public Image[] stars;
    public Text levelText;
    public int level;
    public GameObject confirmPanel;

    [SerializeField] private AudioSource _audioSource;
    private GameData _gameData;
    // Start is called before the first frame update
    void Start()
    {
        _gameData = FindObjectOfType<GameData>();
        _buttonImage = GetComponent<Image>();
        _myButton = GetComponent<Button>();
        LoadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    private void LoadData()
    {
        if(_gameData != null)
        {
            if(_gameData._saveData.isActive[level - 1])
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }
            _starsActive = _gameData._saveData.stars[level - 1];
        }
    }

    private void ActivateStars()
    {
        for(int i = 0;i<_starsActive; i++)
        {

            stars[i].enabled = true;
        }
    }

    private void DecideSprite()
    {
        if (isActive)
        {
            _buttonImage.sprite = activeSprite;
            _myButton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            _buttonImage.sprite = lockedSprite;
            _myButton.enabled = false;
            levelText.enabled = false;
        }
    }
    private void ShowLevel()
    {
        levelText.text = "" + level;
    }
    public void ConfirmPanel(int level)
    {
        _audioSource.Play();
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
}
