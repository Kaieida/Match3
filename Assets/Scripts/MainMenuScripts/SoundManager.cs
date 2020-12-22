using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //public AudioSource destroyNoise;
    public AudioSource[] _audioArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void PlayRandomDestroyNoise()
    {
        _audioArray[0].Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
