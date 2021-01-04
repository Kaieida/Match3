using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMain : MonoBehaviour
{
    public string sceneToLoad;
    private GameData _gameData;
    private GameObject _levelSelect;
    private Board _board;

    public void WinOK()
    {
        if(_gameData != null)
        {
            _gameData._saveData.isActive[_board.level+1] = true;
            _gameData.Save();
        }
        SceneManager.LoadScene(sceneToLoad);
    }
    public void LoseOK()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    // Start is called before the first frame update
    void Start()
    {
        _gameData = FindObjectOfType<GameData>();
        _board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }
}
