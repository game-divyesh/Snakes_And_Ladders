using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceThrower : MonoBehaviour
{
    public static DiceThrower Instance;

    

    [SerializeField] private float throwForce;
    [SerializeField] private float rollForce;
    [Space(5)]
    [SerializeField] private Dice dice;

    private void Awake() => Instance = this;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) &&!IsMouseOverUI() && GameManager.Instance.canRollDice)
        {
            GameManager.Instance.canRollDice = false;
            ThrowDice();
            AudioManager.Instance.PlayAudioClip( ClipName.DiceRoll);
        }
    }

    public void ThrowDice() 
    {
        dice.RollDice(throwForce, rollForce);
    }

    public void ResetDice()
    {
        dice.transform.localPosition = new Vector3(0,0.5f,0);
        dice.transform.rotation = Quaternion.identity;
    }

    public bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}// CLASS
