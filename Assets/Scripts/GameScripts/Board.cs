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
    Lock,
    Concrete,
    Slime,
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
    [Header("Scriptable object stuff")]
    public World world;
    public int level;

    public GameState currentState = GameState.move;
    [Header("Board dimensions")]
    public int Width;
    public int Height;
    public int OffSet;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    [SerializeField] private GameObject _backgroundTile;
    public GameObject[] Dots;
    public GameObject lockTilePrefab;
    public GameObject concretetilePrefab;
    public GameObject slimePiecePrefab;

    [Header("Layout")]
    public int basePieceValue = 20;
    public float refillDelay = 0.5f;
    public GameObject[,] AllDots;
    public BackgroundTiles[,] lockTiles;
    private BackgroundTiles[,] _slimeTiles;
    
    public TileType[] BoardLayout;
    public Dot CurrentDot;
    public int[] scoreGoals;
    private bool[,] blankSpaces;
    private int streakValue = 1;
    private GoalManager _goalManager;
    private SoundManager _soundManager;
    private BackgroundTiles[,] _breakableTiles;
    private BackgroundTiles[,] _concreteTiles;
    private GameObject[,] _backgroundTiles;
    private FindMatches _findMatches;
    private MatchType _matchType = new MatchType();
    private ScoreManager _scoreManager;
    private bool makeSlime = false;

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
        _soundManager = FindObjectOfType<SoundManager>();
        _goalManager = FindObjectOfType<GoalManager>();
        _scoreManager = FindObjectOfType<ScoreManager>();
        _backgroundTiles = new GameObject[Width, Height];
        _breakableTiles = new BackgroundTiles[Width, Height];
        lockTiles = new BackgroundTiles[Width, Height];
        _concreteTiles = new BackgroundTiles[Width, Height];
        _slimeTiles = new BackgroundTiles[Width, Height];
        _findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[Width, Height];
        AllDots = new GameObject[Width, Height];
        SetUp();
        currentState = GameState.pause;
    }
    private void GenerateBackgroundTiles()
    {
        for(int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                Vector2 tempPos = new Vector2(i, h);
                GameObject tile = Instantiate(_backgroundTile, tempPos, Quaternion.identity);
                _backgroundTiles[i, h] = tile;
            }
        }
        for (int i = 0; i < BoardLayout.Length; i++)
        {
            Destroy(_backgroundTiles[BoardLayout[i].x, BoardLayout[i].y]);
        }

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
    private void GenerateLockTiles()
    {
        for (int i = 0; i < BoardLayout.Length; i++)
        {
            if (BoardLayout[i].tileKind == TileKind.Lock)
            {
                Vector2 tempPos = new Vector2(BoardLayout[i].x, BoardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPos, Quaternion.identity);
                lockTiles[BoardLayout[i].x, BoardLayout[i].y] = tile.GetComponent<BackgroundTiles>();
            }
        }
    }
    private void GenerateConcreteTiles()
    {
        for (int i = 0; i < BoardLayout.Length; i++)
        {
            if (BoardLayout[i].tileKind == TileKind.Concrete)
            {
                Vector2 tempPos = new Vector2(BoardLayout[i].x, BoardLayout[i].y);
                GameObject tile = Instantiate(concretetilePrefab, tempPos, Quaternion.identity);
                _concreteTiles[BoardLayout[i].x, BoardLayout[i].y] = tile.GetComponent<BackgroundTiles>();
            }
        }
    }
    private void GenerateSlimeTiles()
    {
        for (int i = 0; i < BoardLayout.Length; i++)
        {
            if (BoardLayout[i].tileKind == TileKind.Slime)
            {
                Vector2 tempPos = new Vector2(BoardLayout[i].x, BoardLayout[i].y);
                GameObject tile = Instantiate(slimePiecePrefab, tempPos, Quaternion.identity);
                _slimeTiles[BoardLayout[i].x, BoardLayout[i].y] = tile.GetComponent<BackgroundTiles>();
            }
        }
    }

    private void SetUp()
    {
        GenerateBackgroundTiles();
        //GenerateBlankSpaces();
        GenerateBreakableTiles();
        GenerateLockTiles();
        GenerateConcreteTiles();
        GenerateSlimeTiles();
        GenerateBlankSpaces();
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (!blankSpaces[i, h] && !_concreteTiles[i,h] && !_slimeTiles[i,h])
                {
                    Vector2 _tempPosition = new Vector2(i, h + OffSet);
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
                if (AllDots[column - 1, row].CompareTag(piece.tag) && AllDots[column - 2, row].CompareTag(piece.tag))
                {
                    return true;
                } 
            }
            if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
            {
                if (AllDots[column, row-1].CompareTag(piece.tag) && AllDots[column, row-2].CompareTag(piece.tag))
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
                    if (AllDots[column, row - 1].CompareTag(piece.tag) && AllDots[column, row - 2].CompareTag(piece.tag))
                    {
                        return true;
                    }
                }
            }

            if (column > 1)
            {
                if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)
                {
                    if (AllDots[column - 1, row].CompareTag(piece.tag) && AllDots[column - 2, row].CompareTag(piece.tag))
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
                if (nextDot.Column == thisDot.Column && nextDot.CompareTag(color))
                {
                    columnMatch++;
                }
                if (nextDot.Row == thisDot.Row && nextDot.CompareTag(color))
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
                if (CurrentDot != null && CurrentDot.IsMatched && CurrentDot.CompareTag(typeOfMatch.color))
                {
                    CurrentDot.IsMatched = false;
                    CurrentDot.MakeColorBomb();
                }
                else
                {
                    if (CurrentDot.OtherDot != null)
                    {
                        Dot otherDot = CurrentDot.OtherDot.GetComponent<Dot>();
                        if (otherDot.IsMatched && otherDot.CompareTag(typeOfMatch.color))
                        {
                            otherDot.IsMatched = false;
                            otherDot.MakeColorBomb();
                        }
                    }
                }
            }
            else if (typeOfMatch.type == 2)
            {
                if (CurrentDot != null && CurrentDot.IsMatched && CurrentDot.IsMatched && CurrentDot.CompareTag(typeOfMatch.color))
                {
                    CurrentDot.IsMatched = false;
                    CurrentDot.MakeAdjacentBomb();
                }
                else if (CurrentDot.OtherDot != null)
                {
                    Dot otherDot = CurrentDot.OtherDot.GetComponent<Dot>();
                    if (otherDot.IsMatched && otherDot.CompareTag(typeOfMatch.color))
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
    public void BombRow(int row)
    {
        for (int i = 0; i < Width; i++)
        {
            if (_concreteTiles[i, row])
            {
                _concreteTiles[i, row].TakeDamage(1);
                if (_concreteTiles[i, row].hitPoints <= 0)
                {
                    _concreteTiles[i, row] = null;
                }
            }
        }
    }
    public void BombColumn(int column)
    {
        for (int i = 0; i < Width; i++)
        {
            if (_concreteTiles[column, i])
            {
                _concreteTiles[column, i].TakeDamage(1);
                if (_concreteTiles[column, i].hitPoints <= 0)
                {
                    _concreteTiles[column, i] = null;
                }
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
            if (lockTiles[column, row] != null)
            {
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }
            }
            DamageConcrete(column,row);
            DamageSlime(column, row);
            if (_goalManager != null)
            {
                _goalManager.CompareGoal(AllDots[column,row].tag.ToString());
                _goalManager.UpdateGoals();
            }
            if(_soundManager != null)
            {
                _soundManager.PlayRandomDestroyNoise();
            }
            AllDots[column, row].GetComponent<Dot>().StartAnimation();
            if (currentState != GameState.win || currentState != GameState.lose)
            {
                _scoreManager.IncreaseScore(basePieceValue * streakValue);
            }
            
            AllDots[column, row] = null;
        }
    }
    private void DamageSlime(int column, int row)
    {
        if (column > 0)
        {
            if (_slimeTiles[column - 1, row])
            {
                _slimeTiles[column - 1, row].TakeDamage(1);
                if (_slimeTiles[column - 1, row].hitPoints <= 0)
                {
                    _slimeTiles[column - 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (column < Width - 1)
        {
            if (_slimeTiles[column + 1, row])
            {
                _slimeTiles[column + 1, row].TakeDamage(1);
                if (_slimeTiles[column + 1, row].hitPoints <= 0)
                {
                    _slimeTiles[column + 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (row > 0)
        {
            if (_slimeTiles[column, row - 1])
            {
                _slimeTiles[column, row - 1].TakeDamage(1);
                if (_slimeTiles[column, row - 1].hitPoints <= 0)
                {
                    _slimeTiles[column, row - 1] = null;
                }
                makeSlime = false;
            }
        }
        if (row < Height - 1)
        {
            if (_slimeTiles[column, row + 1])
            {
                _slimeTiles[column, row + 1].TakeDamage(1);
                if (_slimeTiles[column, row + 1].hitPoints <= 0)
                {
                    _slimeTiles[column, row + 1] = null;
                }
                makeSlime = false;
            }
        }
    }
    private void DamageConcrete(int column, int row)
    {
        if (column > 0)
        {
            if (_concreteTiles[column - 1, row])
            {
                _concreteTiles[column - 1, row].TakeDamage(1);
                if (_concreteTiles[column - 1, row].hitPoints <= 0)
                {
                    _concreteTiles[column - 1, row] = null;
                }
            }
        }
        if (column < Width - 1)
        {
            if (_concreteTiles[column + 1, row])
            {
                _concreteTiles[column + 1, row].TakeDamage(1);
                if (_concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    _concreteTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (_concreteTiles[column, row - 1])
            {
                _concreteTiles[column, row - 1].TakeDamage(1);
                if (_concreteTiles[column, row - 1].hitPoints <= 0)
                {
                    _concreteTiles[column, row - 1] = null;
                }
            }
        }
        if (row < Height - 1)
        {
            if (_concreteTiles[column, row + 1])
            {
                _concreteTiles[column, row + 1].TakeDamage(1);
                if (_concreteTiles[column, row + 1].hitPoints <= 0)
                {
                    _concreteTiles[column, row + 1] = null;
                }
            }
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
        yield return new WaitForSeconds(0.35f); //pause before candies falls after destroying matches.
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (!blankSpaces[i, h] && AllDots[i, h] == null && !_concreteTiles[i,h] && !_slimeTiles[i,h])
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
    private void RefillBoard()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (AllDots[i, h] == null && !blankSpaces[i, h] && !_concreteTiles[i, h] && !_slimeTiles[i, h])
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
                    piece.transform.parent = transform;
                    piece.GetComponent<Dot>().Row = h;
                    piece.GetComponent<Dot>().Column = i;
                    piece.name = "(" + i + ", " + h + ")";
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
        yield return new WaitForSeconds(refillDelay*0.25f);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
        }
        CurrentDot = null;
        CheckToMakeSlime();
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
        yield return new WaitForSeconds(refillDelay);
        System.GC.Collect();
        if (currentState != GameState.win && currentState != GameState.pause)
        {
            currentState = GameState.move;
        }
            makeSlime = false;
        
        
        streakValue = 1;
    }
    private void CheckToMakeSlime()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int h = 0; h < Height; h++)
            {
                if (_slimeTiles[i, h] != null && !makeSlime)
                {
                    MakeNewSlime();
                }
            }
        }
    }
    private Vector2 CheckForAdjacent(int column, int row)
    {
        if (column<Width-1 && AllDots[column + 1, row])
        {
            return Vector2.right;
        }
        if (column > 0&& AllDots[column - 1, row] )
        {
            return Vector2.left;
        }
        if (row < Height - 1 && AllDots[column, row + 1])
        {
            return Vector2.up;
        }
        if (row > 0 && AllDots[column, row - 1])
        {
            return Vector2.down;
        }
        
        return Vector2.zero;
        
    }
    private void MakeNewSlime()
    {
        bool slime = false;
        int loops = 0;
        Debug.Log("Loop starts");
        while (!slime && loops < 200)
        {
            int newX = Random.Range(0,Width);
            int newY = Random.Range(0,Height);
            if (_slimeTiles[newX, newY])
            {
                Vector2 adjacent = CheckForAdjacent(newX, newY);
                Debug.Log(adjacent);
                if (adjacent != Vector2.zero)
                {
                    Destroy(AllDots[newX+(int)adjacent.x, newY+(int)adjacent.y]);
                    Vector2 tempPosition = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y);
                    Debug.Log("Creating slime");
                    GameObject tile = Instantiate(slimePiecePrefab, tempPosition, Quaternion.identity);
                    _slimeTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTiles>();
                    slime = true;
                }
            }
            loops++;
        }
    }
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        if (AllDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            GameObject holder = AllDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
            AllDots[column + (int)direction.x, row + (int)direction.y] = AllDots[column, row];
            AllDots[column, row] = holder;
        }
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
                            if (AllDots[i + 1, h].CompareTag(AllDots[i, h].tag) && AllDots[i + 2, h].CompareTag(AllDots[i, h].tag))
                            {
                                return true;
                            }
                        }
                    }
                    if (h < Height - 2)
                    {
                        if (AllDots[i, h + 1] != null && AllDots[i, h + 2] != null)
                        {
                            if (AllDots[i, h + 1].CompareTag(AllDots[i, h].tag) && AllDots[i, h + 2].CompareTag(AllDots[i, h].tag))
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
                if (!blankSpaces[i, h] && !_concreteTiles[i,h] && !_slimeTiles[i, h])
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
