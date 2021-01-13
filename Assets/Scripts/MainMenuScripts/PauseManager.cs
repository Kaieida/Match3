using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField] private Transform _pauseTransform;
    [SerializeField] private Transform _bolvan4ik;
    private Board _board;
    public bool pause = false;
    public Button soundButton;
    public Sprite musicOn;
    public Sprite musicOff;

    void Start()
    {
         pausePanel.SetActive(false);
         _board = GameObject.FindWithTag("Board").GetComponent<Board>();
        _pauseTransform.DOMove(_bolvan4ik.position, 2f);

     }
    public void PauseGame()
    {
        pause = !pause;
    }
    // Update is called once per frame
    void Update()
    {
        if (pause && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            _board.currentState = GameState.pause;
        }
        if (!pause && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            _board.currentState = GameState.move;
        }
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

