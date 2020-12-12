using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal

}
[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
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

    [Header("Scriptable object stuff")]
    public World world;
    public int level;

    [Header("Board dimensions")]
    public int Width;
    public int Height;
    public int OffSet;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject[] Dots;
    public GameObject DestroyEffect;

    [Header("Layout")]
    public int basePieceValue = 20;
    public float refillDelay = 0.5f;
    public GameObject[,] AllDots;
    public TileType[] BoardLayout;
    public Dot CurrentDot;
    public int[] scoreGoals;
    private bool[,] blankSpaces;
    private int streakValue = 1;
    private GoalManager _goalManager;
    private BackgroundTiles[,] _breakableTiles;
    private FindMatches _findMatches;
    private MatchType _matchType = new MatchType();
    private ScoreManager _scoreManager;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if(world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    Width = world.levels[level].width;
                    Height = world.levels[level].height;
                    Dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    BoardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }
    void Start()
    {
        _goalManager = FindObjectOfType<GoalManager>();
        _scoreManager = FindObjectOfType<ScoreManager>();
        _breakableTiles = new BackgroundTiles[Width, Height];
        _findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[Width, Height];
        AllDots = new GameObject[Width, Height];
        SetUp();
        currentState = GameState.pause;
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

    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < BoardLayout.Length; i++)
        {
            if (BoardLayout[i].tileKind == TileKind.Breakable)
            {
                Vector2 tempPos = new Vector2(BoardLayout[i].x, BoardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPos, Quaternion.identity);
                _breakableTiles[BoardLayout[i].x, BoardLayout[i].y] = tile.GetComponent<BackgroundTiles>();
            }
        }
    }

    private void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (!blankSpaces[i, h])
                {


                    Vector2 _tempPosition = new Vector2(i, h + OffSet);
                    Vector2 tilePosition = new Vector2(i, h);//if spawning of jellys are wrong, add this instead of tempposition.
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

    private MatchType ColumnOrRow()
    {
        List<GameObject> matchCopy = _findMatches.CurrentMatches as List<GameObject>;
        _matchType.type = 0;
        _matchType.color = "";
        for (int i = 0; i < matchCopy.Count; i++)
        {
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            string color = matchCopy[i].tag;
            int column = thisDot.Column;
            int row = thisDot.Row;
            int columnMatch = 0;
            int rowMatch = 0;
            for (int h = 0; h < matchCopy.Count; h++)
            {
                Dot nextDot = matchCopy[h].GetComponent<Dot>();
                if(nextDot == thisDot)
                {
                    continue;
                }
                if(nextDot.Column == thisDot.Column && nextDot.tag == color)
                {
                    columnMatch++;
                }
                if (nextDot.Row == thisDot.Row && nextDot.tag == color)
                {
                    rowMatch++;
                }
            }
            if (columnMatch == 4 || rowMatch == 4)
            {
                _matchType.type = 1;
                _matchType.color = color;
                return _matchType;
            }
            else if(columnMatch == 2 && rowMatch == 2)
            {
                _matchType.type = 2;
                _matchType.color = color;
                return _matchType;
            }
            else if(columnMatch == 3 || rowMatch == 3)
            {
                _matchType.type = 3;
                _matchType.color = color;
                return _matchType;
            }
        }
        _matchType.type = 0;
        _matchType.color = "";
        return _matchType;
    }

    private void CheckToMakeBombs()
    {
        if (_findMatches.CurrentMatches.Count > 3)
        {
            MatchType typeOfMatch = ColumnOrRow();
            if (typeOfMatch.type == 1)
            {
                if (CurrentDot != null && CurrentDot.IsMatched && CurrentDot.tag == typeOfMatch.color)
                {
                    CurrentDot.IsMatched = false;
                    CurrentDot.MakeColorBomb();
                }
                else
                {
                    if (CurrentDot.OtherDot != null)
                    {
                        Dot otherDot = CurrentDot.OtherDot.GetComponent<Dot>();
                        if (otherDot.IsMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.IsMatched = false;
                            otherDot.MakeColorBomb();
                        }
                    }
                }
            }
            else if (typeOfMatch.type == 2)
            {
                if (CurrentDot != null && CurrentDot.IsMatched && CurrentDot.IsMatched && CurrentDot.tag == typeOfMatch.color)
                {
                    CurrentDot.IsMatched = false;
                    CurrentDot.MakeAdjacentBomb();
                }
                else if (CurrentDot.OtherDot != null)
                {
                    Dot otherDot = CurrentDot.OtherDot.GetComponent<Dot>();
                    if (otherDot.IsMatched && otherDot.tag == typeOfMatch.color)
                    {
                        otherDot.IsMatched = false;
                        otherDot.MakeAdjacentBomb();
                    }
                }
            }
            else if (typeOfMatch.type == 3)
            {
                _findMatches.CheckBombs(typeOfMatch);
            }
        }
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (AllDots[column, row].GetComponent<Dot>().IsMatched)
        {
            if (_breakableTiles[column, row] != null)
            {
                _breakableTiles[column, row].TakeDamage(1);
                if (_breakableTiles[column, row].hitPoints <= 0)
                {
                    _breakableTiles[column, row] = null;
                }
            }
            if(_goalManager != null)
            {
                _goalManager.CompareGoal(AllDots[column,row].tag.ToString());
                _goalManager.UpdateGoals();
            }

            GameObject particle = Instantiate(DestroyEffect, AllDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(AllDots[column, row]);
            _scoreManager.IncreaseScore(basePieceValue * streakValue);
            AllDots[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        if (_findMatches.CurrentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
        _findMatches.CurrentMatches.Clear();
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
                    for (int k = h + 1; k < Height; k++)
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
        yield return new WaitForSeconds(refillDelay * 0.5f);
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
                if (AllDots[i, h] == null && !blankSpaces[i, h])
                {
                    Vector2 tempPosition = new Vector2(i, h + OffSet);
                    int dotToUse = Random.Range(0, Dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, h, Dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, Dots.Length);
                    }
                    maxIterations = 0;
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
        _findMatches.FindingAllMatches();
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
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
            //yield return new WaitForSeconds(refillDelay * 2);
        }
        CurrentDot = null;

        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
        yield return new WaitForSeconds(refillDelay);
        if(currentState != GameState.win)
        {
            currentState = GameState.move;
        }
        
        streakValue = 1;
    }
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject holder = AllDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        AllDots[column + (int)direction.x, row + (int)direction.y] = AllDots[column, row];
        AllDots[column, row] = holder;
    }
    private bool CheckForMatches()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] != null)
                {
                    if (i < Width - 2)
                    {
                        if (AllDots[i + 1, h] != null && AllDots[i + 2, h] != null)
                        {
                            if (AllDots[i + 1, h].tag == AllDots[i, h].tag && AllDots[i + 2, h].tag == AllDots[i, h].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (h < Height - 2)
                    {
                        if (AllDots[i, h + 1] != null && AllDots[i, h + 2] != null)
                        {
                            if (AllDots[i, h + 1].tag == AllDots[i, h].tag && AllDots[i, h + 2].tag == AllDots[i, h].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }
    private bool IsDeadLocked()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] != null)
                {
                    if (i < Width - 1)
                    {
                        if (SwitchAndCheck(i, h, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (h < Height - 1)
                    {
                        if (SwitchAndCheck(i, h, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] != null)
                {
                    newBoard.Add(AllDots[i, h]);
                }
            }
        }
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (!blankSpaces[i, h])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    int maxIterations = 0;
                    while (MatchesAt(i, h, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;
                    piece.Column = i;
                    piece.Row = h;
                    AllDots[i, h] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
    }
}
