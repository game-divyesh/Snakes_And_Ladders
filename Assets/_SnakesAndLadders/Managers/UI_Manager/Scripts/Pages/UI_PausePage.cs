using UnityEngine;
using UnityEngine.UI;

public class UI_PausePage : UIPageBase
{
    [Space]
    [Header("SOUND")]
    [SerializeField] private Image soundImg;
    [SerializeField] private Sprite onSoundIcon;
    [SerializeField] private Sprite offSoundIcon;

    [Space(5)]
    [Header("MUSIC")]
    [SerializeField] private Image musicImg;
    [SerializeField] private Sprite onMusicIcon;
    [SerializeField] private Sprite offMusicIcon;


    public override void ShowPage()
    {
        base.ShowPage();
        Time.timeScale = 0;
        soundImg.sprite = AudioManager.Instance.IsSoundOn ? onSoundIcon : offSoundIcon;
        musicImg.sprite = AudioManager.Instance.IsMusicOn ? onMusicIcon : offMusicIcon;
    }

    public override void HidePage()
    {
        base.HidePage();
        Time.timeScale = 1;
    }

    #region -------------- BUTTON METHODS --------------

    public void OnHomeBtnClick()
    {

       /* UIManager.Instance.ShowSinglePage(UIPageType.TITLE);
        GameManager.Instance.UnloadScene();*/
    }

    public void OnSoundBtnClick()
    {
        AudioManager.Instance.IsSoundOn = !AudioManager.Instance.IsSoundOn;
        soundImg.sprite = AudioManager.Instance.IsSoundOn ? onSoundIcon : offSoundIcon;
    }

    public void OnMusicBtnClick()
    {
        AudioManager.Instance.IsMusicOn = !AudioManager.Instance.IsMusicOn;
        musicImg.sprite = AudioManager.Instance.IsMusicOn ? onMusicIcon : offMusicIcon;
    }

    public void OnRetryBtnClick()
    {


        /*UIManager.Instance.ShowSinglePage(UIPageType.INGAME);
        GameManager.Instance.UnloadScene();
        GameManager.Instance.LoadCurrentLevel();*/
    }

    #endregion

}// CLASS
