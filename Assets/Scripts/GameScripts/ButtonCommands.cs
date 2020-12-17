using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCommands : MonoBehaviour
{
    [SerializeField] GameObject _levelSelectPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BackToStartMenu()
    {
        _levelSelectPanel.SetActive(false);
    }
}
