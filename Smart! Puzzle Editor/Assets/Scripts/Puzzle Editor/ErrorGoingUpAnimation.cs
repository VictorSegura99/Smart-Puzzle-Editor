using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorGoingUpAnimation : MonoBehaviour
{
    public Text errormessage;
    
    float time = 0;
    Color color = Color.white;
    RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * 0.75f;

        if (time < 1)
        {
            color.a = 1 - time;
        }
        else
        {
            Destroy(gameObject);
        }

        errormessage.color = color;

        Vector3 pos = rt.anchoredPosition;
        pos.y += 75 * Time.deltaTime;
        rt.anchoredPosition = pos;
    }
}
