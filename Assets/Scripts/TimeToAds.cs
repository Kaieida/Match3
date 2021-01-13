using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToAds : MonoBehaviour
{
    private static TimeToAds _instance;

    public static TimeToAds Instance { get { return _instance; } }
    public bool flag = true;
    float time = 0;
    public void StartTimer()
    {
        StartCoroutine(Timer());
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public IEnumerator Timer()
    {
        flag = false;
        time = 90;
        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        flag = true;
    }
}
