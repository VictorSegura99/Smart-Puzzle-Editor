using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpening : MonoBehaviour
{
    public GameObject upper_door;

    enum Door_State
    {
        CLOSED,
        OPEN,
        OPENING,
        CLOSING
    }

    Animator animator;
    SpriteRenderer upper_door_sprite;
    BoxCollider2D main_collider;

    Door_State state = Door_State.CLOSED;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        upper_door_sprite = upper_door.GetComponent<SpriteRenderer>();
        main_collider = GetComponents<BoxCollider2D>()[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(state.ToString()))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("OPEN"))
            {
                upper_door_sprite.enabled = true;
                main_collider.enabled = false;
                state = Door_State.OPEN;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("CLOSED")) 
            {
                state = Door_State.CLOSED;
            }
        }

        if ((state == Door_State.OPEN || state == Door_State.CLOSED) && Input.GetKeyDown(KeyCode.Space))
        {
            OpenDoors(IsDoorClosed());
        }
    }

    void OpenDoors(bool open)
    {
        if(open)
        {
            state = Door_State.OPENING;
        }
        else
        {
            state = Door_State.CLOSING;
        }

        upper_door_sprite.enabled = false;
        main_collider.enabled = true;
        animator.SetInteger("State", (int)state);
    }

    bool IsDoorClosed()
    {
        if (state == Door_State.CLOSED) 
        {
            return true;
        }

        return false;
    }
}
