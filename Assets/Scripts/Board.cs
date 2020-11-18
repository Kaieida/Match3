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
}
