using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    private Board _board;
    [SerializeField]
    private GameObject[] tileArray;
    // Start is called before the first frame update
    void Start()
    {
        _board = GameObject.FindWithTag("Board").GetComponent<Board>();
        for(int i = 0;i < _board.Width; i++)
        {
            for(int h = 0; h < _board.Height; h++)
            {
                int randomTile = Random.Range(0, 1);
                Vector2 spawnPosition = new Vector2(i, h);
                Instantiate(tileArray[randomTile],spawnPosition , Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
