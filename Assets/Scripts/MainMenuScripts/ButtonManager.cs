using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private AudioSource _clickSound;
    public void LevelSelectMenu(GameObject levelPanel)
    {
        _clickSound.Play();
        //PlayerPrefs.SetInt("LevelSelect", 1);
        levelPanel.SetActive(true);
        
    }
    public void BackToStartMenu(GameObject levelPanel)
    {
        _clickSound.Play();
        levelPanel.SetActive(false);
    }
    public void ButtonChange(Sprite sprite)
    {
        gameObject.GetComponent<Image>().sprite = sprite;
    }
}
