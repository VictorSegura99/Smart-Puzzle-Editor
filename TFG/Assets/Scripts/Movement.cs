using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using XInputDotNetPure;

public class Movement : MonoBehaviour
{
    enum Player_States
    {
        IDLE,
        RUN
    }

    Rigidbody2D rb;
    public float speed = 3;
    SpriteRenderer engineer_sprite;
    Player_States player_state = Player_States.IDLE;
    Animator anim;
    Transform shadow;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        engineer_sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        shadow = transform.GetChild(0).GetChild(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get Input
        Vector2 KB_direction = HandleKeyboardMovement();
        Vector2 GP_direction = HandleControllerMovement();

        Vector2 direction = new Vector2(Mathf.Clamp(KB_direction.x + GP_direction.x, -speed, speed), Mathf.Clamp(KB_direction.y + GP_direction.y, -speed, speed)).normalized * speed;

        // Flip Sprites
        if (direction.x < 0 && !engineer_sprite.flipX)
        {
            engineer_sprite.flipX = true;
        }
        else if (direction.x > 0 && engineer_sprite.flipX)
        {
            engineer_sprite.flipX = false;
        }

        // Animations Control
        if (direction != Vector2.zero && player_state != Player_States.RUN)
        {
            player_state = Player_States.RUN;
            shadow.localScale = new Vector3(3, 1, 0.8f);
        }
        else if (direction == Vector2.zero && player_state != Player_States.IDLE)
        {
            player_state = Player_States.IDLE;
            shadow.localScale = new Vector3(2.75f, 1, 0.8f);
        }

        anim.SetInteger("State", (int)player_state);
        rb.velocity = direction;
    }

    Vector2 HandleKeyboardMovement()
    {
        Vector2 direction = new Vector2();

        if (Input.GetKey(KeyCode.W))
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

        return direction;
    }

    Vector2 HandleControllerMovement()
    {
        return (new Vector2(GamePad.GetState(0).ThumbSticks.Left.X, GamePad.GetState(0).ThumbSticks.Left.Y)) * speed;
    }
}
