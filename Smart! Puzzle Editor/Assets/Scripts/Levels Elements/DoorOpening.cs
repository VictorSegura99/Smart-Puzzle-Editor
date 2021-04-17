using UnityEngine;

public class DoorOpening : MonoBehaviour
{
    // Components
    Animator animator;
    Collider2D main_collider;

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

    void Awake()
    {
        animator = GetComponent<Animator>();
        main_collider = GetComponents<Collider2D>()[0];
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
        return state == Door_State.CLOSED;
    }
}
