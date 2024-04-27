using UnityEngine;

public class PrefManager : MonoBehaviour
{
    public static PrefManager Instance;
    private void Awake() => Instance = this;

    private string currentLevel = "CurrentLevel";
    internal int Current_Level
    {
        get
        {
            return PlayerPrefs.GetInt(currentLevel, 1);
        }
        set
        {
            PlayerPrefs.SetInt(currentLevel, value);
            PlayerPrefs.Save();
        }
    }

}// CLASS
