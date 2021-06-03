using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnim;
    public Animator gameInfoAnim;
    [SerializeField] private GameObject _soundManager;

    public void Ok()
    {
        if (panelAnim != null && gameInfoAnim != null)
        {
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            StartCoroutine(GameStart());
            Debug.Log("Kappa123");
        }
    }
    public void GameOver()
    {
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Game Over", true);
    }
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);
        Board _board = FindObjectOfType<Board>();
        _board.currentState = GameState.move;
    }
}
