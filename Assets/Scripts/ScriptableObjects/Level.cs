using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimensions")]
    public int width;
    public int height;

    [Header("Starting tiles")]
    public TileType[] boardLayout;

    [Header("Available dots")]
    public GameObject[] dots;

    [Header("Score goals")]
    public int[] scoreGoals;

    [Header("End game requirements")]
    public EndGameReq endGameReq;
    public BlankGoal[] levelGoals;

    [Header("Camera pading")]
    public float pading = 3.5f;
}
