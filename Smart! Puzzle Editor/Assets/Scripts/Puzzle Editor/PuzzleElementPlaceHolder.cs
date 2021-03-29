using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElementPlaceHolder : MonoBehaviour
{
    public enum States
    {
        InLevel,
        Selected
    }
    
    [SerializeField]
    GameObject realObject;
    [SerializeField]
    Canvas canvas;

    States state = States.Selected;

    private void Start()
    {
        canvas.worldCamera = PuzzleEditorController.instance.cam;
    }

    private void Update()
    {
        if (state == States.Selected)
        {
            Vector3 position = PuzzleEditorController.instance.baseTM.WorldToCell(PuzzleEditorController.instance.cam.ScreenToWorldPoint(Input.mousePosition));
            position.x += 0.5f;
            position.y += 0.5f;
            transform.position = position;
        }
    }

    public void Replace()
    {
        Instantiate(realObject, transform.position, realObject.transform.rotation);
        Destroy(gameObject);
    }

    public void ChangeState(States newState)
    {
        switch (newState)
        {
            case States.InLevel:
                if (canvas.gameObject.activeSelf)
                    canvas.gameObject.SetActive(false);
                break;
            case States.Selected:
                if (canvas.gameObject.activeSelf)
                    canvas.gameObject.SetActive(false);
                break;
        }

        state = newState;
    }

    private void OnMouseDown()
    {
        if (state == States.InLevel)
        {
            canvas.gameObject.SetActive(true);
        }
        else
        {
            canvas.gameObject.SetActive(false);
        }
    }

    public void MoveObject()
    {
        ChangeState(States.Selected);
        PuzzleEditorController.instance.PuzzleElementSelected(gameObject);
        Destroy(gameObject);
    }

    public void DeleteObject()
    {
        Destroy(gameObject);
    }
}
