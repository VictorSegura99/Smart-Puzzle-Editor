using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direction = new Vector2();
    public float speed = 3;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        direction *= 0;

        if(Input.GetKey(KeyCode.W))
        {
            direction.y += speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction.y -= speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction.x -= speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction.x += speed;
        }

        rb.velocity = direction;
    }
}
