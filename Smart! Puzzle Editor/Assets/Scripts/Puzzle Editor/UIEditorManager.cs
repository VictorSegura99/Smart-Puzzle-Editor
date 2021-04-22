using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEditorManager : MonoBehaviour
{
    static public UIEditorManager instance;

    public enum Menus
    {
        Groups,
        Grounds,
        Walls,
        PuzzleElements,

        None = -1
    }

    [Header("Menus")]
    public GameObject selectingSizeMenu;
    public CanvasGroup mainPanel;
    public CanvasGroup toolsPanel;
    public GameObject saveLoadMenu;
    public GameObject resetLevelMenu;
    [SerializeField]
    GameObject groupsMenu;
    [SerializeField]
    GameObject groundsMenu;
    [SerializeField]
    GameObject wallsMenu;
    [SerializeField]
    GameObject puzzleElementsMenu;

    [Header("Menus Pages")]
    [SerializeField]
    Text pageIndicator;
    [SerializeField]
    GameObject[] wallsPages;


    [HideInInspector]
    public Menus currentMenu = Menus.Groups;

    int currentWallsPage = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ChangeMenu(Menus newMenu)
    {
        switch (newMenu)
        {
            case Menus.Groups:
                switch (currentMenu)
                {
                    case Menus.Grounds:
                        groundsMenu.SetActive(false);
                        break;
                    case Menus.Walls:
                        wallsMenu.SetActive(false);
                        break;
                    case Menus.PuzzleElements:
                        puzzleElementsMenu.SetActive(false);
                        break;
                }

                groupsMenu.SetActive(true);
                break;
            case Menus.Grounds:
                groupsMenu.SetActive(false);
                groundsMenu.SetActive(true);
                break;
            case Menus.Walls:
                groupsMenu.SetActive(false);
                wallsMenu.SetActive(true);
                break;
            case Menus.PuzzleElements:
                groupsMenu.SetActive(false);
                puzzleElementsMenu.SetActive(true);
                break;
        }

        currentMenu = newMenu;
    }

    public void ChangeWallsPage(int change)
    {
        currentWallsPage += change;
        if (currentWallsPage >= wallsPages.Length)
        {
            currentWallsPage = 0;
        }
        else if (currentWallsPage < 0)
        {
            currentWallsPage = wallsPages.Length - 1;
        }

        for (int i = 0; i < wallsPages.Length; ++i)
        {
            if (currentWallsPage == i && !wallsPages[i].activeSelf)
            {
                wallsPages[i].SetActive(true);
            }
            else if (wallsPages[i].activeSelf && currentWallsPage != i)
            {
                wallsPages[i].SetActive(false);
            }
        }

        pageIndicator.text = "Page " + currentWallsPage.ToString();
    }

    public void ReturnGroupsMenu()
    {
        ChangeMenu(Menus.Groups);
    }

    public void BlockPanelsUIRaycast(bool block)
    {
        if (mainPanel.interactable != !block)
        {
            mainPanel.interactable = !block;
        }

        if (toolsPanel.interactable != !block)
        {
            toolsPanel.interactable = !block;
        }
    }
}
