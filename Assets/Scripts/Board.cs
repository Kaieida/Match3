using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int Width;
    public int Height;
    public int OffSet;
    public GameObject tilePrefab;
    private BackgroundTiles[,] allTiles;
    public GameObject[,] AllDots;
    public GameObject[] Dots;
    public GameObject DestroyEffect;
    private FindMatches _findMatches;
    // Start is called before the first frame update
    void Start()
    {
        _findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTiles[Width, Height];
        AllDots = new GameObject[Width, Height];
        SetUp();
    }
    private void SetUp()
    {
        for(int i = 0; i < Width; i++)
        {
            for(int h = 0; h < Height; h++)
            {
                Vector2 _tempPosition =new Vector2(i, h + OffSet);
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
    // Update is called once per frame
    void Update()
    {
        
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (AllDots[column, row].GetComponent<Dot>().IsMatched)
        {
            _findMatches.CurrentMatches.Remove(AllDots[column, row]);
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
            for(int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] != null)
                {
                    DestroyMatchesAt(i, h);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for(int i = 0; i< Width; i++)
        {
            for(int h = 0; h < Height; h++)
            {
                if(AllDots[i,h] == null)
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
        for(int i = 0; i< Width; i++)
        {
            for(int h = 0; h < Height; h++)
            {
                if (AllDots[i,h] == null)
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
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.move;
    }
}
