using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board _board;
    public List<GameObject> CurrentMatches = new List<GameObject>();
    void Start()
    {
        _board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void FindingAllMatches()
    {
        StartCoroutine(FindAllMatches());
    }
    private IEnumerator FindAllMatches()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < _board.Width; i++)
        {
            for (int h = 0; h < _board.Height; h++)
            {
                GameObject currentDot = _board.AllDots[i, h];
                if (currentDot != null)
                {
                    if (i > 0 && i < _board.Width - 1)
                    {
                        GameObject leftDot = _board.AllDots[i - 1, h];
                        GameObject rightDot = _board.AllDots[i + 1, h];
                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().IsRowBomb || leftDot.GetComponent<Dot>().IsRowBomb || rightDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    CurrentMatches.Union(GetRowPieces(h));
                                }
                                if (currentDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    CurrentMatches.Union(GetColumnPieces(i));
                                }
                                if (leftDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    CurrentMatches.Union(GetColumnPieces(i - 1));
                                }
                                if (rightDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    CurrentMatches.Union(GetColumnPieces(i + 1));
                                }

                                if (!CurrentMatches.Contains(leftDot))
                                {
                                    CurrentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().IsMatched = true;
                                if (!CurrentMatches.Contains(rightDot))
                                {
                                    CurrentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().IsMatched = true;
                                if (!CurrentMatches.Contains(currentDot))
                                {
                                    CurrentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().IsMatched = true;
                            }
                        }
                    }
                    if (h > 0 && h < _board.Height - 1)
                    {
                        GameObject upDot = _board.AllDots[i, h + 1];
                        GameObject downDot = _board.AllDots[i, h - 1];
                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().IsColumnBomb || upDot.GetComponent<Dot>().IsColumnBomb || downDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    CurrentMatches.Union(GetColumnPieces(i));
                                }
                                if (currentDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    CurrentMatches.Union(GetRowPieces(h));
                                }
                                if (upDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    CurrentMatches.Union(GetRowPieces(h + 1));
                                }
                                if (downDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    CurrentMatches.Union(GetRowPieces(h - 1));
                                }
                                if (!CurrentMatches.Contains(upDot))
                                {
                                    CurrentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().IsMatched = true;
                                if (!CurrentMatches.Contains(downDot))
                                {
                                    CurrentMatches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().IsMatched = true;
                                if (!CurrentMatches.Contains(currentDot))
                                {
                                    CurrentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().IsMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < _board.Height; i++)
        {
            if (_board.AllDots[column, i] != null)
            {
                dots.Add(_board.AllDots[column, i]);
                _board.AllDots[column, i].GetComponent<Dot>().IsMatched = true;
            }
        }
        return dots;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < _board.Width; i++)
        {
            if (_board.AllDots[i, row] != null)
            {
                dots.Add(_board.AllDots[i, row]);
                _board.AllDots[i, row].GetComponent<Dot>().IsMatched = true;
            }
        }
        return dots;
    }

}
