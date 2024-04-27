using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Camera uiCamera;

    [SerializeField] private UIPageType startingPage;

    public List<UIPage> pageList = new List<UIPage>();

    [SerializeField] private bool enableDeviceBackButton = false;

    public Stack<UIPage> activePagesStack = new Stack<UIPage>();


    #region ------------------------ UNITY CALLBACKS ------------------------

    private void Awake() => Instance = this;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            HideTopPage();
    }

    #endregion

    private void Initialize()
    {
        HideAllPages();
        ShowSinglePage(startingPage);
    }

    public void ShowPage(UIPageType pageType)
    {
        var page = pageList.First(page => page.pageType == pageType);
        if (page is null)
        {
            Debug.Log($"<color=red><b>{pageType} is not found.</b></color>");
            return;
        }
        else
        {
            page.page.ShowPage();
            activePagesStack.Push(page);
            Debug.Log($"<color=yellow> Showing -> {pageType} \n Stack Count -> {activePagesStack.Count} \n Last ->{activePagesStack.LastOrDefault().pageType}</color>");
        }
    }

    public void HideTopPage()
    {
        if (activePagesStack.Count <= 1)
        {
            Debug.Log($"<color=red>Can't Hide Current page</color>");
            return;
        }
        else
        {
            var page = activePagesStack.Pop();
            page.page.HidePage();
            activePagesStack.Peek().page.ShowPage();
            Debug.Log($"<color=green>Hiding ->{page.pageType} \n Showing -> {activePagesStack.Peek().pageType}</color>");
        }

    }


    public void ShowSinglePage(UIPageType pageType)
    {
        HideAllActivePages();

        ShowPage(pageType);
    }


    public void HideSinglePage(UIPageType pageType)
    {
        var page = pageList.First(page => page.pageType == pageType);
        if (page is null)
        {
            Debug.Log($"<color=red><b>{pageType} is not found.</b></color>");
            return;
        }
        else
        {
            if (activePagesStack.Contains(page))
                activePagesStack.Pop();
            page.page.HidePage();
        }
    }
    private void HideAllActivePages()
    {
        if (activePagesStack.Count > 0)
        {
            foreach (var uiPage in activePagesStack)
            {
                uiPage.page.HidePage();
            }
            activePagesStack.Clear();
        }
    }

    private void HideAllPages()
    {
        activePagesStack.Clear();
        foreach (var uiPage in pageList)
        {
            uiPage.page.HidePage();
        }
    }

}// CLASS


[Serializable]
public class UIPage
{
    public string name;
    public UIPageBase page;
    public UIPageType pageType;
}

[Serializable]
public enum UIPageType
{
    DEFAULT = 0,

    TITLE,
    INGAME,
    WIN,
    LOSE,
    PAUSE,
    SELECT_PLAYER,

}// Pages


