using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class BoxBehaviour : MonoBehaviour
{
    // Components
    Animator anim;
    AudioSource sfx;

    // Inspector Variables
    public bool is_static = false;
    public float time_to_reset = 1.5f;
    public float raycast_lenght = 1.25f;

    // Internal Variables
    bool can_move = true;
    bool repeat_anim = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MovementTriggered(GameObject GO, Box_Trigger.Box_Trigger_Side side)
    {
        if (!is_static && can_move && anim.GetCurrentAnimatorStateInfo(0).IsName("IDLE"))
        {
            Vector2 direction = new Vector2();
            Vector2 offset = new Vector2();
            can_move = false;
            StartCoroutine(MoveReset());

            // Checks the name of the trigger activated to set the direction
            // that the box has to move.
            switch (side)
            {
                case Box_Trigger.Box_Trigger_Side.NORTH:
                    {
                        direction = new Vector2(0, -1);
                        offset = new Vector2(0, -1);
                        break;
                    }
                case Box_Trigger.Box_Trigger_Side.SOUTH:
                    {
                        direction = new Vector2(0, 1);
                        offset = new Vector2(0, 1);
                        break;
                    }
                case Box_Trigger.Box_Trigger_Side.WEST:
                    {
                        direction = new Vector2(1, 0);
                        offset = new Vector2(1, 0);
                        break;
                    }
                case Box_Trigger.Box_Trigger_Side.EAST:
                    {
                        direction = new Vector2(-1, 0);
                        offset = new Vector2(-1, 0);
                        break;
                    }
            }

            // Checking if the box can move:
            // Launch a ray in the move direction and check if there is any collider.
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + offset.x, transform.position.y + offset.y), direction);
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                switch (side)
                {
                    case Box_Trigger.Box_Trigger_Side.NORTH:
                        {
                            if (Mathf.Abs((transform.position.y - hit.transform.position.y)) <= raycast_lenght)
                            {
                                StopMove();
                                return;
                            }
                            break;
                        }
                    case Box_Trigger.Box_Trigger_Side.SOUTH:
                        {
                            if (Mathf.Abs((transform.position.y - hit.transform.position.y)) <= raycast_lenght)
                            {
                                StopMove();
                                return;
                            }
                            break;
                        }
                    case Box_Trigger.Box_Trigger_Side.WEST:
                        {
                            if (Mathf.Abs((transform.position.x - hit.transform.position.x)) <= raycast_lenght)
                            {
                                StopMove();
                                return;
                            }
                            break;
                        }
                    case Box_Trigger.Box_Trigger_Side.EAST:
                        {
                            if (Mathf.Abs((transform.position.x - hit.transform.position.x)) <= raycast_lenght)
                            {
                                StopMove();
                                return;
                            }
                            break;
                        }
                }
            }
            else if (hit.collider.CompareTag("Portal"))
            {
                switch (side)
                {
                    case Box_Trigger.Box_Trigger_Side.NORTH:
                        {
                            if (Mathf.Abs((transform.position.y - hit.collider.transform.position.y)) <= raycast_lenght)
                            {
                                side += 4;
                            }
                            break;
                        }
                    case Box_Trigger.Box_Trigger_Side.SOUTH:
                        {
                            if (Mathf.Abs((hit.collider.transform.position.y - transform.position.y)) <= raycast_lenght)
                            {
                                side += 4;
                            }
                            break;
                        }
                    case Box_Trigger.Box_Trigger_Side.WEST:
                        {
                            if (Mathf.Abs((hit.collider.transform.position.x - transform.position.x)) <= raycast_lenght)
                            {
                                side += 4;
                            }
                            break;
                        }
                    case Box_Trigger.Box_Trigger_Side.EAST:
                        {
                            if (Mathf.Abs((transform.position.x - hit.collider.transform.position.x)) <= raycast_lenght)
                            {
                                side += 4;
                            }
                            break;
                        }
                }
            }

            GO.transform.GetChild(0).gameObject.SetActive(true);
            anim.SetInteger("Direction", (int)side);
        }
    }

    void Stop()
    {
        // Deactivates the particles systems.
        if (transform.GetChild(1).GetChild(0).gameObject.activeSelf)
        {
            transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }
        else if(transform.GetChild(4).GetChild(0).gameObject.activeSelf)
        {
            transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
        }
        else if (transform.GetChild(3).GetChild(0).gameObject.activeSelf)
        {
            transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
        }
        else if(transform.GetChild(2).GetChild(0).gameObject.activeSelf)
        {
            transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
        }

        anim.SetInteger("Direction", 0);
        anim.SetTrigger("Stopped");
    }

    IEnumerator MoveReset()
    {
        float time = Time.realtimeSinceStartup;

        while (time + time_to_reset > Time.realtimeSinceStartup)
        {
            yield return null;
        }

        can_move = true;
    }

    void StopMove()
    {
        if (!sfx.isPlaying)
        {
            sfx.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal") && !repeat_anim) 
        {
            repeat_anim = true;
        }
    }
}
