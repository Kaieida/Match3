using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Board _board;
    public float CameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 2;
    // Start is called before the first frame update
    void Start()
    {
        _board = FindObjectOfType<Board>();
        if (_board != null)
        {
            CameraReposition(_board.Width-1, _board.Height-1);
        }
    }
    void CameraReposition(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x / 2, y / 2,CameraOffset);
        transform.position = tempPosition;
        if (_board.Width >= _board.Height)
        {
            Camera.main.orthographicSize = (_board.Width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = _board.Height / 2 + padding;
        }    
    }
}
