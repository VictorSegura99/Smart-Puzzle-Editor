using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkElementPlaceholder : PuzzleElementPlaceHolder
{
    public enum LinkElementType
    {
        Receiver,
        Activator
    }

    public LinkElementType type;
    public GameObject elementLinked;
    public Text buttonText; 

    [HideInInspector]
    public GameObject objectGenerated;

    Vector3 oldPos;

    public override void Replace()
    {
        switch (type)
        {
            case LinkElementType.Receiver:
                if (!objectGenerated)
                    GenerateObject();
                break;
            case LinkElementType.Activator:
                GameObject rO = Instantiate(realObject, transform.position, realObject.transform.rotation, LevelManager.instance.mainElementsPlay);
                if (elementLinked)
                {
                    if (!elementLinked.GetComponent<LinkElementPlaceholder>().objectGenerated)
                    {
                        elementLinked.GetComponent<LinkElementPlaceholder>().GenerateObject();
                    }
                    rO.GetComponent<PlatformBox>().element_linked = elementLinked.GetComponent<LinkElementPlaceholder>().objectGenerated;
                }
                break;
        }

        gameObject.SetActive(false);
    }

    public void GenerateObject()
    {
        objectGenerated = Instantiate(realObject, transform.position, realObject.transform.rotation, LevelManager.instance.mainElementsPlay);
    }

    public void Link()
    {
        PuzzleEditorController.instance.LinkElement(this);
    }

    public override void DeleteObject()
    {
        PuzzleEditorController.instance.DeleteLinkingObject(this);

        if (elementLinked)
        {
            LinkElementPlaceholder LEP = elementLinked.GetComponent<LinkElementPlaceholder>();
            LEP.elementLinked = null;
            LEP.buttonText.text = "Link";
        }

        base.DeleteObject();
    }

    public void ShowCanvas(bool show)
    {
        if (show != canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(show);
        }
    }

    protected override void OnMouseDown()
    {
        if (PuzzleEditorController.instance.GetCurrentTool() == PuzzleEditorController.Tools.Arrow)
        {
            if (state == States.InLevel)
            {
                canvas.gameObject.SetActive(true);

                if (elementLinked)
                {
                    if (buttonText.text != "Unlink")
                    {
                        buttonText.text = "Unlink";
                    }

                    if (elementLinked.GetComponent<LinkElementPlaceholder>().buttonText.text != "Unlink")
                    {
                        elementLinked.GetComponent<LinkElementPlaceholder>().buttonText.text = "Unlink";
                    }

                    LinkElementPlaceholder LEP = elementLinked.GetComponent<LinkElementPlaceholder>();
                    LEP.canvas.gameObject.SetActive(true);

                    PuzzleEditorController.instance.ShowPath(this);
                }
            }
            else
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    public override void ChangeState(States newState)
    {
        base.ChangeState(newState);

        if (newState == States.InLevel && elementLinked)
        {
            PuzzleEditorController.instance.MovedLinkingObject(this, oldPos);
        }
    }
    public override void MoveObject()
    {
        oldPos = transform.position;
        base.MoveObject();

        if (elementLinked)
        {
            elementLinked.GetComponent<LinkElementPlaceholder>().canvas.gameObject.SetActive(false);
            PuzzleEditorController.instance.HidePath(this);
        }
    }
}
