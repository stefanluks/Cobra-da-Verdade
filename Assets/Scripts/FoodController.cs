using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    void Start()
    {
        RandomPosition();
    }

    void RandomPosition()
    {
        int x = Mathf.RoundToInt(Random.Range(-19, 19));
        int y = Mathf.RoundToInt(Random.Range(-10, 10));
        transform.position = new Vector2(x, y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            RandomPosition();
        }
    }
}
