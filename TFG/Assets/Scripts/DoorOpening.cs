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

    Door_State state = Door_State.CLOSED;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        upper_door_sprite = upper_door.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case Door_State.OPEN:
                if (!upper_door_sprite.enabled)
                {
                    upper_door_sprite.enabled = true;
                }
                break;
            case Door_State.OPENING:
                break;
            case Door_State.CLOSED:
                break;
            case Door_State.CLOSING:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
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
