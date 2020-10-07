using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // Components
    Animator anim;

    // Inspector variables

    // Internal Variables

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG:
        // Open/Close Portals
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (anim.GetCurrentAnimatorStateInfo(0).IsName("IDLE"))
        //    {
        //        anim.SetTrigger("Close");
        //    }
        //    else if (anim.GetCurrentAnimatorStateInfo(0).IsName("CLOSED"))
        //    {
        //        anim.SetTrigger("Open");
        //    }
        //}
    }
}
