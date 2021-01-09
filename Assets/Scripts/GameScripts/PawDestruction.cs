using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawDestruction : MonoBehaviour
{
    [SerializeField] private float _timeToDestroy;
    [SerializeField] private int _soundInt;
    private SoundManager _soundManager;
    // Start is called before the first frame update
    void Start()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _soundManager._sounds[_soundInt].Play();
        Destroy(gameObject, _timeToDestroy);
    }
}
