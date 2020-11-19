using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject tilePrefab;
    private BackgroundTiles[,] allTiles;
    public GameObject[,] AllDots;
    public GameObject[] Dots;
    // Start is called before the first frame update
    void Start()
    {
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
                Vector2 _tempPosition =new Vector2(i, h);
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
    }
}
