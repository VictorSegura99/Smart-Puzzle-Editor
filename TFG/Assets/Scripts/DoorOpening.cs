using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class DoorOpening : MonoBehaviour
{
    // Components
    Animator animator;
    BoxCollider2D main_collider;

    // Inspector Variables

    // Internal Variables
    enum Door_State
    {
        CLOSED,
        OPEN,
        OPENING,
        CLOSING
    }

    Door_State state = Door_State.CLOSED;

    [HideInInspector]
    public bool door_opened_lock = false;

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

        // DEBUG:
        // Open/Close door
        //if ((state == Door_State.OPEN || state == Door_State.CLOSED) && Input.GetKeyDown(KeyCode.Space))
        //{
        //    OpenDoors(IsDoorClosed());
        //}
    }

    public void OpenDoors(bool open)
    {
        float moment = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        main_collider.enabled = !open;

        if (open)
        {
            state = Door_State.OPENING;
        }
        else
        {
            state = Door_State.CLOSING;
        }

        animator.SetInteger("State", (int)state);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(state.ToString()))
        {
            if (moment > 0 && moment < 1)
            {
                animator.Play(state.ToString(), 0, 1 - moment);
            }
        }
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
