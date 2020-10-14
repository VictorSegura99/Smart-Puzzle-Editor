using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_Trigger : MonoBehaviour
{
    // Components
    SpriteRenderer sprite;

    // Inspector Variables
    [SerializeField]
    float time_to_fade = 0.5f;

    // Internal Variables
    float time_start = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Time.timeScale = 0;
            StartCoroutine(FadePuzzle());
        }
    }

    IEnumerator FadePuzzle()
    {
        time_start = Time.realtimeSinceStartup;

        while (sprite.color.a > 0)
        {
            float t = 1 - (Time.realtimeSinceStartup - time_start) / time_to_fade;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, t);

            yield return null;
        }

        GameObject.Find("Levels_UI").GetComponent<UI_Manager>().ShowYouWinMenu();
        Destroy(gameObject);
    }
}
