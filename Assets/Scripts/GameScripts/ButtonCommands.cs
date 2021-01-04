using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCommands : MonoBehaviour
{
    [SerializeField] GameObject _levelSelectPanel;
    public void BackToStartMenu()
    {
        _levelSelectPanel.SetActive(false);
    }
}
