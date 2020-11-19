using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Varibales")]
    public int Column;
    public int Row;
    public int TargetX;
    public int TargetY;
    public int PreviousColumn;
    public int PreviousRow;
    public bool IsMatched = false;

    private GameObject _otherDot;
    private Board _board;
    private Vector2 _firstTouchPosition;
    private Vector2 _finalTouchPosition;
    private Vector2 temPosition;
    public float SwipeAngle = 0;
    public float SwipeResist = 1f;
    void Start()
    {
        _board = FindObjectOfType<Board>();
        TargetX = (int)transform.position.x;
        TargetY = (int)transform.position.y;
        Row = TargetY;
        Column = TargetX;
        PreviousRow = Row;
        PreviousColumn = Column;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if (IsMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, 0.2f);
        }
        TargetX = Column;
        TargetY = Row;
        if(Mathf.Abs(TargetX - transform.position.x) > 0.1f)
        {
            temPosition = new Vector2(TargetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, temPosition, 0.1f);
        }
        else
        {
            temPosition = new Vector2(TargetX, transform.position.y);
            transform.position = temPosition;
            _board.AllDots[Column, Row] = this.gameObject;
        }

        if (Mathf.Abs(TargetY - transform.position.y) > 0.1f)
        {
            temPosition = new Vector2(transform.position.x, TargetY);
            transform.position = Vector2.Lerp(transform.position, temPosition, 0.1f);
        }
        else
        {
            temPosition = new Vector2(transform.position.x, TargetY);
            transform.position = temPosition;
            _board.AllDots[Column, Row] = this.gameObject;
        }
    }
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(0.5f);
        if(_otherDot != null)
        {
            if (!IsMatched && !_otherDot.GetComponent<Dot>().IsMatched)
            {
                _otherDot.GetComponent<Dot>().Row = Row;
                _otherDot.GetComponent<Dot>().Column = Column;
                Row = PreviousRow;
                Column = PreviousColumn;
            }
            else
            {
                _board.DestroyMatches();
            }
            _otherDot = null;
        }

    }
    private void OnMouseDown()
    {
        _firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseUp()
    {
        _finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }
    private void CalculateAngle()
    {
        if (Mathf.Abs(_finalTouchPosition.y - _firstTouchPosition.y) > SwipeResist || Mathf.Abs(_finalTouchPosition.x - _firstTouchPosition.x) > SwipeResist )
        {
            SwipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y, _finalTouchPosition.x - _firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
        }
    }

    void MovePieces()
    {
        if(SwipeAngle > -45 && SwipeAngle <= 45 && Column < _board.Width-1)
        {
            _otherDot = _board.AllDots[Column + 1, Row];
            _otherDot.GetComponent<Dot>().Column -= 1;
            Column += 1;
        }
        else if (SwipeAngle > 45 && SwipeAngle <= 135 && Row < _board.Height-1)
        {
            _otherDot = _board.AllDots[Column, Row+1];
            _otherDot.GetComponent<Dot>().Row -= 1;
            Row += 1;
        }
        else if ((SwipeAngle > 135 || SwipeAngle <= -135) && Column > 0)
        {
            _otherDot = _board.AllDots[Column - 1, Row];
            _otherDot.GetComponent<Dot>().Column += 1;
            Column -= 1;
        }
        else if (SwipeAngle < -45 && SwipeAngle >= -135 && Row > 0)
        {
            _otherDot = _board.AllDots[Column, Row - 1];
            _otherDot.GetComponent<Dot>().Row += 1;
            Row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }
    void FindMatches()
    {
        if (Column > 0 && Column < _board.Width - 1)
        {
            GameObject leftDot1 = _board.AllDots[Column - 1, Row];
            GameObject rightDot1 = _board.AllDots[Column + 1, Row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().IsMatched = true;
                    rightDot1.GetComponent<Dot>().IsMatched = true;
                    IsMatched = true;
                }
            }
           
        }
        if (Row > 0 && Row < _board.Height - 1)
        {
            GameObject upDot1 = _board.AllDots[Column, Row + 1];
            GameObject downDot1 = _board.AllDots[Column, Row - 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().IsMatched = true;
                    downDot1.GetComponent<Dot>().IsMatched = true;
                    IsMatched = true;
                }
            }

        }
    }
}
