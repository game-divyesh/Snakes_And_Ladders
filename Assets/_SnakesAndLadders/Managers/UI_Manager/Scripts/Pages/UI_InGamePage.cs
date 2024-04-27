using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGamePage : UIPageBase
{

    [Space(5)]
    [SerializeField] private TextMeshProUGUI insText;
    [SerializeField] private Toggle skipTurnToggle;

    public override void ShowPage()
    {
        base.ShowPage();
        ActionOnPlayerStop();

        //skipTurnToggle.transform.parent.gameObject.SetActive(true);

    }

    private void OnEnable()
    {
        Dice.onDiceStopped += ActionOnDiceStopped;
        Player.onPlayerStopMoving += ActionOnPlayerStop;
    }

    private void OnDisable()
    {
        Dice.onDiceStopped -= ActionOnDiceStopped;
        Player.onPlayerStopMoving -= ActionOnPlayerStop;
    }

    public void OnPauseBtnClick()
    {
        UIManager.Instance.ShowPage(UIPageType.PAUSE);
    }



    private void ActionOnDiceStopped(int diceCount)
    {
        //Debug.Log($"<color=yellow><b>OnDiceStopped call -> {Time.realtimeSinceStartup}</b></color>");
        Player player = GameManager.Instance.playerList[GameManager.Instance.whosTurn];

        if (player.isSkipTurn || (player.previousDiceNum == 6 && diceCount == 6 && player.isTurnAgain))
        {
            DOVirtual.DelayedCall(.05f, () =>
            {
                ActionOnPlayerStop();
            });
        }
        else
        {
            string diceNum = string.Empty;

            if (!player.isTurnAgain && player.isSkipUsed && player.previousDiceNum > -1)
                diceNum = $"{diceCount} + {player.previousDiceNum}";
            else
                diceNum = $"{diceCount}";

            switch (GameManager.Instance.whosTurn)
            {
                case 0:
                    insText.text = $"Player One have <color=green>{diceNum}</color>";
                    break;

                case 1:
                    insText.text = $"Player Two have <color=green>{diceNum}</color>";
                    break;

                case 2:
                    insText.text = $"Player Three have <color=green>{diceNum}</color>";
                    break;

                case 3:
                    insText.text = $"Player Four have <color=green>{diceNum}</color>";
                    break;
            }
        }
    }


    private void ActionOnPlayerStop()
    {
        DOVirtual.DelayedCall(0f, () =>
        {
            EnableDisableSkipTurnToggle();

            switch (GameManager.Instance.whosTurn)
            {
                case 0:
                    insText.text = $"Player <color=green>One</color> Turn";
                    break;

                case 1:
                    insText.text = $"Player <color=green>Two</color> Turn";
                    break;

                case 2:
                    insText.text = $"Player <color=green>Three</color> Turn";
                    break;

                case 3:
                    insText.text = $"Player <color=green>Four</color> Turn";
                    break;
            }

        });


    }


    private void EnableDisableSkipTurnToggle()
    {
        skipTurnToggle.isOn = false;
        if (GameManager.Instance.playerList[GameManager.Instance.whosTurn].isSkipUsed)
            skipTurnToggle.transform.parent.gameObject.SetActive(false);
        else
            skipTurnToggle.transform.parent.gameObject.SetActive(true);
    }

    public void OnSkipTurnToggleClick()
    {
        if (skipTurnToggle.isOn)
            GameManager.Instance.playerList[GameManager.Instance.whosTurn].isSkipTurn = true;
    }
}// CLASS
