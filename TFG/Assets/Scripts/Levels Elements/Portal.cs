using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // Components
    Animator anim;
    Portal_Manager manager;
    Collider2D trigger;

    // Inspector variables

    // Internal Variables

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        manager = transform.parent.GetComponent<Portal_Manager>();
        trigger = GetComponent<Collider2D>();
        trigger.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && !collision.CompareTag("Moving_Box"))
        {
            return;
        }

        manager.PortalTriggered(this, collision);
    }

    public void Open()
    {
        float moment = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("CLOSING"))
        {
            anim.Play("OPENING", 0, 1 - moment);
        }
        else if(!anim.GetCurrentAnimatorStateInfo(0).IsName("IDLE"))
        {
            anim.SetTrigger("Open");
        }

        trigger.enabled = true;
    }

    public void Close()
    {
        float moment = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("OPENING"))
        {
            anim.Play("CLOSING", 0, 1 - moment);
        }
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("CLOSED"))
        {
            anim.SetTrigger("Close");
        }

        trigger.enabled = false;
    }
}
