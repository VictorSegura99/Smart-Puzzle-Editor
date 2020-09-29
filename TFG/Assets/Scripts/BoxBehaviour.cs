using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class BoxBehaviour : MonoBehaviour
{
    enum TRIGGER_DIRECTION
    {
        IDLE,
        NORTH,
        SOUTH,
        WEST,
        EAST
    }

    Animator anim;
    TRIGGER_DIRECTION trigger_dir = TRIGGER_DIRECTION.IDLE;
    bool can_move = true;

    public float time_to_reset = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Triggered(GameObject GO)
    {
        if (trigger_dir == TRIGGER_DIRECTION.IDLE && can_move)
        {
            if (GO.name == "North")
            {
                trigger_dir = TRIGGER_DIRECTION.NORTH;
            }
            else if (GO.name == "South")
            {
                trigger_dir = TRIGGER_DIRECTION.SOUTH;
            }
            else if (GO.name == "West")
            {
                trigger_dir = TRIGGER_DIRECTION.WEST;
            }
            else if (GO.name == "East")
            {
                trigger_dir = TRIGGER_DIRECTION.EAST;
            }

            can_move = false;
            anim.SetInteger("Direction", (int)trigger_dir);
        }
    }

    void Stop()
    {
        trigger_dir = TRIGGER_DIRECTION.IDLE;
        anim.SetInteger("Direction", (int)trigger_dir);
        anim.SetTrigger("Stopped");
        StartCoroutine(MoveReset());
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
}
