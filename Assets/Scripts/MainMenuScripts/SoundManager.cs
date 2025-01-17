﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public bool pause = false;
    public AudioSource[] _sounds;
    public Button soundButton;
    public Sprite musicOn;
    public Sprite musicOff;

    private void Start()
    {
        bool sound = PlayerPrefs.GetString("Music", "On")=="On"?true:false;
        if (sound)
        {
            soundButton.image.sprite = musicOn;
            //PlayerPrefs.SetString("Sound", "Off");
        }
        else
        {
            soundButton.image.sprite = musicOff;
            //PlayerPrefs.SetString("Sound", "On");
        }
        MuteSound(!sound);
    }
    private void MuteSound(bool value)
    {
        if (value)
        {
            foreach (AudioSource sound in _sounds)
            {
                sound.mute = true;
            }
        }
        else
        {
            foreach (AudioSource sound in _sounds)
            {
                sound.mute = false;
            }
        }
    }
    public void SoundButtonClick()
    {
        Debug.Log(PlayerPrefs.GetString("Music"));
        bool sound = PlayerPrefs.GetString("Music", "On") == "On" ? true : false;
        Debug.Log(sound);
        if (sound)
        {
            soundButton.image.sprite = musicOff;
            PlayerPrefs.SetString("Music", "Off");
        }
        else
        {
            soundButton.image.sprite = musicOn;
            PlayerPrefs.SetString("Music", "On");
        }
        MuteSound(sound);
    }
}
