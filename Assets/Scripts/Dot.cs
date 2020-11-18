using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private Vector2 _firstTouchPosition;
    private Vector2 _finalTouchPosition;
    public float SwipeAngle = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        SwipeAngle = Mathf.Atan2(_finalTouchPosition.y - _firstTouchPosition.y, _finalTouchPosition.x - _firstTouchPosition.x)*180/Mathf.PI;
    }
}
