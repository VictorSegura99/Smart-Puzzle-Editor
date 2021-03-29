using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElementButton : MonoBehaviour
{
    [SerializeField]
    GameObject element;

    public void ElementSelected()
    {
        PuzzleEditorController.instance.PuzzleElementSelected(element);
    }
}
