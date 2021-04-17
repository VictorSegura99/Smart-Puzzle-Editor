using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBox : MonoBehaviour
{
    // Components
    SpriteRenderer activated_sprite;

    // Inspector Variables
    public GameObject element_linked;
    public bool is_pressed = false;
    public bool constant_pressure_needed = true;

    // Internal Variables
    DoorOpening door;
    Portal_Manager portal;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        activated_sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (!element_linked)
        {
            return;
        }

        if (element_linked.GetComponent<DoorOpening>() != null)
        {
            door = element_linked.GetComponent<DoorOpening>();
        }
        else if (element_linked.GetComponent<Portal_Manager>() != null)
        {
            portal = element_linked.GetComponent<Portal_Manager>();
        }


        if (door != null)
        {
            if (is_pressed)
            {
                activated_sprite.enabled = true;
                if (door.IsDoorClosed())
                {
                    door.OpenDoors(is_pressed);
                }
            }
            else
            {
                activated_sprite.enabled = false;
                if (!door.IsDoorClosed())
                {
                    door.OpenDoors(is_pressed);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Moving_Box")
        {
            Activate(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (constant_pressure_needed)
        {
            if (collision.tag == "Player" || collision.tag == "Moving_Box")
            {
                Activate(false);
            }
        }
    }

    void Activate(bool is_active)
    {
        activated_sprite.enabled = is_active;

        if (door != null)
        {
            door.OpenDoors(is_active);
        }
        else if (portal != null)
        {
            portal.ChangeState(is_active);
        }

        if (!constant_pressure_needed)
        {
            enabled = false;
        }
    }
}
