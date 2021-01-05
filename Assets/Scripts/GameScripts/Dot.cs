using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;
using UnityEngine.Events;

public class Dot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
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
    //public bool flag;
    void Start()
    {
        IsColumnBomb = false;
        IsRowBomb = false;
        IsColorBomb = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _endGameManager = GameObject.Find("EndGameManager").GetComponent<EndGameManager>();
        _hintManager = GameObject.FindWithTag("Board").GetComponent<HintManager>();
        _board = GameObject.FindWithTag("Board").GetComponent<Board>();
        _findMatches = GameObject.Find("Match Finder").GetComponent<FindMatches>();
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
    }
    private void CalculateAngle()
    {
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
        _animator.enabled = true;
        yield return new WaitForSeconds(0.35f);
        Destroy(gameObject);
    }
    public void MakeRowBomb()
    {
        if (!IsColumnBomb && !IsColorBomb && !IsAdjacentBomb)
        {
            IsRowBomb = true;
            _spriteRenderer.sprite = rowSprite;
        }
    }
    public void MakeColumnBomb()
    {
        if (!IsRowBomb && !IsColorBomb && !IsAdjacentBomb)
        {
            IsColumnBomb = true;
            _spriteRenderer.sprite = columnSprite;
        }
    }
    public void MakeColorBomb()
    {
        if (!IsColumnBomb && !IsRowBomb && !IsAdjacentBomb)
        {
            IsColorBomb = true;
            _spriteRenderer.sprite = colorSprite;
            this.gameObject.tag = "Color";
        }
    }
    public void MakeAdjacentBomb()
    {
        if (!IsColumnBomb && !IsColorBomb && !IsRowBomb)
        {
            IsAdjacentBomb = true;
            _spriteRenderer.sprite = adjacentSprite;
            
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
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
        if (_board.currentState == GameState.move)
        {
            _finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
}
