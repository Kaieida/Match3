using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;
using UnityEngine.Events;

public class SwipeEvent : UnityEvent<Vector2>
{

}

public class Dot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private static SwipeEvent _swipeEvent = new SwipeEvent();
    [Header("Board Varibales")]
    public int Column;
    public int Row;
    public int TargetX;
    public int TargetY;
    public int PreviousColumn;
    public int PreviousRow;
    public bool IsMatched = false;

    private EndGameManager _endGameManager;
    private HintManager _hintManager;
    private FindMatches _findMatches;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    public GameObject OtherDot;
    private Board _board;
    private Vector2 _firstTouchPosition = Vector2.zero;
    private Vector2 _finalTouchPosition = Vector2.zero;
    private Vector2 temPosition;

    [Header("Swipe Variables")]
    public float SwipeAngle = 0;
    public float SwipeResist = 1f;

    [Header("PowerUp Variables")]
    public bool IsColorBomb;
    public bool IsColumnBomb;
    public bool IsRowBomb;
    public bool IsAdjacentBomb;
    public Sprite adjacentSprite;
    public Sprite rowSprite;
    public Sprite columnSprite;
    public Sprite colorSprite;
    public bool flag;
    void Start()
    {
        IsColumnBomb = false;
        IsRowBomb = false;
        IsColorBomb = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _endGameManager = FindObjectOfType<EndGameManager>();
        _hintManager = FindObjectOfType<HintManager>();
        _board = GameObject.FindWithTag("Board").GetComponent<Board>();
        _findMatches = FindObjectOfType<FindMatches>();
    }
    private void LateUpdate()
    {
        if(flag)
        _spriteRenderer.sprite = colorSprite;
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            IsAdjacentBomb = true;
            //GameObject marker = Instantiate(AdjacetMarker, transform.position, Quaternion.identity);
            //marker.transform.parent = this.transform;
        }
    }
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
                _findMatches.FindingAllMatches();
            }
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
                _findMatches.FindingAllMatches();
            }
        }
        else
        {
            temPosition = new Vector2(transform.position.x, TargetY);
            transform.position = temPosition;
        }
    }
    public IEnumerator test()
    {
        yield return null;
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
                if(_endGameManager != null)
                {
                    if(_endGameManager.requirements.gameType == GameType.Moves)
                    {
                        _endGameManager.DecreaseCounterValue();
                    }
                }
                _board.DestroyMatches();

            }
        }
        //LeanTouch.OnFingerSwipe -= CalculateAngle;
    }
    private void CalculateAngle()//private void CalculateAngle(Vector2 f)
    {
        /*if (Mathf.Abs(f.x) > Mathf.Abs(f.y))
        {
            if (f.x > 0)
            {
                ActualMovePieces(Vector2.right);
            }
            else
            {
                ActualMovePieces(Vector2.left);
            }
        }
        else
        {
            if (f.y > 0)
            {
                ActualMovePieces(Vector2.up);
            }
            else
            {
                ActualMovePieces(Vector2.down);
            }
        }*/
        if (Mathf.Abs(_finalTouchPosition.y - _firstTouchPosition.y) > SwipeResist || Mathf.Abs(_finalTouchPosition.x - _firstTouchPosition.x) > SwipeResist )
        {
            _board.currentState = GameState.wait;
            SwipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y, _finalTouchPosition.x - _firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
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
        if (_board.lockTiles[Column, Row] == null && _board.lockTiles[Column + (int)direction.x, Row + (int)direction.y] == null)
        {
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
    public void StartAnimation()
    {
        StartCoroutine(StartsAnimation());
    }
    public IEnumerator StartsAnimation()
    {
        //_animator.SetBool("Destruction", true);
        _animator.enabled = true;
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
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
        if (!IsColumnBomb && !IsColorBomb && !IsAdjacentBomb)
        {
            IsRowBomb = true;
            Debug.Log("Making bomb");
            /*GameObject arrow = Instantiate(RowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;*/
            _spriteRenderer.sprite = rowSprite;
        }
    }
    public void MakeColumnBomb()
    {
        if (!IsRowBomb && !IsColorBomb && !IsAdjacentBomb)
        {
            IsColumnBomb = true;
            Debug.Log("Making bomb");
            /*GameObject arrow = Instantiate(ColumnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;*/
            _spriteRenderer.sprite = columnSprite;
        }
    }
    public void MakeColorBomb()
    {
        if (!IsColumnBomb && !IsRowBomb && !IsAdjacentBomb)
        {
            IsColorBomb = true;
            /*GameObject color = Instantiate(ColorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;*/
            Debug.Log("Making bomb");
            _spriteRenderer.sprite = colorSprite;
            this.gameObject.tag = "Color";
        }
    }
    public void MakeAdjacentBomb()
    {
        if (!IsColumnBomb && !IsColorBomb && !IsRowBomb)
        {
            IsAdjacentBomb = true;
            Debug.Log("Making bomb");
            /*GameObject marker = Instantiate(AdjacetMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;*/
            _spriteRenderer.sprite = adjacentSprite;
            
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /*_swipeEvent.RemoveAllListeners();
        _swipeEvent.AddListener(CalculateAngle);*/
        
        if (_hintManager != null)
        {
            _hintManager.DestroyHint();
        }

        if (_board.currentState == GameState.move)
        {
            _firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //LeanTouch.OnFingerSwipe -= CalculateAngle;
        if (_board.currentState == GameState.move)
        {
            _finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    public static void Swipe(LeanFinger finger)
    {
        Vector2 f = finger.SwipeScreenDelta.normalized;
        _swipeEvent.Invoke(f);
        
    }
}
