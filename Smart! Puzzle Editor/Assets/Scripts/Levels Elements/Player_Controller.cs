using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Player_Controller : MonoBehaviour
{
    // Components
    Rigidbody2D rb;
    SpriteRenderer engineer_sprite;
    Animator anim;
    [SerializeField]
    Transform shadow;
    Vector3 originalShadowScale;

    // Inspector Variables
    public float speed = 3;

    // Internal Variables
    enum Player_States
    {
        IDLE,
        RUN
    }
    Player_States player_state = Player_States.IDLE;

    [HideInInspector]
    public Vector2 direction = new Vector3();

    bool input_blocked = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        engineer_sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        originalShadowScale = shadow.localScale;
    }

    private void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!input_blocked)
        {
            // Get Input
            Vector2 KB_direction = HandleKeyboardMovement();
            Vector2 GP_direction = HandleControllerMovement();
            Vector2 GP_Arrows_direction = HandleGPArrowsMovement();

            direction = new Vector2(KB_direction.x + GP_direction.x + GP_Arrows_direction.x, KB_direction.y + GP_direction.y + GP_Arrows_direction.y).normalized * speed;

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
                Vector3 scale = originalShadowScale;
                scale.x *= 1.05f;
                shadow.localScale = scale;
            }
            else if (direction == Vector2.zero && player_state != Player_States.IDLE)
            {
                player_state = Player_States.IDLE;
                shadow.localScale = originalShadowScale;
            }

            anim.SetInteger("State", (int)player_state);
            rb.velocity = direction;
        }
    }

    Vector2 HandleKeyboardMovement()
    {
        Vector2 direction = new Vector2();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            direction.y += speed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            direction.y -= speed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            direction.x -= speed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            direction.x += speed;
        }

        return direction.normalized;
    }

    Vector2 HandleControllerMovement()
    {
        return new Vector2(GamePad.GetState(0).ThumbSticks.Left.X, GamePad.GetState(0).ThumbSticks.Left.Y).normalized;
    }

    Vector2 HandleGPArrowsMovement()
    {
        GamePadState state = GamePad.GetState(0);
        Vector2 direction = new Vector2();

        if (state.DPad.Up == ButtonState.Pressed)
        {
            direction.y += speed;
        }

        if (state.DPad.Down == ButtonState.Pressed)
        {
            direction.y -= speed;
        }

        if (state.DPad.Left == ButtonState.Pressed)
        {
            direction.x -= speed;
        }

        if (state.DPad.Right == ButtonState.Pressed)
        {
            direction.x += speed;
        }

        return direction.normalized;
    }

    public void BlockInput(bool block)
    {
        input_blocked = block;
        rb.velocity = Vector2.zero;
        player_state = Player_States.IDLE;
        shadow.localScale = new Vector3(2.75f, 1, 0.8f);
        anim.SetInteger("State", (int)player_state);
    }

    public bool IsInputBlocked()
    {
        return input_blocked;
    }
}
