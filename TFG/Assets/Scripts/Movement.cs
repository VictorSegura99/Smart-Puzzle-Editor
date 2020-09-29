using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    enum Player_States
    {
        IDLE,
        RUN
    }

    Rigidbody2D rb;
    Vector2 direction = new Vector2();
    public float speed = 3;
    SpriteRenderer engineer_sprite;
    Player_States player_state = Player_States.IDLE;
    Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        engineer_sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
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

        if (direction.x < 0 && !engineer_sprite.flipX)
        {
            engineer_sprite.flipX = true;
        }
        else if (direction.x > 0 && engineer_sprite.flipX)
        {
            engineer_sprite.flipX = false;
        }

        if (direction != Vector2.zero && player_state!=Player_States.RUN)
        {
            player_state = Player_States.RUN;
        }
        else if (direction == Vector2.zero && player_state != Player_States.IDLE)
        {
            player_state = Player_States.IDLE;
        }

        anim.SetInteger("State", (int)player_state);
        rb.velocity = direction;
    }
}
