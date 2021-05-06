using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleElementPlaceHolder : MonoBehaviour
{
    public enum PuzzleElementType
    {
        Puzzle,
        Player,
        PlatformBox,
        PlatformCircle,
        Doors,
        MovingBox,
        VerticalLeftDoors,
        VerticalRightDoors,

        None = -1
    }

    public enum States
    {
        InLevel,
        Selected
    }

    public PuzzleElementType PEType = PuzzleElementType.None;
    [SerializeField]
    protected GameObject realObject;
    public  Canvas canvas;
    [SerializeField]
    bool addOffset = true;
    [SerializeField]
    Vector3 cursorOffset = new Vector3(0.5f, 0.5f);
    //public Image closingCanvas;

    protected States state = States.Selected;

    private void Start()
    {
        LevelManager.instance.replacingCallbacks.AddListener(Replace);
        LevelManager.instance.reActivatePE.AddListener(Activate);

        canvas.worldCamera = PuzzleEditorController.instance.cam;

        if (LevelManager.instance.mode == LevelManager.LevelMode.Play)
        {
            Replace();
        }
    }

    private void Update()
    {
        if (state == States.Selected)
        {
            Vector3 position = PuzzleEditorController.instance.baseTM.WorldToCell(PuzzleEditorController.instance.cam.ScreenToWorldPoint(Input.mousePosition));

            if (addOffset)
            {
                position += cursorOffset;
            }

            transform.position = position;
        }
    }

    virtual public void Replace()
    {
        Instantiate(realObject, transform.position, realObject.transform.rotation, LevelManager.instance.mainElementsPlay);
        gameObject.SetActive(false);
    }

    virtual public void ChangeState(States newState)
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

    protected virtual void OnMouseDown()
    {
        if (PuzzleEditorController.instance.GetCurrentTool() == PuzzleEditorController.Tools.Arrow)
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
    }

    virtual public void MoveObject()
    {
        ChangeState(States.Selected);
        PuzzleEditorController.instance.PuzzleElementSelected(gameObject, false);
        canvas.gameObject.SetActive(false);
    }

    virtual public void DeleteObject()
    {
        LevelManager.instance.reActivatePE.RemoveListener(Activate);
        LevelManager.instance.replacingCallbacks.RemoveListener(Replace);
        Destroy(gameObject);
    }

    void Activate()
    {
        gameObject.SetActive(true);
    }

    public void ShowCanvas(bool show)
    {
        if (show != canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(show);
        }
    }
}
