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
            PuzzleEditorController.instance.HideAllInspectors();
        }
        /*

            transform.parent.gameObject.SetActive(false);
            var LEP = transform.parent.parent.gameObject.GetComponent<LinkElementPlaceholder>();
            if (LEP && LEP.elementLinked)
            {
                LEP.elementLinked.GetComponent<LinkElementPlaceholder>().canvas.gameObject.SetActive(false);
                PuzzleEditorController.instance.HidePath(LEP);
            }
        }*/
    }
}
