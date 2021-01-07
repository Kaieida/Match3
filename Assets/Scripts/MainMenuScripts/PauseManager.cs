using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board _board;
    public bool pause = false;
    public Button soundButton;
    public Sprite musicOn;
    public Sprite musicOff;

    void Start()
    {
         pausePanel.SetActive(false);
         _board = GameObject.FindWithTag("Board").GetComponent<Board>();
        /* if (PlayerPrefs.HasKey("Sound"))
         {
             if(PlayerPrefs.GetInt("Sound") == 0)
             {
                 soundButton.image.sprite = musicOn;
                 PlayerPrefs.SetInt("Sound", 1);
             }
             else
             {
                 soundButton.image.sprite = musicOff;
                 PlayerPrefs.SetInt("Sound", 0);
             }
         }
         else
         {
             soundButton.image.sprite = musicOn;
             PlayerPrefs.SetInt("Sound", 1);
         }
     }
     public void SoundButton()
     {
         if (PlayerPrefs.HasKey("Sound"))
         {
             if (PlayerPrefs.GetInt("Sound") == 0)
             {
                 soundButton.image.sprite = musicOn;
                 PlayerPrefs.SetInt("Sound", 1);
             }
             else
             {
                 soundButton.image.sprite = musicOff;
                 PlayerPrefs.SetInt("Sound", 0);
             }
         }
         else
         {
             soundButton.image.sprite = musicOn;
             PlayerPrefs.SetInt("Sound", 1);
         }*/
     }
    public void PauseGame()
    {
        pause = !pause;
    }
    // Update is called once per frame
    void Update()
    {
        if (pause && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            _board.currentState = GameState.pause;
        }
        if (!pause && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            _board.currentState = GameState.move;
        }
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

