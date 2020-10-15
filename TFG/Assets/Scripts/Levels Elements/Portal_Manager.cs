using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Manager : MonoBehaviour
{
    // Components

    // Inspector variables
    [SerializeField]
    GameObject Green_Portal;
    [SerializeField]
    GameObject Purple_Portal;
    [SerializeField]
    float cool_time = 1.0f;

    // Internal Variables
    float time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PortalTriggered(Portal which_is, Collider2D collider)
    {
        if (time + cool_time <= Time.realtimeSinceStartup)
        {
            time = Time.realtimeSinceStartup;
            if (which_is == Green_Portal.GetComponent<Portal>())
            {
                collider.transform.position += Purple_Portal.transform.position - Green_Portal.transform.position;
            }
            else
            {
                collider.transform.position += Green_Portal.transform.position - Purple_Portal.transform.position;
            }
        }
    }

    public void ChangeState(bool want_to_open)
    {
        if(want_to_open)
        {
            Green_Portal.GetComponent<Portal>().Open();
            Purple_Portal.GetComponent<Portal>().Open();
        }
        else
        {
            Green_Portal.GetComponent<Portal>().Close();
            Purple_Portal.GetComponent<Portal>().Close();
        }
    }
}
