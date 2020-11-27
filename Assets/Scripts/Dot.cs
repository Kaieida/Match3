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

    private HintManager _hintManager;
    private FindMatches _findMatches;
    public GameObject OtherDot;
    private Board _board;
    private Vector2 _firstTouchPosition;
    private Vector2 _finalTouchPosition;
    private Vector2 temPosition;

    [Header("Swipe Variables")]
    public float SwipeAngle = 0;
    public float SwipeResist = 1f;

    [Header("PowerUp Variables")]
    public bool IsColorBomb;
    public bool IsColumnBomb;
    public bool IsRowBomb;
    public bool IsAdjacentBomb;
    public GameObject AdjacetMarker;
    public GameObject RowArrow;
    public GameObject ColumnArrow;
    public GameObject ColorBomb;
    void Start()
    {
        IsColumnBomb = false;
        IsRowBomb = false;
        IsColorBomb = false;
        _hintManager = FindObjectOfType<HintManager>();
        _board = FindObjectOfType<Board>();
        _findMatches = FindObjectOfType<FindMatches>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            IsAdjacentBomb = true;
            GameObject marker = Instantiate(AdjacetMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        TargetX = Column;
        TargetY = Row;
        if(Mathf.Abs(TargetX - transform.position.x) > 0.1f)
        {
            temPosition = new Vector2(TargetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, temPosition, 0.1f);
            if(_board.AllDots[Column,Row] != this.gameObject)
            {
                _board.AllDots[Column, Row] = this.gameObject;
            }
            _findMatches.FindingAllMatches();
        }
        else
        {
            temPosition = new Vector2(TargetX, transform.position.y);
            transform.position = temPosition;
            
        }

        if (Mathf.Abs(TargetY - transform.position.y) > 0.1f)
        {
            temPosition = new Vector2(transform.position.x, TargetY);
            transform.position = Vector2.Lerp(transform.position, temPosition, 0.1f);
            if (_board.AllDots[Column, Row] != this.gameObject)
            {
                _board.AllDots[Column, Row] = this.gameObject;
            }
            _findMatches.FindingAllMatches();
        }
        else
        {
            temPosition = new Vector2(transform.position.x, TargetY);
            transform.position = temPosition;
        }
    }
    public IEnumerator CheckMove()
    {
        if (IsColorBomb)
        {
            _findMatches.MatchColorPieces(OtherDot.tag);
            IsMatched = true;
        }
        else if (OtherDot.GetComponent<Dot>().IsColorBomb)
        {
            _findMatches.MatchColorPieces(this.gameObject.tag);
            OtherDot.GetComponent<Dot>().IsMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if(OtherDot != null)
        {
            if (!IsMatched && !OtherDot.GetComponent<Dot>().IsMatched)
            {
                OtherDot.GetComponent<Dot>().Row = Row;
                OtherDot.GetComponent<Dot>().Column = Column;
                Row = PreviousRow;
                Column = PreviousColumn;
                yield return new WaitForSeconds(0.5f);
                _board.CurrentDot = null;
                _board.currentState = GameState.move;
            }
            else
            {
                _board.DestroyMatches();

            }
            //_otherDot = null;
        }

    }
    private void OnMouseDown()
    {
        if(_hintManager != null)
        {
            _hintManager.DestroyHint();
        }
        if (_board.currentState == GameState.move)
        {
            _firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        

    }
    private void OnMouseUp()
    {
        if (_board.currentState == GameState.move)
        {
            _finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
        
    }
    private void CalculateAngle()
    {
        if (Mathf.Abs(_finalTouchPosition.y - _firstTouchPosition.y) > SwipeResist || Mathf.Abs(_finalTouchPosition.x - _firstTouchPosition.x) > SwipeResist )
        {
            _board.currentState = GameState.wait;
            SwipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y, _finalTouchPosition.x - _firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            //_board.currentState = GameState.wait;
            _board.CurrentDot = this;
        }
        else
        {
            _board.currentState = GameState.move;
        }
    }

    private void ActualMovePieces(Vector2 direction)
    {
        OtherDot = _board.AllDots[Column + (int)direction.x, Row + (int)direction.y];
        PreviousRow = Row;
        PreviousColumn = Column;
        if (OtherDot != null)
        {
            OtherDot.GetComponent<Dot>().Column += -1 * (int)direction.x;
            OtherDot.GetComponent<Dot>().Row += -1 * (int)direction.y;
            Column += (int)direction.x;
            Row += (int)direction.y;
            StartCoroutine(CheckMove());
        }
        else
        {
            _board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        if (SwipeAngle > -45 && SwipeAngle <= 45 && Column < _board.Width - 1)
        {
            ActualMovePieces(Vector2.right);
        }
        else if (SwipeAngle > 45 && SwipeAngle <= 135 && Row < _board.Height - 1)
        {
            ActualMovePieces(Vector2.up);
        }
        else if ((SwipeAngle > 135 || SwipeAngle <= -135) && Column > 0)
        {
            ActualMovePieces(Vector2.left);
        }
        else if (SwipeAngle < -45 && SwipeAngle >= -135 && Row > 0)
        {
            ActualMovePieces(Vector2.down);
        }
        else
        {
            _board.currentState = GameState.move;
        }
    }
    private void FindMatches()
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
    public void MakeRowBomb()
    {
        IsRowBomb = true;
        GameObject arrow = Instantiate(RowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
    public void MakeColumnBomb()
    {
        IsColumnBomb = true;
        GameObject arrow = Instantiate(ColumnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
    public void MakeColorBomb()
    {
        IsColorBomb = true;
        GameObject color = Instantiate(ColorBomb, transform.position, Quaternion.identity);
        color.transform.parent = this.transform;
        this.gameObject.tag = "Color";
    }
    public void MakeAdjacentBomb()
    {
        IsAdjacentBomb = true;
        GameObject marker = Instantiate(AdjacetMarker, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }
}
