using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Trigger : MonoBehaviour
{
    public enum Box_Trigger_Side
    {
        NONE = -1,
        NORTH,
        SOUTH,
        WEST,
        EAST   
    }

    public Box_Trigger_Side side = Box_Trigger_Side.NONE;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CheckPlayerCollision(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CheckPlayerCollision(collision);
        }
    }

    void CheckPlayerCollision(Collider2D collision)
    {
        switch (side)
        {
            case Box_Trigger_Side.NONE:
                {
                    Debug.Log("No side selected for the Box Trigger");
                    return;
                }
            case Box_Trigger_Side.NORTH:
                if (collision.GetComponent<Player_Controller>().direction.y >= 0)
                    return;
                break;
            case Box_Trigger_Side.SOUTH:
                if (collision.GetComponent<Player_Controller>().direction.y <= 0)
                    return;
                break;
            case Box_Trigger_Side.WEST:
                if (collision.GetComponent<Player_Controller>().direction.x <= 0)
                    return;
                break;
            case Box_Trigger_Side.EAST:
                if (collision.GetComponent<Player_Controller>().direction.x >= 0)
                    return;
                break;
        }

        transform.parent.GetComponent<BoxBehaviour>().MovementTriggered(side);
    }
}
