using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private AudioSource _clickSound;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void LevelSelectMenu(GameObject levelPanel)
    {
        _clickSound.Play();
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
