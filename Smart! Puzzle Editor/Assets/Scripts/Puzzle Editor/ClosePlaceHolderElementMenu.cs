using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClosePlaceHolderElementMenu : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (transform.parent.parent.GetComponent<LinkElementPlaceholder>())
                PuzzleEditorController.instance.HideAllInspectors();
            else
                transform.parent.parent.GetComponent<PuzzleElementPlaceHolder>().ShowCanvas(false);
        }
    }
}
