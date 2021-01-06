using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PanelSoundActivation : MonoBehaviour
{
    [SerializeField] private GameObject _soundManager;
    [SerializeField] private bool _win;
    // Start is called before the first frame update
    void Start()
    {
        _soundManager.GetComponent<SoundManager>()._sounds[3].Stop();
        if (_win)
        {
            _soundManager.GetComponent<SoundManager>()._sounds[2].Play();
        }
        else
        {
            _soundManager.GetComponent<SoundManager>()._sounds[1].Play();
        }
    }
}
