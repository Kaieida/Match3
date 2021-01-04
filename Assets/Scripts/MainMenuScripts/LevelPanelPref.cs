using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPanelPref : MonoBehaviour
{
    [SerializeField] private GameObject _levelPanel;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("levelPanel") == 1)
        {
            _levelPanel.SetActive(true);
        }
    }
}
