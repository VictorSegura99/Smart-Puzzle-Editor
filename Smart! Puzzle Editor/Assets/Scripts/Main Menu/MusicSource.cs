using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Music");
        if (go && go != gameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.tag = "Music";
            DontDestroyOnLoad(gameObject);
        }
    }
}
