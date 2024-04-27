using UnityEngine;
using UnityEngine.UI;

public class UI_TitlePage : UIPageBase
{
    [Space(8)]
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
        soundImg.sprite = AudioManager.Instance.IsSoundOn ? onSoundIcon : offSoundIcon;
        musicImg.sprite = AudioManager.Instance.IsMusicOn ? onMusicIcon : offMusicIcon;
    }


    public void OnPlayBtnClick()
    {
        UIManager.Instance.ShowPage(UIPageType.SELECT_PLAYER);
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

   



}// CLASS
