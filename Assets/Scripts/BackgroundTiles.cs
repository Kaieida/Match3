using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTiles : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite;
    private GoalManager _goalManager;
    private void Start()
    {
        _goalManager = FindObjectOfType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if(hitPoints <= 0)
        {
            if(_goalManager != null)
            {
                _goalManager.CompareGoal(this.gameObject.tag);
                _goalManager.UpdateGoals();
            }
            Destroy(this.gameObject);
        }  
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }
    private void MakeLighter()
    {
        Color color = sprite.color;
        float newAlpha = color.a * 0.5f;
        sprite.color = new Color(color.r,color.g,color.b,newAlpha);
    }
}
