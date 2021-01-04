using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //public AudioSource destroyNoise;
    public AudioSource[] _audioArray;
    public void PlayRandomDestroyNoise()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                _audioArray[0].Play();
            }
        }
        else
        {
            _audioArray[0].Play();
        }
    }
}
