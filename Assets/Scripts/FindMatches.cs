using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
