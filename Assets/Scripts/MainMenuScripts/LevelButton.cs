using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header ("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image _buttonImage;
    private Button _myButton;
    private TextMeshProUGUI _levelText;

    public Image[] stars;
    public TextMeshProUGUI levelText;
    public int level;
    public GameObject confirmPanel;
    // Start is called before the first frame update
    void Start()
    {
        _buttonImage = GetComponent<Image>();
        _myButton = GetComponent<Button>();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    private void ActivateStars()
    {
        for(int i = 0;i< stars.Length; i++)
        {
            stars[i].enabled = false;
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
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ConfirmPanel(int level)
    {
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
}
