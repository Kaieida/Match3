﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal

}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int Width;
    public int Height;
    public int OffSet;
    public GameObject tilePrefab;
    private bool[,] blankSpaces;
    public GameObject[,] AllDots;
    public TileType[] BoardLayout;
    public Dot CurrentDot;
    public GameObject[] Dots;
    public GameObject DestroyEffect;
    private FindMatches _findMatches;
    // Start is called before the first frame update
    void Start()
    {
        _findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[Width, Height];
        AllDots = new GameObject[Width, Height];
        SetUp();
    }
    private void GenerateBlankSpaces()
    {
        for (int i = 0; i < BoardLayout.Length; i++)
        {
            if (BoardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[BoardLayout[i].x, BoardLayout[i].y] = true;
            }
        }
    }

    private void SetUp()
    {
        GenerateBlankSpaces();
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (!blankSpaces[i, h])
                {


                    Vector2 _tempPosition = new Vector2(i, h + OffSet);
                    GameObject backgroundTile = Instantiate(tilePrefab, _tempPosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + h + " )";
                    int dotToUse = Random.Range(0, Dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, h, Dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, Dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(Dots[dotToUse], _tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().Row = h;
                    dot.GetComponent<Dot>().Column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + h + " )";
                    AllDots[i, h] = dot;
                }
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)
            {
                if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
            {
                if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
                {
                    if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

            if (column > 1)
            {
                if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)
                {
                    if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = _findMatches.CurrentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in _findMatches.CurrentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.Row == firstPiece.Row)
                {
                    numberHorizontal++;
                }
                if (dot.Column == firstPiece.Column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
    }

    private void CheckToMakeBombs()
    {
        if (_findMatches.CurrentMatches.Count == 4 || _findMatches.CurrentMatches.Count == 7)
        {
            _findMatches.CheckBombs();
        }
        if (_findMatches.CurrentMatches.Count == 5 || _findMatches.CurrentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                if (CurrentDot != null)
                {
                    if (CurrentDot.IsMatched)
                    {
                        if (!CurrentDot.IsColorBomb)
                        {
                            CurrentDot.IsMatched = false;
                            CurrentDot.MakeColorBomb();
                        }
                        else
                        {
                            if (CurrentDot.OtherDot != null)
                            {
                                Dot otherDot = CurrentDot.OtherDot.GetComponent<Dot>();
                                if (otherDot.IsMatched)
                                {
                                    if (!otherDot.IsColorBomb)
                                    {
                                        otherDot.IsMatched = false;
                                        otherDot.MakeColorBomb();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (CurrentDot != null)
                {
                    if (CurrentDot.IsMatched)
                    {
                        if (!CurrentDot.IsAdjacentBomb)
                        {
                            CurrentDot.IsMatched = false;
                            CurrentDot.MakeAdjacentBomb();
                        }
                        else
                        {
                            if (CurrentDot.OtherDot != null)
                            {
                                Dot otherDot = CurrentDot.OtherDot.GetComponent<Dot>();
                                if (otherDot.IsMatched)
                                {
                                    if (!otherDot.IsAdjacentBomb)
                                    {
                                        otherDot.IsMatched = false;
                                        otherDot.MakeAdjacentBomb();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (AllDots[column, row].GetComponent<Dot>().IsMatched)
        {
            if (_findMatches.CurrentMatches.Count >= 4)
            {
                CheckToMakeBombs();
                //_findMatches.CheckBombs();
            }
            //_findMatches.CurrentMatches.Remove(AllDots[column, row]);
            GameObject particle = Instantiate(DestroyEffect, AllDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(AllDots[column, row]);
            AllDots[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] != null)
                {
                    DestroyMatchesAt(i, h);
                }
            }
        }
        _findMatches.CurrentMatches.Clear();
        StartCoroutine(DecreaseRowTwo());
    }

    private IEnumerator DecreaseRowTwo()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (!blankSpaces[i, h] && AllDots[i, h] == null)
                {
                    for(int k = h + 1; k < Height; k++)
                    {
                        if (AllDots[i, k] != null)
                        {
                            AllDots[i, k].GetComponent<Dot>().Row = h;
                            AllDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoard());
    }

    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    AllDots[i, h].GetComponent<Dot>().Row -= nullCount;
                    AllDots[i, h] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoard());
    }
    private void RefillBoard()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] == null && !blankSpaces[i,h])
                {
                    Vector2 tempPosition = new Vector2(i, h + OffSet);
                    int dotToUse = Random.Range(0, Dots.Length);
                    GameObject piece = Instantiate(Dots[dotToUse], tempPosition, Quaternion.identity);
                    AllDots[i, h] = piece;
                    piece.GetComponent<Dot>().Row = h;
                    piece.GetComponent<Dot>().Column = i;
                }
            }
        }
    }
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] != null)
                {
                    if (AllDots[i, h].GetComponent<Dot>().IsMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        _findMatches.CurrentMatches.Clear();
        CurrentDot = null;
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.move;
    }
}
