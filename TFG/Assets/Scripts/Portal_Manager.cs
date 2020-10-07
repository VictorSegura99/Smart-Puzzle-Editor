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
                collider.transform.position = new Vector3(Purple_Portal.transform.position.x, Purple_Portal.transform.position.y - 0.75f, Purple_Portal.transform.position.z);
            }
            else
            {
                collider.transform.position = new Vector3(Green_Portal.transform.position.x, Green_Portal.transform.position.y - 0.75f, Green_Portal.transform.position.z);
            }
        }
    }
}
