
using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectPlayerPage : UIPageBase
{
    [SerializeField] private ToggleGroup toggleGroup;

    public void OnPlayerCountChanged()
    {
        // Find the active toggle in the toggle group
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();

        if (activeToggle is not null)
        {
            switch (activeToggle.transform.parent.name)
            {
                case "2Player":
                    GameManager.Instance.totalPlayer = PlayerCount.TwoPlayer;
                    break;
                case "3Player":
                    GameManager.Instance.totalPlayer = PlayerCount.ThreePlayer;
                    break;
                case "4Player":
                    GameManager.Instance.totalPlayer = PlayerCount.FourPlayer;
                    break;
                default:
                    GameManager.Instance.totalPlayer = PlayerCount.TwoPlayer;// Default to 2 players
                    break;
            }

        }
    }


    public void OnStartBtnClick()
    {
        GameManager.Instance.LoadGamePlayScene();

        DOVirtual.DelayedCall(1, () =>
        {
            UIManager.Instance.ShowSinglePage(UIPageType.INGAME);
            GameManager.Instance.SetDataOnGamePlayStart();
        });
    }

}// CLASS
