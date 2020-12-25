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

    void Update()
    {

    }
    public void FindingAllMatches()
    {
        StartCoroutine(FindAllMatches());
    }
    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.IsAdjacentBomb)
        {
            CurrentMatches.Union(GetAdjacentPieces(dot1.Column, dot1.Row));
        }
        if (dot2.IsAdjacentBomb)
        {
            CurrentMatches.Union(GetAdjacentPieces(dot2.Column, dot2.Row));
        }
        if (dot3.IsAdjacentBomb)
        {
            CurrentMatches.Union(GetAdjacentPieces(dot3.Column, dot3.Row));
        }
        return currentDots;
    }
    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.IsRowBomb)
        {
            CurrentMatches.Union(GetRowPieces(dot1.Row));
            _board.BombRow(dot1.Row);
        }
        if (dot2.IsRowBomb)
        {
            CurrentMatches.Union(GetRowPieces(dot2.Row));
            _board.BombRow(dot2.Row);
        }
        if (dot3.IsRowBomb)
        {
            CurrentMatches.Union(GetRowPieces(dot3.Row));
            _board.BombRow(dot3.Row);
        }
        return currentDots;
    }
    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.IsColumnBomb)
        {
            CurrentMatches.Union(GetColumnPieces(dot1.Column));
            _board.BombColumn(dot1.Column);
        }
        if (dot2.IsColumnBomb)
        {
            CurrentMatches.Union(GetColumnPieces(dot2.Column));
            _board.BombColumn(dot2.Column);
        }
        if (dot3.IsColumnBomb)
        {
            CurrentMatches.Union(GetColumnPieces(dot3.Column));
            _board.BombColumn(dot3.Column);
        }
        return currentDots;
    }
    private void AddToListAndMatch(GameObject dot)
    {
        if (!CurrentMatches.Contains(dot))
        {
            CurrentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().IsMatched = true;
    }
    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
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
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    if (i > 0 && i < _board.Width - 1)
                    {
                        GameObject leftDot = _board.AllDots[i - 1, h];
                        GameObject rightDot = _board.AllDots[i + 1, h];
                        if (leftDot != null && rightDot != null)
                        {
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                CurrentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));
                                CurrentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));
                                CurrentMatches.Union(IsAdjacentBomb(leftDotDot, currentDotDot, rightDotDot));
                                GetNearbyPieces(leftDot, currentDot, rightDot);
                                //currentDot.GetComponent<Dot>().IsMatched = true;
                            }
                        }
                    }
                    if (h > 0 && h < _board.Height - 1)
                    {
                        GameObject upDot = _board.AllDots[i, h + 1];
                        GameObject downDot = _board.AllDots[i, h - 1];
                        if (upDot != null && downDot != null)
                        {
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                CurrentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));
                                CurrentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));
                                CurrentMatches.Union(IsAdjacentBomb(upDotDot, currentDotDot, downDotDot));
                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
        yield return null;
    }
    public void MatchColorPieces(string color)
    {
        for (int i = 0; i < _board.Width; i++)
        {
            for (int h = 0; h < _board.Height; h++)
            {
                if (_board.AllDots[i, h] != null)
                {
                    if (_board.AllDots[i, h].tag == color)
                    {
                        _board.AllDots[i, h].GetComponent<Dot>().IsMatched = true;
                    }
                }
            }
        }
    }
    private List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int h = row - 1; h <= row + 1; h++)
            {
                if (i >= 0 && i < _board.Width && h >= 0 && h < _board.Height)
                {
                    if (_board.AllDots[i, h] != null)
                    {
                        dots.Add(_board.AllDots[i, h]);
                        _board.AllDots[i, h].GetComponent<Dot>().IsMatched = true;
                    }
                }
            }
        }
        return dots;
    }
    private List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < _board.Height; i++)
        {
            if (_board.AllDots[column, i] != null)
            {
                Dot dot = _board.AllDots[column, i].GetComponent<Dot>();
                if (dot.IsRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }
                dots.Add(_board.AllDots[column, i]);
                dot.IsMatched = true;
            }
        }
        return dots;
    }
    private List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < _board.Width; i++)
        {
            if (_board.AllDots[i, row] != null)
            {
                Dot dot = _board.AllDots[i, row].GetComponent<Dot>();
                if (dot.IsColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(_board.AllDots[i, row]);
                dot.IsMatched = true;
            }
        }
        return dots;
    }
    public void CheckBombs(MatchType matchType)
    {
        if (_board.CurrentDot != null)
        {
            if (_board.CurrentDot.IsMatched && _board.CurrentDot.tag == matchType.color)
            {
                _board.CurrentDot.IsMatched = false;
                if ((_board.CurrentDot.SwipeAngle > -45 && _board.CurrentDot.SwipeAngle <= 45) || (_board.CurrentDot.SwipeAngle < -135 || _board.CurrentDot.SwipeAngle >= 135))
                {
                    _board.CurrentDot.MakeRowBomb();
                }
                else
                {
                    _board.CurrentDot.MakeColumnBomb();
                }
            }
            else if (_board.CurrentDot.OtherDot != null)
            {
                Dot otherDot = _board.CurrentDot.OtherDot.GetComponent<Dot>();
                if (otherDot.IsMatched && otherDot.tag == matchType.color)
                {
                    otherDot.IsMatched = false;
                    if ((_board.CurrentDot.SwipeAngle > -45 && _board.CurrentDot.SwipeAngle <= 45) || (_board.CurrentDot.SwipeAngle < -135 || _board.CurrentDot.SwipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}
