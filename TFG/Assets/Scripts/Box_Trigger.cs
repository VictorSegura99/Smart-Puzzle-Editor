using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Trigger : MonoBehaviour
{
    public enum Box_Trigger_Side
    {
        NONE,
        NORTH,
        SOUTH,
        WEST,
        EAST    
    }

    public Box_Trigger_Side side = Box_Trigger_Side.NONE;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            CheckPlayerCollision(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            CheckPlayerCollision(collision);
        }
    }

    void CheckPlayerCollision(Collider2D collision)
    {
        switch (side)
        {
            case Box_Trigger_Side.NORTH:
                if (collision.GetComponent<Movement>().direction.y >= 0)
                    return;
                break;
            case Box_Trigger_Side.SOUTH:
                if (collision.GetComponent<Movement>().direction.y <= 0)
                    return;
                break;
            case Box_Trigger_Side.EAST:
                if (collision.GetComponent<Movement>().direction.x >= 0)
                    return;
                break;
            case Box_Trigger_Side.WEST:
                if (collision.GetComponent<Movement>().direction.x <= 0)
                    return;
                break;
            case Box_Trigger_Side.NONE:
                {
                    Debug.Log("No side selected for the Box Trigger");
                    return;
                }
        }
        transform.parent.GetComponent<BoxBehaviour>().MovementTriggered(gameObject, side);
    }
}
