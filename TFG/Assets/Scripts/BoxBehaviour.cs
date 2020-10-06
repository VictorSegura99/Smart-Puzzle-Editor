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

    // Internal Variables
    enum TRIGGER_DIRECTION
    {
        IDLE,
        NORTH,
        SOUTH,
        WEST,
        EAST
    }

    TRIGGER_DIRECTION trigger_dir = TRIGGER_DIRECTION.IDLE;
    bool can_move = true;


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

    public void Triggered(GameObject GO)
    {
        if (!is_static && trigger_dir == TRIGGER_DIRECTION.IDLE && can_move)
        {
            Vector2 direction = new Vector2();
            Vector2 offset = new Vector2();
            can_move = false;
            StartCoroutine(MoveReset());

            // Checks the name of th trigger activated to set the direction
            // that the box has to move.
            if (GO.name == "North")
            {
                trigger_dir = TRIGGER_DIRECTION.NORTH;
                direction = new Vector2(0, -1);
                offset = new Vector2(0, -1);
            }
            else if (GO.name == "South")
            {
                trigger_dir = TRIGGER_DIRECTION.SOUTH;
                direction = new Vector2(0, 1);
                offset = new Vector2(0, 1);
            }
            else if (GO.name == "West")
            {
                trigger_dir = TRIGGER_DIRECTION.WEST;
                direction = new Vector2(1, 0);
                offset = new Vector2(1, 0);
            }
            else if (GO.name == "East")
            {
                trigger_dir = TRIGGER_DIRECTION.EAST;
                direction = new Vector2(-1, 0);
                offset = new Vector2(-1, 0);
            }

            // Checking if the box can move:
            // Launch a ray in the move direction and check if there is any collider.
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + offset.x, transform.position.y + offset.y), direction);
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                switch (trigger_dir)
                {
                    case TRIGGER_DIRECTION.NORTH:
                        {
                            if (Mathf.Abs((transform.position.y - hit.transform.position.y)) <= 1.5f)
                            {
                                StopMove();
                                return;
                            }
                            break;
                        }
                    case TRIGGER_DIRECTION.SOUTH:
                        {
                            if (Mathf.Abs((transform.position.y - hit.transform.position.y)) <= 1.5f)
                            {
                                StopMove();
                                return;
                            }
                            break;
                        }
                    case TRIGGER_DIRECTION.WEST:
                        {
                            if (Mathf.Abs((transform.position.x - hit.transform.position.x)) <= 1.5f)
                            {
                                StopMove();
                                return;
                            }
                            break;
                        }
                    case TRIGGER_DIRECTION.EAST:
                        {
                            if (Mathf.Abs((transform.position.x - hit.transform.position.x)) <= 1.5f)
                            {
                                StopMove();
                                return;
                            }
                            break;
                        }
                }
            }

            GO.transform.GetChild(0).gameObject.SetActive(true);
            anim.SetInteger("Direction", (int)trigger_dir);
        }
    }

    void Stop()
    {
        // Deactivates the particles systems.
        switch(trigger_dir)
        {
            case TRIGGER_DIRECTION.NORTH:
                transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                break;
            case TRIGGER_DIRECTION.EAST:
                transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
                break;
            case TRIGGER_DIRECTION.WEST:
                transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
                break;
            case TRIGGER_DIRECTION.SOUTH:
                transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                break;
        }

        trigger_dir = TRIGGER_DIRECTION.IDLE;
        anim.SetInteger("Direction", (int)trigger_dir);
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
        trigger_dir = TRIGGER_DIRECTION.IDLE;

        if (!sfx.isPlaying)
        {
            sfx.Play();
        }
    }
}
