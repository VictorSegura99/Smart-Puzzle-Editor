using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Trigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            transform.parent.GetComponent<BoxBehaviour>().Triggered(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            transform.parent.GetComponent<BoxBehaviour>().Triggered(gameObject);
        }
    }
}
