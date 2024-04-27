using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour
{
    public delegate void OnDiceStopped(int count);
    public static OnDiceStopped onDiceStopped;
    //public static event Action<int> OnDiceStopped;

    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Transform[] diceFaces;

    [SerializeField] private bool hasStoppedRolling = false;
    [SerializeField] private bool hasDelayFinished = false;

    [Space(10)]
    [SerializeField] private int desireValue = 0;

    private Coroutine delayCoroutine;

    private void Update()
    {
        if (!hasDelayFinished)
            return;

        if (!hasStoppedRolling && rigidBody.angularVelocity.sqrMagnitude <= 0.1f)
        {
            hasStoppedRolling = true;
            hasDelayFinished = false;

            if (delayCoroutine is not null)
                StopCoroutine(delayCoroutine);

            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.velocity = Vector3.zero;

            if (desireValue > 0)
                GetDesireValue();
            else
                GetNumberTopFace();
        }
    }


    public void RollDice(float throwForce, float rollForce)
    {
        rigidBody.AddForce(Vector3.up * throwForce, ForceMode.Impulse);

        //rigidBody.AddTorque(Vector3.one * rollForce, ForceMode.Impulse);

        rigidBody.AddTorque(RandomRotation(rollForce), ForceMode.Impulse);

        delayCoroutine = StartCoroutine(WaitForDiceStop());

        hasStoppedRolling = false;
    }


    private IEnumerator WaitForDiceStop()
    {
        yield return new WaitForFixedUpdate();
        while (rigidBody.angularVelocity.sqrMagnitude > 0.1f)
            yield return new WaitForFixedUpdate();

        hasDelayFinished = true;
    }

    [ContextMenu("Get Number Top Face")]
    public void GetNumberTopFace()
    {
        int topFace = 0;
        float lastFaceYpos = diceFaces[0].position.y;


        for (int index = 1; index < diceFaces.Length; index++)
        {
            if (diceFaces[index].position.y > lastFaceYpos)
            {
                lastFaceYpos = diceFaces[index].position.y;
                topFace = index;
            }
        }

        Debug.Log($"Dice{topFace + 1}");
        onDiceStopped?.Invoke(topFace + 1);
    }

    private Vector3 RandomRotation(float rollForce)
    {
        float x = Random.Range(-rollForce, rollForce + 1);
        if (x < 1 && x > -1)
            x = 1.5f;

        float y = Random.Range(-rollForce, rollForce + 1);
        if (y < 1 && y > -1)
            y = 1.5f;

        float z = Random.Range(-rollForce, rollForce + 1);
        if (z < 1 && z > -1)
            z = 1.5f;

        return new Vector3(x, y, z);
    }

    public void GetDesireValue()
    {
        onDiceStopped?.Invoke(desireValue);
    }
}// CLASS

public enum DiceNumber
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
}