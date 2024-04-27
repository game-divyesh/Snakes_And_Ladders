using UnityEngine;

public class UIPageBase : MonoBehaviour
{
    public RectTransform pageRect;

    public bool isPageActive => gameObject.activeInHierarchy;



    public virtual void ShowPage()
    {
        this .gameObject.SetActive (true);
        pageRect.SetAsLastSibling();
    }

    public virtual void HidePage() 
    {
        this.gameObject.SetActive(false);
    }

    public virtual void OnBackButton()
    {
        HidePage();
    }

}// CLASS
