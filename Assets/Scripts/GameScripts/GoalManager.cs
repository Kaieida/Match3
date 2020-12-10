using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;//change to 
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntParent;
    //public GameObject goalGameParent;//uncomment if broken
    private EndGameManager _endGame;
    private Board _board;
    // Start is called before the first frame update
    void Start()
    {
        _board = FindObjectOfType<Board>();
        GetGoals();
        SetupGoals();
        _endGame = FindObjectOfType<EndGameManager>();
        
    }
    private void GetGoals()
    {
        if(_board != null)
        {
            if (_board.level < _board.world.levels.Length)
            {
                if (_board.world != null)
                {
                    if (_board.world.levels[_board.level] != null)
                    {
                        levelGoals = _board.world.levels[_board.level].levelGoals;
                        for(int i = 0;i<levelGoals.Length; i++)
                        {
                            levelGoals[i].numberCollected = 0;
                        }
                    }
                }
            }
        }
    }
    private void SetupGoals()
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntParent.transform);

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/"+levelGoals[i].numberNeeded;


            //GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);//uncomment if broken
            //gameGoal.transform.SetParent(goalGameParent.transform);//uncomment if broken
            //panel = gameGoal.GetComponent<GoalPanel>();//uncomment if broken
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for(int i = 0; i<levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" +levelGoals[i].numberNeeded;
            if(levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            }
        }
        if(goalsCompleted >= levelGoals.Length)
        {
            if(_endGame != null)
            {
                _endGame.WinGame();
            }
            Debug.Log("you win!");
        }
    }
    public void CompareGoal(string goalToCompare)
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            if(goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}
