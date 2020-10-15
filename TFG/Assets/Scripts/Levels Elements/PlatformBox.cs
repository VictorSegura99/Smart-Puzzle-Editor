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

    public enum ELEMENT_LINKED
    {
        DOOR,
        PORTAL,
        NONE
    }
    [SerializeField]
    ELEMENT_LINKED element = ELEMENT_LINKED.NONE;

    // Internal Variables
    DoorOpening door;
    Portal_Manager portal;

    void Awake()
    {
        activated_sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        switch (element)
        {
            case ELEMENT_LINKED.DOOR:
                {
                    if (element_linked.GetComponent<DoorOpening>() != null)
                    {
                        door = element_linked.GetComponent<DoorOpening>();
                    }
                    break;
                }
            case ELEMENT_LINKED.PORTAL:
                {
                    if (element_linked.GetComponent<Portal_Manager>() != null)
                    {
                        portal = element_linked.GetComponent<Portal_Manager>();
                    }
                    break;
                }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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

        switch (element)
        {
            case ELEMENT_LINKED.DOOR:
                {
                    door.OpenDoors(is_active);
                    break;   
                }
            case ELEMENT_LINKED.PORTAL:
                {
                    portal.ChangeState(is_active);
                    break;
                }
        }

        if (!constant_pressure_needed)
        {
            enabled = false;
        }
    }
}
