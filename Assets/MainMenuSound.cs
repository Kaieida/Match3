using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSound : MonoBehaviour
{
    //public GameObject pausePanel;
    //private Board _board;
    public bool pause = false;
    [SerializeField] private AudioSource[] _sounds;
    public Button soundButton;
    public Sprite musicOn;
    public Sprite musicOff;

    private void Start()
    {
        if(PlayerPrefs.GetString("Sound") == "Off")
        {
            soundButton.image.sprite = musicOn;
            PlayerPrefs.SetString("Sound", "Off");
        }
        else
        {
            soundButton.image.sprite = musicOff;
            PlayerPrefs.SetString("Sound", "On");
        }
        MuteSound();
    }
    public void MuteSound()
    {
        if (PlayerPrefs.GetString("Sound") == "On")
        {
            soundButton.image.sprite = musicOff;
            foreach(AudioSource sound in _sounds)
            {
                sound.mute = true;
            }
            PlayerPrefs.SetString("Sound", "Off");
        }
        else
        {
            soundButton.image.sprite = musicOn;
            foreach (AudioSource sound in _sounds)
            {
                sound.mute = false;
            }
            PlayerPrefs.SetString("Sound", "On");
        }
    }
}
