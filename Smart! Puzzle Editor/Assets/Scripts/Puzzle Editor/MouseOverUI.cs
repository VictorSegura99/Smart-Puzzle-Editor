using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        PuzzleEditorController.instance.BlockMouse(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PuzzleEditorController.instance.BlockMouse(false);
    }
}
