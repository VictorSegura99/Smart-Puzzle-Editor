using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElementButtonController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    enum Elements
    {
        Menus,
        Tiles,
        Elements
    }

    [SerializeField]
    Elements element = Elements.Tiles;
    [SerializeField]
    UIEditorManager.Menus menuToOpen = UIEditorManager.Menus.None;

    RectTransform rt;
    Vector3 originalScale;

    Coroutine currentScaling = null;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (element)
        {
            case Elements.Menus:
                UIEditorManager.instance.ChangeMenu(menuToOpen);
                break;
            case Elements.Tiles:
                GetComponent<TileButton>().TileSelected();
                break;
            case Elements.Elements:
                GetComponent<PuzzleElementButton>().ElementSelected();
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentScaling != null)
        {
            StopCoroutine(currentScaling);
        }

        currentScaling = StartCoroutine(Scale(1.05f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentScaling != null)
        {
            StopCoroutine(currentScaling);
        }

        currentScaling = StartCoroutine(Scale(1));
    }

    IEnumerator Scale(float scale)
    {
        Vector3 startScale = transform.localScale;
        Vector3 desiredScale = originalScale * scale;

        float timeStart = Time.time;

        while (transform.localScale != desiredScale)
        {
            float t = (Time.time - timeStart) / 0.1f;

            if (t < 1)
                transform.localScale = Vector3.Lerp(startScale, desiredScale, t);
            else
                transform.localScale = desiredScale;

            yield return null;
        }

        currentScaling = null;
    }
}
