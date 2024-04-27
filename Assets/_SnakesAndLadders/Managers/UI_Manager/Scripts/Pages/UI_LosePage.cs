using UnityEditor;
using UnityEngine.SceneManagement;

public class UI_LosePage : UIPageBase
{

    public void OnHomeButtonClick()
    {
       
        UIManager.Instance.ShowSinglePage(UIPageType.TITLE);
    }

    public void OnRetryButtonClick()
    {
        /*if (GameManager.Instance.isChallengeMode)
        {
            SceneManager.LoadScene(GameManager.Instance.currentChallengeLevel);
        }
        else
        {
            GameManager.Instance.UnloadScene();
            GameManager.Instance.LoadCurrentLevel();
        }
        UIManager.Instance.ShowSinglePage(UIPageType.INGAME);*/

    }

}// CLASS
