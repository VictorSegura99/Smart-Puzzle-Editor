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
    BoxCollider2D main_collider;

    Door_State state = Door_State.CLOSED;

    void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        main_collider = GetComponents<BoxCollider2D>()[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(state.ToString()))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("OPEN"))
            {
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

    public void OpenDoors(bool open)
    {
        if(open)
        {
            state = Door_State.OPENING;
        }
        else
        {
            state = Door_State.CLOSING;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("CLOSING") && state == Door_State.OPENING)
        {
            animator.Play("OPENING");
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("OPENING") && state == Door_State.CLOSING)
        {
            animator.Play("CLOSING");
        }

        main_collider.enabled = !open;
        animator.SetInteger("State", (int)state);
    }

    public bool IsDoorClosed()
    {
        if (state == Door_State.CLOSED) 
        {
            return true;
        }

        return false;
    }
}
