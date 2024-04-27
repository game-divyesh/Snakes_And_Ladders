using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WinPage : UIPageBase
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private TextMeshProUGUI winnerText;

    public override void ShowPage()
    {
        base.ShowPage();
        AudioManager.Instance.PlayAudioClip(ClipName.Victory);
        SetWinnerPlayers();
        DOVirtual.DelayedCall(0.5f, () =>
        {
            continueBtn.interactable = true;
        });
    }

    public void OnContinueButtonClick()
    {
        continueBtn.interactable = false;
        GameManager.Instance.UnloadGamePlayScene();
        GameManager.Instance.ResetGameOnComplete();
        UIManager.Instance.ShowSinglePage(UIPageType.TITLE);
    }


    private void SetWinnerPlayers()
    {
        string players = string.Empty;

        for (int i = 0; i < GameManager.Instance.winnerPlayers.Count; i++)
        {
            string playerName = GameManager.Instance.winnerPlayers[i].name.Split('_')[1];
            switch (playerName)
            {
                case "1":
                    players += $"<b>{i+1}</b>. Player <color=blue>One</color>\n";
                    break;

                case "2":
                    players += $"<b>{i + 1}</b>. Player <color=green>Two</color>\n";
                    break;

                case "3":
                    players += $"<b>{i + 1}</b>. Player <color=red>Three</color>\n";
                    break;

                case "4":
                    players += $"<b>{i + 1}</b>. Player <color=yellow>Four</color>\n";
                    break;

            }
        }
       winnerText.text = players;
    }

}// CLASS
